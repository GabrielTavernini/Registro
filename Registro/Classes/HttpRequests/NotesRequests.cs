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
            try
            {
                String notesPage = await getNotesPageAsync();

                extratNotesIndiv(notesPage);
                extratNotesClass(notesPage);

                App.Notes = tempNotes;
                tempNotes = new List<Note>();

                return notesPage;
            }
            catch { return "failed"; }
        }

        public async Task<Boolean> refreshNotes()
        {
            tempNotes.Clear();
            if(!globalRefresh)
                if (!await LoginAsync())
                    return false;



            try
            {
                String Page = await getNotesPageAsync();
                extratNotesIndiv(Page);
                extratNotesClass(Page);

                //done checking for errors! 
                //let's save and notify new data
                if (App.Settings.notifyNotes)
                {
                    List<Note> list3 = tempNotes.Except(App.Notes, new NotesComparer()).ToList();

                    if(list3.Count < 6)
                    {
                        for (int i = 0; i < list3.Count(); i++)
                        {
                            if (Device.RuntimePlatform == Device.Android)
                                DependencyService.Get<INotifyAndroid>().NotifyNotes(list3[i], i + 9999);//9999 offset from others notifications
                            else
                                DependencyService.Get<INotifyiOS>().NotifyNotes(list3[i]);
                        }   
                    }
                }


                App.Notes = tempNotes;
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                Xamarin.Forms.Application.Current.Properties["notes"] = JsonConvert.SerializeObject(App.Notes, Formatting.Indented, jsonSettings);
                return true;
            }
            catch { return false; }
            

        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<string> getNotesPageAsync()
        {
            String pageSource = await Utility.GetPageAsync(User.school.noteUrl);
            if (pageSource.Contains("Warning") || pageSource.Contains("Fatal error"))
                throw new System.InvalidOperationException("Wrong Page");

            return pageSource;

        }

        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratMarks-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public void extratNotesIndiv(String html)
        {
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