using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Supremes;
using Supremes.Nodes;

namespace Registro
{
    public class ArgumentsRequests : HttpRequest
    {
        static private List<Arguments> tempArgs = new List<Arguments>();
        
        static public async Task<String> extractAllArguments()
        {
            String argsPage = await getArgsPageAsync();
            await extratArgsAsync(argsPage);
            App.Arguments = tempArgs;
            tempArgs = new List<Arguments>();
            return argsPage;
        }

        static public async Task<Boolean> refreshArguments()
        {
            if (!await LoginAsync())
                return false;


            String Page = await getArgsPageAsync();
            await extratArgsAsync(Page);

            if (tempArgs == App.Arguments)
            {
                tempArgs = new List<Arguments>();
                return true;                
            }
            else
            {
                App.Arguments = tempArgs;
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Xamarin.Forms.Application.Current.Properties["arguments"] = JsonConvert.SerializeObject(App.Arguments, Formatting.Indented, jsonSettings);
                tempArgs = new List<Arguments>();
                return true;                 
            }

        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getArgsPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        static public async Task<String> getArgsPageAsync()
        {
            string pageSource;
            HttpRequestMessage getRequest = new HttpRequestMessage();
            getRequest.RequestUri = new Uri(User.school.argsUrl);
            getRequest.Headers.Add("Cookie", cookies);
            getRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            getRequest.Headers.Add("UserAgent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.84 Safari / 537.36");

            HttpResponseMessage getResponse = await new HttpClient(new NativeMessageHandler()).SendAsync(getRequest);

            pageSource = await getResponse.Content.ReadAsStringAsync();

            getRequest.Dispose();
            getResponse.Dispose();

            return pageSource;
        }

        static public async Task<String> getArgsSubPageAsync(String id)
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
            catch { return ""; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratArgs-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        static public async Task<List<Arguments>> extratArgsAsync(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Dictionary<String, String> subjects = new Dictionary<String, String>();
            Element selector = doc.Select("body > div.contenuto > form > table > tbody > tr > td > select").First;

            foreach (Element option in selector.GetElementsByTag("option"))
            {
                if (option.Attributes["value"] != null)
                {
                    subjects.Add(option.Attributes["value"], option.Text);
                }
                System.Diagnostics.Debug.WriteLine(option.Attributes["value"] + "    " + option.Text);
            }


            foreach (KeyValuePair<String, String> KVp in subjects)
            {
                extratArgsTable(await getArgsSubPageAsync(KVp.Key), KVp.Value);
            }

            return tempArgs;
        }

        static public void extratArgsTable(String html, String currentSubject)
        {
            Document doc = Dcsoup.ParseBodyFragment(html, "");
            //Arguments currentArgument = new Arguments();

            for (int i = 2; ; i++)
            {
                Element table = doc.Select("body > div.contenuto > table > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");

                int Column = 1;
                foreach (Element inputElement in inputElements)
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
}
