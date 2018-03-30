using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Registro.Classes.HttpRequests;
using Xamarin.Forms;
using static Registro.Controls.Notifications;

namespace Registro.Classes.JsonRequest
{
    public class JsonRequest
    {
        static public User user;
        static private String json;
        static private JObject dati;
        static public DateTime lastRequest;

        static public async Task<bool> JsonLogin()
        {
            if (user == null)
                return false;

            if ((DateTime.Now - lastRequest).Minutes < 1)
            {
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().DisplayToast("Già aggiornato");
                else
                    DependencyService.Get<INotifyiOS>().ShowToast("Già aggiornato");
                return false;
            }

            try
            {
                String QueryLogin = user.school.baseUrl + "/lsapp/jsonlogin.php?utente=" + user.username +
                                    "&password=" + Utility.hex_md5(user.password) + "&suffisso=" + user.school.suffisso + "&versione=16";
                json = await Utility.GetPageAsync(QueryLogin);
                System.Diagnostics.Debug.WriteLine(json);  
                dati = JObject.Parse(json);
            }
            catch { return await controllaTempoBassoAsync(); }
                
            
            lastRequest = DateTime.Now;
            App.lastRefresh = lastRequest;

            App.Subjects = getMaterieFromJson();
            List<Grade> voti = getVotiFromJson();
            List<Note> note = getNoteFromJson();
            List<Absence> assenze = getAssenzeFromJson();
            List<LateEntry> ritardi = getRitardiFromJson();
            List<EarlyExit> uscite = getUsciteFromJson();
            List<Arguments> lezioni = getLezioniFromJson();

            controllaVoti(voti);
            controllaNote(note);
            controllaAssenze(assenze);
            controllaRitrdi(ritardi);
            controllaUscite(uscite);
            controllaLezioni(lezioni);

            App.Grades = voti;
            App.Notes = note;
            App.Absences = assenze;
            App.LateEntries = ritardi;
            App.EarlyExits = uscite;
            App.Arguments = lezioni;

            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<INotifyAndroid>().DisplayToast("Aggiornamento completato");
            else
                DependencyService.Get<INotifyiOS>().ShowToast("Aggiornamento completato"); 

            App.SerializeObjects();
            return true;
        }

        private static async Task<bool> controllaTempoBassoAsync()
        {
            if (json.Contains("Tempo basso"))
            {
                HttpRequest.User = user;
                if (!await MarksRequests.RefreshAsync())
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotifyAndroid>().DisplayToast("Aggiornamento non riuscito");
                    else
                        DependencyService.Get<INotifyiOS>().ShowToast("Aggiornamento non riuscito");

                    return false;
                }

                lastRequest = DateTime.Now;
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().DisplayToast("Aggiornamento completato");
                else
                    DependencyService.Get<INotifyiOS>().ShowToast("Aggiornamento completato");

                return true;


            }

            return false;/*
            if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().DisplayToast("Già aggiornato");
            return false;*/
        }

