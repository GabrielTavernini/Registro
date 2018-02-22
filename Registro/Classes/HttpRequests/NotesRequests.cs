using System;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Supremes;
using Supremes.Nodes;

namespace Registro.Classes.HttpRequests
{
    public class NotesRequests : HttpRequest
    {
        static public async Task<String> extractAll()
        {
            String notesPage = await getNotesPageAsync();

            extratNotesIndiv(notesPage);
            extratNotesClass(notesPage);
            System.Diagnostics.Debug.WriteLine(notesPage);
            return notesPage;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        static public async Task<string> getNotesPageAsync()
        {
            string pageSource;
            HttpRequestMessage getRequest = new HttpRequestMessage();
            getRequest.RequestUri = new Uri(User.school.noteUrl);
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

        static public void extratNotesIndiv(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Note currentNote = new Note();

            for (int i = 3; ; i++)
            {
                Element table = doc.Select("body > div.contenuto > table:nth-child(8) > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");

                int Column = 1;
                foreach (Element inputElement in inputElements)
                {
                    
                    if ("4".Equals(inputElement.Attr("colspan")))
                    {
                        return;
                    }
                    else if (Column == 1)
                    {
                        currentNote = new Note(inputElement.Text, "", "", "", true);
                        Column++;
                    }
                    else if (Column == 2)
                    {
                        currentNote.setDate(inputElement.Text);
                        Column++;
                    }
                    else if (Column == 3)
                    {
                        System.Diagnostics.Debug.WriteLine(inputElement.Text);
                        currentNote.Text = (inputElement.Text);
                        Column++;
                    }
                    else if (Column == 4)
                    {
                        currentNote.Measures = (inputElement.Text);
                        Column = 1;
                    }

                }
            }
        }


        static public void extratNotesClass(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Note currentNote = new Note();

            for (int i = 3; ; i++)
            {
                Element table = doc.Select("body > div.contenuto > table:nth-child(10) > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");

                int Column = 1;
                foreach (Element inputElement in inputElements)
                {

                    if ("4".Equals(inputElement.Attr("colspan")))
                    {
                        return;
                    }
                    else if (Column == 1)
                    {
                        currentNote = new Note(inputElement.Text, "", "", "", true);
                        Column++;
                    }
                    else if (Column == 2)
                    {
                        currentNote.setDate(inputElement.Text);
                        Column++;
                    }
                    else if (Column == 3)
                    {
                        System.Diagnostics.Debug.WriteLine(inputElement.Text);
                        currentNote.Text = (inputElement.Text);
                        Column++;
                    }
                    else if (Column == 4)
                    {
                        currentNote.Measures = (inputElement.Text);
                        Column = 1;
                    }

                }
            }
        }
    }
}