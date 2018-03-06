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
    public class NotesRequests : HttpRequest
    {
        private List<Note> tempNotes = new List<Note>();
        
        public async Task<String> extractAllNotes()
        {
            String notesPage = await getNotesPageAsync();

            extratNotesIndiv(notesPage);
            extratNotesClass(notesPage);
            System.Diagnostics.Debug.WriteLine(notesPage);

            App.Notes = tempNotes;
            tempNotes = new List<Note>();

            return notesPage;
        }

        public async Task<Boolean> refreshNotes()
        {
            tempNotes.Clear();
            if (!await LoginAsync())
                return false;

            String Page = await getNotesPageAsync();
            if (Page == "failed")
                return false;

            extratNotesIndiv(Page);
            extratNotesClass(Page);
            if (!tempNotes.Any())
                return false;
            
            //done checking for errors! 
            //let's save and notify new data
            if(App.Settings.notifyNotes)
            {
                List<Note> list3 = tempNotes.Except(App.Notes, new NotesComparer()).ToList();

                for (int i = 0; i < list3.Count(); i++)
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotifyAndroid>().NotifyNotes(list3[i], i + 9999);//9999 offset from others notifications
                }  
            }

            
            App.Notes = tempNotes;
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            Xamarin.Forms.Application.Current.Properties["notes"] = JsonConvert.SerializeObject(App.Notes, Formatting.Indented, jsonSettings);
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<string> getNotesPageAsync()
        {
            try
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
            catch { return "failed"; }

        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratMarks-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public void extratNotesIndiv(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Note currentNote = new Note();

            for (int i = 3; ; i++)
            {
                Supremes.Nodes.Element table = doc.Select("body > div.contenuto > table:nth-child(8) > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");

                int Column = 1;
                foreach (Supremes.Nodes.Element inputElement in inputElements)
                {
                    if (Column == 1)
                    {
                        currentNote = new Note(inputElement.Text, "", "", "");
                        tempNotes.Add(currentNote);
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


        public void extratNotesClass(String html)
        {
            System.Diagnostics.Debug.WriteLine(html);
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Note currentNote = new Note();

            for (int i = 3; ; i++)
            {
                Supremes.Nodes.Element table = doc.Select("body > div.contenuto > table:nth-child(10) > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");

                int Column = 1;
                foreach (Supremes.Nodes.Element inputElement in inputElements)
                {
                    if (Column == 1)
                    {
                        currentNote = new Note(inputElement.Text, "", "", "");
                        tempNotes.Add(currentNote);
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

    public class NotesComparer : IEqualityComparer<Note>
    {
        public int GetHashCode(Note co)
        {
            if (co == null)
            {
                return 0;
            }
            String s = co.Text + co.Nome + co.Measures + co.date;
            return s.GetHashCode();
        }

        public bool Equals(Note x, Note y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;



            if (x.Nome == y.Nome
                && x.Measures == y.Measures
                && x.Text == y.Text
                && x.date == y.date)
                return true;
            else
                return false;
        }
    }
}