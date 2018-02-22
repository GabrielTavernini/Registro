using System;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Supremes;
using Supremes.Nodes;

namespace Registro.Classes.HttpRequests
{
    public class AbsencesRequests : HttpRequest
    {
        static public async Task<String> extractAll()
        {
            String absencesPage = await getAbsencesPageAsync();

            //extratNotesIndiv(absencesPage);
            //extratNotesClass(absencesPage);
            System.Diagnostics.Debug.WriteLine(absencesPage);
            return absencesPage;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        static public async Task<string> getAbsencesPageAsync()
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

        static public void extratAbsences(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Absence currentAbsence = new Absence();

            Element table = doc.Select("body > div.contenuto > table:nth-child(6) > tbody > tr:nth-child(6)").First;

            if (table == null) return;

            Elements inputElements = table.GetElementsByTag("td");

            int Column = 1;
            foreach (Element inputElement in inputElements)
            {
                if (Column == 1)
                {
                    foreach(String s in inputElement.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                    {
                        currentAbsence = new Absence("Assenza", s, true);
                    }
                    Column++;
                }
                else if (Column == 2)
                {
                    foreach (String s in inputElement.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                    {
                        currentAbsence = new Absence("Ritardo", s, true);
                    }                    
                    Column++;
                }
                else if (Column == 3)
                {
                    foreach (String s in inputElement.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                    {
                        currentAbsence = new Absence("Uscita", s, true);
                    }                    
                    Column = 1;
                }

            }
        }
    }
}