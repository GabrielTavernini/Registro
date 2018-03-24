using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    public class MarksRequests : HttpRequest
    {
        private List<Grade> tempGrade = new List<Grade>();
        private Dictionary<String,Subject> tempSubject = new Dictionary<String, Subject>();

        public async Task<String> extractAllMarks()
        {
            try
            {
                String marksPage = await getMarksPageAsync();
                extratMarks(marksPage);

                App.Grades = tempGrade;
                App.Subjects = tempSubject;

                tempGrade = new List<Grade>();
                tempSubject = new Dictionary<String, Subject>();

                return marksPage;
            }
            catch { return "failed"; }
        }

        public async Task<Boolean> refreshMarks()
        {
            tempGrade.Clear();
            tempSubject.Clear();
            if (!globalRefresh)
                if (!await LoginAsync())
                    return false;
            
            try
            {
                String marksPage = await getMarksPageAsync();
                extratMarks(marksPage);

                //done checking for errors! 
                //let's save and notify new data
                if (App.Settings.notifyMarks)
                {
                    List<Grade> list3 = tempGrade.Except(App.Grades, new GradesComparer()).ToList();

                    if(list3.Count < 6)
                    {
                        for (int i = 0; i < list3.Count(); i++)
                        {
                            if (Device.RuntimePlatform == Device.Android)
                                DependencyService.Get<INotifyAndroid>().NotifyMark(list3[i], -i);
                            else
                                DependencyService.Get<INotifyiOS>().NotifyMark(list3[i]);
                        }  
                    }
                }


                App.Grades = tempGrade;
                App.Subjects = tempSubject;
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Xamarin.Forms.Application.Current.Properties["grades"] = JsonConvert.SerializeObject(App.Grades, Formatting.Indented, jsonSettings);
                return true;
            }
            catch { return false; }

        }


        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<string> getMarksPageAsync()
        {
            String pageSource = await Utility.GetPageAsync(User.school.marksUrl);
            if (pageSource.Contains("Warning") || pageSource.Contains("Fatal error"))
                throw new System.InvalidOperationException("Wrong Page");
            
            return pageSource;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratMarks-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public void extratMarks(String html)
        {
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

}
