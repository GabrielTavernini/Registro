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
using static Registro.Controls.AndroidNotifications;

namespace Registro
{
    public class MarksRequests : HttpRequest
    {
        private List<Grade> tempGrade = new List<Grade>();
        private Dictionary<String,Subject> tempSubject = new Dictionary<String, Subject>();

        public async Task<String> extractAllMarks()
        {
            String marksPage = await getMarksPageAsync();
            extratMarks(marksPage);
            System.Diagnostics.Debug.WriteLine(marksPage);

            App.Grades = tempGrade;
            App.Subjects = tempSubject;

            tempGrade = new List<Grade>();
            tempSubject = new Dictionary<String, Subject>();

            return marksPage;
        }

        public async Task<Boolean> refreshMarks()
        {
            tempGrade.Clear();
            tempSubject.Clear();
            if (!await LoginAsync())
                return false;

            String marksPage = await getMarksPageAsync();
            if (marksPage == "failed")
                return false;
            
            extratMarks(marksPage);
            if (!tempGrade.Any())
                return false;

            //done checking for errors! 
            //let's save and notify new data
            if(App.Settings.notifyMarks)
            {
                List<Grade> list3 = tempGrade.Except(App.Grades, new GradesComparer()).ToList();

                for (int i = 0; i < list3.Count(); i++)
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotify>().NotifyMark(list3[i], -i);
                } 
            }


            App.Grades = tempGrade;
            App.Subjects = tempSubject;
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            Xamarin.Forms.Application.Current.Properties["grades"] = JsonConvert.SerializeObject(App.Grades, Formatting.Indented, jsonSettings);
            return true;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<string> getMarksPageAsync()
        {
            try
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
            catch { return "failed"; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratMarks-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public void extratMarks(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Subject currentSubject = new Subject("");
            Grade currentGrade = new Grade("", "", "", "", currentSubject);

            int Column = 0;
            for (int i = 2; ; i++)
            {
                Supremes.Nodes.Element table = doc.Select("body > div.contenuto > table > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");


                foreach (Supremes.Nodes.Element inputElement in inputElements)
                {
                    if ("4".Equals(inputElement.Attr("colspan")))
                    {
                        currentSubject = new Subject(inputElement.Text.Replace("[", "").Replace("]", ""));

                        tempSubject.Add(currentSubject.name, currentSubject);
                        Column = 1;
                    }
                    else if (Column == 1)
                    {
                        currentGrade = new Grade(inputElement.Text, "", "", "", currentSubject);

                        currentSubject.grades.Add(currentGrade);
                        tempGrade.Add(currentGrade);
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

    public class GradesComparer : IEqualityComparer<Grade>
    {
        public int GetHashCode(Grade co)
        {
            if (co == null)
            {
                return 0;
            }
            String s = co.gradeString + co.subject.name + co.date + co.type + co.Description;
            return s.GetHashCode();
        }

        public bool Equals(Grade x, Grade y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;



            if (x.gradeString == y.gradeString
                && x.subject.name == y.subject.name
                && x.date == y.date
                && x.type == y.type
                && x.Description == y.Description)
                return true;
            else
                return false;
        }
    }
}