        private static List<Grade> getVotiFromJson()
        {
            List<Grade> voti = new List<Grade>();
            JArray tipo, data, voto, giudizio, denominazione;

            try
            {
                tipo = (JArray) dati["tipo"];
                data = (JArray)dati["date"];
                voto = (JArray)dati["voto"];
                giudizio = (JArray)dati["giudizio"];
                denominazione = (JArray)dati["denominazione"];

                for (int i = 0; i < voto.Count; i++)
                {
                    String t = tipo[i].ToString();

                    switch (t)
                    {
                        case "O":
                            t = "Orale";
                            break;
                        case "P":
                            t = "Pratico";
                            break;
                        case "S":
                            t = "Scritto";
                            break;
                    }

                    String votoString = Utility.votoToString(float.Parse((string)voto[i], CultureInfo.InvariantCulture));
                    Subject s = Subject.getSubjectByString((string)denominazione[i]);
                    Grade g = new Grade((string)data[i].ToString(), t, votoString, (string)giudizio[i], Subject.getSubjectByString((string)denominazione[i]));
                    s.grades.Add(g);
                    voti.Add(g);
                }
            }
            catch (JsonException e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            foreach(Grade g in voti)
                System.Diagnostics.Debug.WriteLine(g.subject.name);

            return voti;
        }

        private static List<Absence> getAssenzeFromJson()
        {
            List<Absence> assenze = new List<Absence>();

            JArray date, giustifiche;

            try
            {
                date = (JArray)dati["dateass"];
                giustifiche = (JArray)dati["giustass"];

                Boolean giustificata;
                for (int i = 0; i < date.Count; i++)
                {
                    giustificata = !(giustifiche[i].ToString().Equals("0"));

                    Absence assenza = new Absence("", (string)date[i], giustificata);
                    assenze.Add(assenza);
                }
            }
            catch (JsonException e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            return assenze;
        }

        private static List<LateEntry> getRitardiFromJson()
        {
            List<LateEntry> ritardi = new List<LateEntry>();

            JArray date, giustifiche, n_ore, ora_ent;

            try
            {
                date = (JArray)dati["daterit"];
                giustifiche = (JArray)dati["giustr"];
                n_ore = (JArray)dati["numore"];
                ora_ent = (JArray)dati["oraent"];

                Boolean giustificata;
                for (int i = 0; i < date.Count; i++)
                {
                    giustificata = !giustifiche[i].Equals("0");

                    LateEntry ritardo = new LateEntry((string)date[i], (string)ora_ent[i], giustificata);
                    ritardi.Add(ritardo);
                }
            }
            catch (JsonException e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            return ritardi;
        }

        private static List<EarlyExit> getUsciteFromJson()
        {
            List<EarlyExit> uscite = new List<EarlyExit>();

            JArray date, ora_usc;

            try
            {
                date = (JArray)dati["dateusc"];
                ora_usc = (JArray)dati["oraus"];

                for (int i = 0; i < date.Count; i++)
                {
                    EarlyExit uscita = new EarlyExit((string)date[i], (string)ora_usc[i]);
                    uscite.Add(uscita);
                }
            }
            catch (JsonException e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            return uscite;
        }

        private static List<Note> getNoteFromJson()
        {
            List<Note> note = new List<Note>();
            JArray date, cognomi, nomi, descrizioni;

            try
            {
                date = (JArray)dati["data"];
                cognomi = (JArray)dati["cognomedoc"];
                nomi = (JArray)dati["nomedoc"];
                descrizioni = (JArray)dati["notealunno"];

                for (int i = 0; i < descrizioni.Count; i++)
                {
                    Note nota = new Note((string)cognomi[i] + nomi[i], (string)descrizioni[i], "", (string)date[i]);
                    note.Add(nota);
                }

                date = (JArray)dati["datac"];
                cognomi = (JArray)dati["cognomedc"];
                nomi = (JArray)dati["nomedc"];
                descrizioni = (JArray)dati["noteclasse"];

                for (int i = 0; i < descrizioni.Count; i++)
                {
                    Note nota = new Note((string)cognomi[i] + nomi[i], (string)descrizioni[i], "", (string)date[i]);
                    note.Add(nota);
                }

            }
            catch (JsonException e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            return note;
        }

        private static List<Arguments> getLezioniFromJson()
        {
            List<Arguments> lezioni = new List<Arguments>();

            JArray materie, argomenti, date, attivita;

            try
            {
                materie = (JArray)dati["matelez"];
                argomenti = (JArray)dati["argolez"];
                attivita = (JArray)dati["attilez"];
                date = (JArray)dati["datelez"];

                for (int i = 0; i < materie.Count; i++)
                {
                    Arguments lezione = new Arguments((string)argomenti[i], (string)attivita[i], (string)date[i], (string)materie[i]);
                    lezioni.Add(lezione);
                }
            }
            catch (JsonException e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            return lezioni;
        }

        private static Dictionary<string, Subject> getMaterieFromJson()
        {
            Dictionary<string, Subject> subjects = new Dictionary<string, Subject>();
            try
            {
                JArray materie = (JArray)dati["denominazione"];

                for (int i = 0; i < materie.Count; i++)
                {
                    if(!subjects.ContainsKey((string)materie[i]))
                        subjects.Add((string)materie[i], new Subject((string)materie[i]));
                }
            }
            catch (JsonException e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            return subjects;
        }
    
        private static void controllaVoti(List<Grade> voti)
        {
            if (App.Settings.notifyMarks)
            {
                List<Grade> list3 = voti.Except(App.Grades, new GradesComparer()).ToList();

                if (list3.Count < 6)
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
        }

        private static void controllaAssenze(List<Absence> assenze)
        {
            if (App.Settings.notifyAbsences)
            {
                List<Absence> list3 = assenze.Except(App.Absences, new AbsencesComparer()).ToList();
                for (int i = 0; i < list3.Count(); i++)
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotifyAndroid>().NotifyAbsence(list3[i], i - 9999); //-9999 offset from others notifications
                    else
                        DependencyService.Get<INotifyiOS>().NotifyAbsence(list3[i]);
                }
            }
        }

        private static void controllaRitrdi(List<LateEntry> ritardi)
        {
            if (App.Settings.notifyAbsences)
            {
                List<LateEntry> list3 = ritardi.Except(App.LateEntries, new LateEntriesComparer()).ToList();
                for (int i = 0; i < list3.Count(); i++)
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotifyAndroid>().NotifyLateEntry(list3[i], i - 9999); //-9999 offset from others notifications
                    else
                        DependencyService.Get<INotifyiOS>().NotifyLateEntry(list3[i]);
                }
            }
        }

        private static void controllaUscite(List<EarlyExit> uscite)
        {
            if (App.Settings.notifyAbsences)
            {
                List<EarlyExit> list3 = uscite.Except(App.EarlyExits, new EarlyExitsComparer()).ToList();
                for (int i = 0; i < list3.Count(); i++)
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotifyAndroid>().NotifyEarlyExit(list3[i], i - 9999); //-9999 offset from others notifications
                    else
                        DependencyService.Get<INotifyiOS>().NotifyEarlyExit(list3[i]);
                }
            }
        }

        private static void controllaNote(List<Note> note)
        {
            if (App.Settings.notifyNotes)
            {
                List<Note> list3 = note.Except(App.Notes, new NotesComparer()).ToList();

                if (list3.Count < 6)
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
        }

        private static void controllaLezioni(List<Arguments> argomenti)
        {
            if (App.Settings.notifyArguments)
            {
                List<Arguments> list3 = argomenti.Except(App.Arguments, new ArgumentsComparer()).ToList();

                if (list3.Count < 6)
                {
                    for (int i = 0; i < list3.Count(); i++)
                    {
                        if (Device.RuntimePlatform == Device.Android)
                            DependencyService.Get<INotifyAndroid>().NotifyArguments(list3[i], i);
                        else
                            DependencyService.Get<INotifyiOS>().NotifyArguments(list3[i]);
                    }
                }
            }
        }
    }
}
