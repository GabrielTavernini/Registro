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

namespace Registro
{
    public class SchoolRequests
    {
        public static async Task<Boolean> extractAllSchoolsAsync()
        {
            try
            {
                List<string> schoolsPages = await getSchoolsPageAsync();
                extractSchools(schoolsPages);
                return true;
            }
            catch { return false; }
        }


        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public static async Task<List<string>> getSchoolsPageAsync()
        {
            List<string> schoolsPages = new List<string>();
            for (int i = 0; i < App.schoolsUrls.Length; i++)
            {
                try
                {
                    HttpClient client = new HttpClient(new NativeMessageHandler());
                    using (client)
                    {
                        using (HttpResponseMessage response = await client.GetAsync(App.schoolsUrls[i]))
                        {
                            using (HttpContent content = response.Content)
                            {
                                var page = await content.ReadAsStringAsync();
                                schoolsPages.Add(page);
                            }
                        }
                    }
                }
                catch { }
            }

            return schoolsPages;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratMarks-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public static void extractSchools(List<string> htmls)
        {
            for (int x = 0; x < htmls.Count; x++)
            {
                try
                {
                    string html = htmls[x];
                    System.Diagnostics.Debug.WriteLine(html);
                    Document doc = Dcsoup.ParseBodyFragment(html, "");

                    School currentSchool = new School(false);

                    int Column = 1;
                    for (int i = 2; ; i++)
                    {
                        Supremes.Nodes.Element table = doc.Select("body > table > tbody > tr:nth-child(" + i + ")").First;

                        if (table == null) break;

                        Elements inputElements = table.GetElementsByTag("td");


                        foreach (Supremes.Nodes.Element inputElement in inputElements)
                        {
                            if (Column == 1)
                            {
                                currentSchool = new School(inputElement.Text, true);
                                Column++;
                            }
                            else if (Column == 2)
                            {
                                Column++;
                            }
                            else if (Column == 3)
                            {
                                System.Diagnostics.Debug.WriteLine(inputElement.Text);
                                currentSchool.setUrl(App.schoolsUrls[x] + inputElement.GetElementsByTag("a").Attr("href"));
                                Column = 1;
                            }
                        }
                    }
                }
                catch { }
            }

        }

    }
}