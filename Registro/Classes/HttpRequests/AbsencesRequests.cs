using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Supremes;
using Supremes.Nodes;
using Xamarin.Forms;
using static Registro.Controls.Notifications;

namespace Registro.Classes.HttpRequests
{
    public class AbsencesRequests : HttpRequest
    {
        private List<Absence> tempAbsences = new List<Absence>();
        private List<LateEntry> tempLate = new List<LateEntry>();
        private List<EarlyExit> tempExit = new List<EarlyExit>();

        
        public async Task<String> extractAllAbsences()
        {
            try
            {
                String absencesPage = await getAbsencesPageAsync();
                extratAbsences(absencesPage);

                App.Absences = tempAbsences;
                App.EarlyExits = tempExit;
                App.LateEntries = tempLate;

                tempAbsences = new List<Absence>();
                tempLate = new List<LateEntry>();
                tempExit = new List<EarlyExit>();

                return absencesPage;
                
            }
            catch { return "failed"; }
        }

        public async Task<Boolean> refreshAbsence()
        {
            tempAbsences.Clear();
            if (!globalRefresh)
                if (!await LoginAsync())
                    return false;


            try
            {
                String Page = await getAbsencesPageAsync();
                extratAbsences(Page);


                if (App.Settings.notifyAbsences)
                {
                    List<Absence> list3 = tempAbsences.Except(App.Absences, new AbsencesComparer()).ToList();
                    for (int i = 0; i < list3.Count(); i++)
                    {
                        if (Device.RuntimePlatform == Device.Android)
                            DependencyService.Get<INotifyAndroid>().NotifyAbsence(list3[i], i - 9999); //-9999 offset from others notifications
                        else
                            DependencyService.Get<INotifyiOS>().NotifyAbsence(list3[i]);
                    }
                }


                App.Absences = tempAbsences;
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Xamarin.Forms.Application.Current.Properties["absences"] = JsonConvert.SerializeObject(App.Absences, Formatting.Indented, jsonSettings);
                return true;
            }
            catch { return false; }

        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<string> getAbsencesPageAsync()
        {
            String pageSource = await Utility.GetPageAsync(User.school.absencesUrl);
            if (pageSource.Contains("Warning") || pageSource.Contains("Fatal error"))
                throw new System.InvalidOperationException("Wrong Page");
            
            return pageSource;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratMarks-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public void extratAbsences(String html)
        {
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Supremes.Nodes.Element table = doc.Select("body > div.contenuto > table:nth-child(6) > tbody > tr:nth-child(6)").First;

            if (table == null) return;

            Elements inputElements = table.GetElementsByTag("td");

            int Column = 1;
            foreach (Supremes.Nodes.Element inputElement in inputElements)
            {
                if (Column == 1)
                {
                    foreach(String s in inputElement.Text.Split(' '))
                    {
                        if (s != "" && s != null && s.Length > 4)
                            tempAbsences.Add(new Absence("Assenza", s));
                    }
                    Column++;
                }
                else if (Column == 2)
                {
                    foreach (String s in inputElement.Text.Split(' '))
                    {
                        if (s != "" && s != null && s.Length > 4)
                            tempLate.Add(new LateEntry(s));
                    }                    
                    Column++;
                }
                else if (Column == 3)
                {
                    foreach (String s in inputElement.Text.Split(' '))
                    {
                        if (s != "" && s != null && s.Length > 4)
                            tempExit.Add(new EarlyExit(s));
                    }                    
                    Column = 1;
                }

            }
        }
    }
}