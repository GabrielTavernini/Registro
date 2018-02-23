using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Supremes;
using Supremes.Nodes;

namespace Registro
{
    public class MarksRequests : HttpRequest
    {

        static public async Task<String> extractAllMarks()
        {
            String marksPage = await getMarksPageAsync();
            extratMarks(marksPage);
            System.Diagnostics.Debug.WriteLine(marksPage);
            return marksPage;
        }

        static public async Task<Boolean> refreshMarks()
        {
            if (!await LoginAsync())
                return false;

            App.Subjects = new Dictionary<string, Subject>();
            App.Grades = new List<Grade>();

            String marksPage = await getMarksPageAsync();
            extratMarks(marksPage);

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            Xamarin.Forms.Application.Current.Properties["grades"] = JsonConvert.SerializeObject(App.Grades, Formatting.Indented, jsonSettings);
            return true;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        static public async Task<string> getMarksPageAsync()
        {
            string pageSource;
            HttpRequestMessage getRequest = new HttpRequestMessage();
            getRequest.RequestUri = new Uri(User.school.marksUrl);
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

        static public void extratMarks(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Subject currentSubject = new Subject("", false);
            Grade currentGrade = new Grade("", "", "", "", currentSubject, false);

            int Column = 0;
            for (int i = 2; ; i++)
            {
                Element table = doc.Select("body > div.contenuto > table > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");


                foreach (Element inputElement in inputElements)
                {
                    if ("4".Equals(inputElement.Attr("colspan")))
                    {
                        Column = 1;
                        currentSubject = new Subject(inputElement.Text.Replace("[", "").Replace("]", ""), true);
                    }
                    else if (Column == 1)
                    {
                        currentGrade = new Grade(inputElement.Text, "", "", "", currentSubject, true);
                        currentSubject.grades.Add(currentGrade);
                        Column++;
                    }
                    else if (Column == 2)
                    {
                        currentGrade.type = (inputElement.Text);
                        Column++;
                    }
                    else if (Column == 3)
                    {
                        currentGrade.setGrade(inputElement.Text);
                        Column++;
                    }
                    else if (Column == 4)
                    {
                        currentGrade.Description = (inputElement.Text);
                        Column = 1;
                    }

                }
            }
        }

    }
}
