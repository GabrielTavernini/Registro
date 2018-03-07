using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Supremes;
using Supremes.Nodes;
using Xamarin.Forms;
using static Registro.Controls.Notifications;

namespace Registro
{
    public class ArgumentsRequests : HttpRequest
    {
        private List<Arguments> tempArgs = new List<Arguments>();
        
        public async Task<String> extractAllArguments()
        {
            String argsPage = await getArgsPageAsync();
            await extratArgsAsync(argsPage);
            App.Arguments = tempArgs;
            tempArgs = new List<Arguments>();
            return argsPage;
        }

        public async Task<Boolean> refreshArguments()
        {
            if (!await LoginAsync())
                return false;

            String Page = await getArgsPageAsync();
            if (Page == "failed")
                return false;


            if(await extratArgsAsync(Page))
            {
                //done checking for errors! 
                //let's save and notify new data
                if(App.Settings.notifyArguments)
                {
                    List<Arguments> list3 = tempArgs.Except(App.Arguments, new ArgumentsComparer()).ToList();

                    for (int i = 0; i < list3.Count(); i++)
                    {
                        if (Device.RuntimePlatform == Device.Android)
                            DependencyService.Get<INotifyAndroid>().NotifyArguments(list3[i], i);
                        else
                            DependencyService.Get<INotifyiOS>().NotifyArguments(list3[i]);
                    }  
                }

                App.Arguments = tempArgs;
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Xamarin.Forms.Application.Current.Properties["arguments"] = JsonConvert.SerializeObject(App.Arguments, Formatting.Indented, jsonSettings);
                return true; 
            }
            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getArgsPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<String> getArgsPageAsync()
        {
            try
            {
                string pageSource;
                HttpRequestMessage getRequest = new HttpRequestMessage();
                getRequest.RequestUri = new Uri(User.school.argsUrl);
                getRequest.Headers.Add("Cookie", cookies);
                getRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                getRequest.Headers.Add("Refer", User.school.loginUrl);
                //getRequest.Headers.Add("UserAgent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.84 Safari / 537.36");

                HttpResponseMessage getResponse = await new HttpClient(new NativeMessageHandler()).SendAsync(getRequest);

                pageSource = await getResponse.Content.ReadAsStringAsync();

                getRequest.Dispose();
                getResponse.Dispose();

                return pageSource;
            }
            catch { return "failed"; }
        }

        public async Task<String> getArgsSubPageAsync(String id)
        {
            string formParams = "idmateria=" + id;

            HttpRequestMessage req = new HttpRequestMessage();
            Uri uri = new Uri(User.school.argsUrl);
            req.RequestUri = uri;
            req.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            req.Headers.Add("Cookie", cookies);
            req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            req.Headers.Add("UserAgent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.84 Safari / 537.36");
            req.Headers.Add("Referer", User.school.argsUrl);
            req.Method = HttpMethod.Post;

            byte[] bytes = Encoding.UTF8.GetBytes(formParams);
            req.Headers.TryAddWithoutValidation("Content-Length", bytes.Length.ToString());
            req.Content = new StringContent(formParams, Encoding.UTF8, "application/x-www-form-urlencoded");

            String pageSource;
            try
            {
                HttpResponseMessage resp = await new HttpClient(new NativeMessageHandler()).SendAsync(req);
                pageSource = await resp.Content.ReadAsStringAsync();
                resp.Dispose();
                req.Dispose();
                return pageSource;
            }
            catch { return "failed"; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratArgs-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<Boolean> extratArgsAsync(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Dictionary<String, String> subjects = new Dictionary<String, String>();
            Supremes.Nodes.Element selector = doc.Select("body > div.contenuto > form > table > tbody > tr > td > select").First;
            Elements options;
            try { options = selector.GetElementsByTag("option"); }
            catch { return false; }


            if (options == null)
                return false;

            foreach (Supremes.Nodes.Element option in options)
            {
                if (option.Attributes["value"] != null)
                {
                    subjects.Add(option.Attributes["value"], option.Text);
                }
                System.Diagnostics.Debug.WriteLine(option.Attributes["value"] + "    " + option.Text);
            }


            foreach (KeyValuePair<String, String> KVp in subjects)
            {
                String s = await getArgsSubPageAsync(KVp.Key);
                if(s == "failed")
                    return false;
                    
                extratArgsTable(s, KVp.Value);
            }

            return true;
        }

        public void extratArgsTable(String html, String currentSubject)
        {
            Document doc = Dcsoup.ParseBodyFragment(html, "");
            //Arguments currentArgument = new Arguments();

            for (int i = 2; ; i++)
            {
                Supremes.Nodes.Element table = doc.Select("body > div.contenuto > table > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");

                int Column = 1;
                foreach (Supremes.Nodes.Element inputElement in inputElements)
                {
                    if (Column == 1)
                    {
                        tempArgs.Add(new Arguments("", "", inputElement.Text, currentSubject));
                        Column++;
                    }
                    else if (Column == 2)
                    {
                        tempArgs[tempArgs.Count - 1].Argument = (inputElement.Text);
                        Column++;
                    }
                    else if (Column == 3)
                    {
                        tempArgs[tempArgs.Count - 1].Activity = inputElement.Text;
                        Column = 1;
                    }

                }
            }
        }
    }


    public class ArgumentsComparer : IEqualityComparer<Arguments>
    {
        public int GetHashCode(Arguments co)
        {
            if (co == null)
            {
                return 0;
            }
            String s = co.Argument + co.Activity + co.subject + co.date;
            return s.GetHashCode();
        }

        public bool Equals(Arguments x, Arguments y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;



            if (x.Activity == y.Activity
                && x.Argument == y.Argument
                && x.subject == y.subject
                && x.date == y.date)
                return true;
            else
                return false;
        }
    }
}
