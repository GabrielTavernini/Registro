using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Registro.Classes.HttpRequests;
using Registro.Pages;
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
        static private Boolean retrying;

        static public async Task<bool> JsonLogin()
        {
            if (user == null)
                return false;

            if ((DateTime.Now - lastRequest).Minutes < 1 && !App.isDebugMode)
            {
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().DisplayToast("Già aggiornato");
                else
                    DependencyService.Get<INotifyiOS>().ShowToast("Già aggiornato", 750);
                return false;
            }

            try
            {
                String QueryLogin = user.school.baseUrl + "/lsapp/jsonlogin.php?utente=" + user.username +
                                    "&password=" + Utility.hex_md5(user.password) + "&suffisso=" + user.school.suffisso;
                json = await Utility.GetPageAsync(QueryLogin);

                Debug.WriteLine("\nQueryLogin:\n" + QueryLogin);
                Debug.WriteLine("\nJson:\n" + json);
                 dati = JObject.Parse(json);
            }
            catch (Exception e)
            {
                if (json == null || json == "")
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotifyAndroid>().DisplayToast("Aggiornamento non riuscito");
                    else
                        DependencyService.Get<INotifyiOS>().ShowToast("Aggiornamento non riuscito", 750);

                    return false;
                }
                else if (json.Contains("Tempo basso"))
                {
                    await Task.Delay(1000);
                    if (!retrying)
                    {
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            DependencyService.Get<INotifyAndroid>().DisplayToast("Aspetta...");
                            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
                            {
                                DependencyService.Get<INotifyAndroid>().DisplayToast("Ci vorrà più del solito");
                                return false;
                            });
                        }
                        else
                        {
                            DependencyService.Get<INotifyiOS>().ShowToast("Aspetta...", 1000);
                            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
                            {
                                DependencyService.Get<INotifyiOS>().ShowToast("Ci vorrà più del solito", 1500);
                                return false;
                            });
                        }
                    }

                    retrying = true;
                    await JsonLogin();
                }
                else if (json.Contains("Alunno non trovato!"))
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotifyAndroid>().DisplayToast("Alunno non trovato!");
                    else
                        DependencyService.Get<INotifyiOS>().ShowToast("Alunno non trovato!", 750);

                    return false;
                }
                else
                {
                    int start = json.IndexOf('{');
                    int end = json.LastIndexOf('}');
                    if(start >= 0 && end > 0)
                    {
                        String newJson = json.Substring(start, (end - start) + 1);
                        if(newJson.Length > 2)
                        {
                            Debug.WriteLine("\nNewJson:\n" + newJson);
                            json = newJson;

                            try{ 
                                dati = JObject.Parse(json);
                                goto estraiTutti;
                            }
                            catch{ return false; }
                        }
                    }

                    var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                    Crashes.TrackError(e, properties);

                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<INotifyAndroid>().DisplayToast("Aggiornamento non riuscito");
                    else
                        DependencyService.Get<INotifyiOS>().ShowToast("Aggiornamento non riuscito", 750);

                    return false;
                }
            }

            //starting to extrack all the stuff
            estraiTutti:

            if (dati == null)
                return false;

            retrying = false;
            lastRequest = DateTime.Now;
            App.lastRefresh = lastRequest;

            //Extract
            App.Subjects = getMaterieFromJson();
            List<Grade> voti = getVotiFromJson();
            List<Note> note = getNoteFromJson();
            List<Absence> assenze = getAssenzeFromJson();
            List<LateEntry> ritardi = getRitardiFromJson();
            List<EarlyExit> uscite = getUsciteFromJson();
            List<Arguments> lezioni = getLezioniFromJson();

            //Check
            controllaVoti(voti);
            controllaNote(note);
            controllaAssenze(assenze);
            controllaRitrdi(ritardi);
            controllaUscite(uscite);
            controllaLezioni(lezioni);

            //Save
            App.Grades = voti;
            App.Notes = note;
            App.Absences = assenze;
            App.LateEntries = ritardi;
            App.EarlyExits = uscite;
            App.Arguments = lezioni;
            if(!App.Settings.customPeriodChange)
                App.Settings.periodChange = getFinePrimo();
            App.SerializeObjects();

            //Notify
            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<INotifyAndroid>().DisplayToast("Aggiornamento completato");
            else
                DependencyService.Get<INotifyiOS>().ShowToast("Aggiornamento completato", 750);
            
            return true;
        }


        private static List<Grade> getVotiFromJson()
        {
            List<Grade> voti = new List<Grade>();
            JArray tipo, data, voto, giudizio, denominazione;

            try
            {
                tipo = JArray.Parse(dati["tipo"].ToString());
                data = JArray.Parse(dati["date"].ToString());
                voto = JArray.Parse(dati["voto"].ToString());
                giudizio = JArray.Parse(dati["giudizio"].ToString());
                denominazione = JArray.Parse(dati["denominazione"].ToString());

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

                    if (voto[i].ToString() == "99")
                        return new List<Grade>();

                    String votoString = Utility.votoToString(float.Parse(voto[i].ToString(), CultureInfo.InvariantCulture));
                    Subject s = Subject.getSubjectByString(denominazione[i].ToString());
                    Grade g = new Grade(data[i].ToString(), t, votoString, giudizio[i].ToString(), Subject.getSubjectByString(denominazione[i].ToString()));
                    //s.grades.Add(g);
                    //voti.Add(g);
                    lock (s.grades)
                        s.grades.Add(g);
                    
                    voti.Add(g);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                Crashes.TrackError(e, properties);
            }

            foreach(Grade g in voti)
                Debug.WriteLine(g.subject.name);

            return voti;
        }

        private static List<Absence> getAssenzeFromJson()
        {
            List<Absence> assenze = new List<Absence>();

            JArray date, giustifiche;

            try
            {
                Debug.WriteLine("Giustass:\n" + dati["giustass"].ToString());
                if(dati["giustass"].ToString() == "")
                    return assenze;
                
                date = JArray.Parse(dati["dateass"].ToString());
                giustifiche = JArray.Parse(dati["giustass"].ToString());

                Boolean giustificata;
                for (int i = 0; i < date.Count; i++)
                {
                    giustificata = !(giustifiche[i].ToString().Equals("0"));

                    Absence assenza = new Absence("", date[i].ToString(), giustificata);
                    assenze.Add(assenza);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                Crashes.TrackError(e, properties);
            }

            return assenze;
        }

        private static List<LateEntry> getRitardiFromJson()
        {
            List<LateEntry> ritardi = new List<LateEntry>();

            JArray date, giustifiche, n_ore, ora_ent;

            try
            {
                date = JArray.Parse(dati["daterit"].ToString());
                giustifiche = JArray.Parse(dati["giustr"].ToString());
                n_ore = JArray.Parse(dati["numore"].ToString());
                ora_ent = JArray.Parse(dati["oraent"].ToString());

                Boolean giustificata;
                for (int i = 0; i < date.Count; i++)
                {
                    giustificata = !giustifiche[i].Equals("0");

                    LateEntry ritardo = new LateEntry(date[i].ToString(), ora_ent[i].ToString(), giustificata);
                    ritardi.Add(ritardo);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                Crashes.TrackError(e, properties);
            }

            return ritardi;
        }

        private static List<EarlyExit> getUsciteFromJson()
        {
            List<EarlyExit> uscite = new List<EarlyExit>();

            JArray date, ora_usc;

            try
            {
                date = JArray.Parse(dati["dateusc"].ToString());
                ora_usc = JArray.Parse(dati["oraus"].ToString());

                for (int i = 0; i < date.Count; i++)
                {
                    EarlyExit uscita = new EarlyExit(date[i].ToString(), ora_usc[i].ToString());
                    uscite.Add(uscita);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                Crashes.TrackError(e, properties);
            }

            return uscite;
        }

        private static List<Note> getNoteFromJson()
        {
            List<Note> note = new List<Note>();
            JArray date, cognomi, nomi, descrizioni;

            try
            {
                date = JArray.Parse(dati["data"].ToString());
                cognomi = JArray.Parse(dati["cognomedoc"].ToString());
                nomi = JArray.Parse(dati["nomedoc"].ToString());
                descrizioni = JArray.Parse(dati["notealunno"].ToString());

                for (int i = 0; i < descrizioni.Count; i++)
                {
                    Note nota = new Note(cognomi[i].ToString() + nomi[i].ToString().ToString(), descrizioni[i].ToString(), "", date[i].ToString());
                    note.Add(nota);
                }

                date = JArray.Parse(dati["datac"].ToString());
                cognomi = JArray.Parse(dati["cognomedc"].ToString());
                nomi = JArray.Parse(dati["nomedc"].ToString());
                descrizioni = JArray.Parse(dati["noteclasse"].ToString());

                for (int i = 0; i < descrizioni.Count; i++)
                {
                    Note nota = new Note(cognomi[i].ToString() + nomi[i].ToString(), descrizioni[i].ToString(), "", date[i].ToString());
                    note.Add(nota);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                Crashes.TrackError(e, properties);
            }

            return note;
        }

        private static List<Arguments> getLezioniFromJson()
        {
            List<Arguments> lezioni = new List<Arguments>();

            JArray materie, argomenti, date, attivita;

            try
            {
                materie = JArray.Parse(dati["matelez"].ToString());
                argomenti = JArray.Parse(dati["argolez"].ToString());
                attivita = JArray.Parse(dati["attilez"].ToString());
                date = JArray.Parse(dati["datelez"].ToString());

                for (int i = 0; i < materie.Count; i++)
                {
                    Arguments lezione = new Arguments(argomenti[i].ToString(), attivita[i].ToString(), date[i].ToString(), materie[i].ToString());
                    lezioni.Add(lezione);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                Crashes.TrackError(e, properties);
            }

            return lezioni;
        }

        private static Dictionary<string, Subject> getMaterieFromJson()
        {
            Dictionary<string, Subject> subjects = new Dictionary<string, Subject>();
            try
            {
                JArray materie = JArray.Parse(dati["denominazione"].ToString());

                for (int i = 0; i < materie.Count; i++)
                {
                    if(!subjects.ContainsKey(materie[i].ToString()))
                        subjects.Add(materie[i].ToString(), new Subject(materie[i].ToString()));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                Crashes.TrackError(e, properties);
            }

            return subjects;
        }
    
        public static DateTime getFinePrimo()
        {
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(dati["fineprimo"].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture);
            }
            catch (Exception e)
            {
                if (DateTime.Now.Month > 7)
                    date = new DateTime(DateTime.Now.Year + 1, 1, 31);
                else
                    date = new DateTime(DateTime.Now.Year, 1, 31);

                Debug.WriteLine(e.StackTrace);
                var properties = new Dictionary<string, string> {
                        { "School", user.school.loginUrl },
                        { "Suffisso", user.school.suffisso },
                        { "JsonStart", json },
                        { "JsonEnd", json.Substring(Math.Max(0, json.Length - 64)) }};

                Crashes.TrackError(e, properties);
            }

            return date;
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
