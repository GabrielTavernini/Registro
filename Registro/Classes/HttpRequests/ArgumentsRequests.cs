using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Registro.Classes.HttpRequests;
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
            try
            {
                String argsPage = await getArgsPageAsync();
                await extratArgsAsync(argsPage);
                App.Arguments = tempArgs;
                tempArgs = new List<Arguments>();
                return argsPage;
                
            }
            catch { return "failed"; }
        }

        public async Task<Boolean> refreshArguments()
        {
            tempArgs.Clear();
            if (!globalRefresh)
                if (!await LoginAsync())
                    return false;
            try
            {
                String Page = await getArgsPageAsync();
                await extratArgsAsync(Page);

                //done checking for errors! 
                //let's save and notify new data
                if (App.Settings.notifyArguments)
                {
                    List<Arguments> list3 = tempArgs.Except(App.Arguments, new ArgumentsComparer()).ToList();

                    if(list3.Count < 6)
                    {
                        for (int i = 0; i < list3.Count(); i++)
                        {
                            if (Device.RuntimePlatform == Device.Android)
                                DependencyService.Get<INotifyAndroid>().NotifyArguments(list3[i], i);
                            else
                                DependencyService.Get<INotifyiOS>().NotifyArguments(list3[i]);
                        }   
                    }
                }

                App.Arguments = tempArgs;
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Xamarin.Forms.Application.Current.Properties["arguments"] = JsonConvert.SerializeObject(App.Arguments, Formatting.Indented, jsonSettings);
                return true;
            }
            catch { return false; }
 
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getArgsPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<String> getArgsPageAsync()
        {
            String pageSource = await Utility.GetPageAsync(User.school.argsUrl);
            if (pageSource.Contains("Warning") || pageSource.Contains("Fatal error"))
                throw new System.InvalidOperationException("Wrong Page");

            return pageSource;
        }

        public async Task<String> getArgsSubPageAsync(String id)
        {
            return await Utility.PostPageAsync(User.school.argsUrl, "idmateria=" + id, User.school.argsUrl);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratArgs-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<Boolean> extratArgsAsync(String html)
        {
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Dictionary<String, String> subjects = new Dictionary<String, String>();
            Supremes.Nodes.Element selector = doc.Select("body > div.contenuto > form > table > tbody > tr > td > select").First;

            if (selector == null)
                return false;

            Elements options;
            options = selector.GetElementsByTag("option");




            foreach (Supremes.Nodes.Element option in options)
            {
                if (option.Attributes["value"] != null)
                {
                    subjects.Add(option.Attributes["value"], option.Text);
                }
            }

            foreach (KeyValuePair<String, String> KVp in subjects)
            {
                String s = await getArgsSubPageAsync(KVp.Key);
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



}
