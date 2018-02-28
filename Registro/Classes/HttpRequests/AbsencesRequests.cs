using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Supremes;
using Supremes.Nodes;

namespace Registro.Classes.HttpRequests
{
    public class AbsencesRequests : HttpRequest
    {
        private List<Absence> tempAbsences = new List<Absence>();
        
        public async Task<String> extractAllAbsences()
        {
            String absencesPage = await getAbsencesPageAsync();

            extratAbsences(absencesPage);
            System.Diagnostics.Debug.WriteLine(absencesPage);

            App.Absences = tempAbsences;
            tempAbsences = new List<Absence>();

            return absencesPage;
        }

        public async Task<Boolean> refreshAbsence()
        {
            if (!await LoginAsync())
                return false;


            String Page = await getAbsencesPageAsync();
            extratAbsences(Page);

            if (tempAbsences == App.Absences)
            {
                tempAbsences = new List<Absence>();
                return true;
            }
            else
            {
                App.Absences = tempAbsences;
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Xamarin.Forms.Application.Current.Properties["absences"] = JsonConvert.SerializeObject(App.Absences, Formatting.Indented, jsonSettings);
                tempAbsences = new List<Absence>();
                return true;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<string> getAbsencesPageAsync()
        {
            string pageSource;
            HttpRequestMessage getRequest = new HttpRequestMessage();
            getRequest.RequestUri = new Uri(User.school.absencesUrl);
            getRequest.Headers.Add("Cookie", cookies);
            getRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            getRequest.Headers.Add("UserAgent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.84 Safari / 537.36");

            HttpResponseMessage getResponse = await new HttpClient(new NativeMessageHandler()).SendAsync(getRequest);

            pageSource = await getResponse.Content.ReadAsStringAsync();

            getRequest.Dispose();
            getResponse.Dispose();

            return pageSource;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratMarks-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public void extratAbsences(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Element table = doc.Select("body > div.contenuto > table:nth-child(6) > tbody > tr:nth-child(6)").First;

            if (table == null) return;

            Elements inputElements = table.GetElementsByTag("td");

            int Column = 1;
            foreach (Element inputElement in inputElements)
            {
                if (Column == 1)
                {
                    foreach(String s in inputElement.Text.Split(' '))
                    {
                        System.Diagnostics.Debug.WriteLine(s);
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
                            tempAbsences.Add(new Absence("Ritardo", s));
                    }                    
                    Column++;
                }
                else if (Column == 3)
                {
                    foreach (String s in inputElement.Text.Split(' '))
                    {
                        if (s != "" && s != null && s.Length > 4)
                            tempAbsences.Add(new Absence("Uscita", s));
                    }                    
                    Column = 1;
                }

            }
        }
    }
}