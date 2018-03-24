using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Registro.Classes;
using Registro.Models;
using Registro.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Registro
{
    public partial class App : Application
    {
        public static string[] schoolsUrls = {
            "https://www.lampschool.it/hosting_rete_17_18/",
            "https://www.lampschool.it/hosting_trentino_17_18/",
            "http://www.salesianisb.net/registro_2017_2018/",
            "http://www.iccasaleone.gov.it/registroinf/",
            "http://www.icviadantevoghera.gov.it/res2017/",
            "http://www.liceibelluno.gov.it/registro/",
            "https://www.icscaprinoveronese.it/registroonline/201718/secondaria201718_1/",
            "https://www.icscaprinoveronese.it/registroonline/201718/primaria201718/",
            "http://www.buonconsigliotorino.it/Registro_201718/",
            "http://www.comprensivoviguzzolo.gov.it/rep2017/",
            "http://www.istitutomnicomprensivotrivento.gov.it/lampschool-code_2017/",
            "http://www.istitutopadrepioispica.it/regimedia2017/",
            "https://istitutocomprensivocortina.it/lampschool_2017_18/",
            "http://www.icsanguinetto.gov.it/registrosec/",
            "https://www.comprensivovr11.it/registro2017-2018/",
            "https://www.isdimaggio.it/lampschool/",
            "http://www.primocircolosestu.gov.it/Lampschool_inf_1718/",
            "http://www.primocircolosestu.gov.it/Lampschool_2017_18/",
            "http://www.icsmalcesine.gov.it/registro_elettronico/ls_17_18/",
            "http://www.icpapanice.gov.it/registrosecondaria2017-18/",
            "http://www.icpapanice.gov.it/registroprimaria2017-18/",
            "http://www.istitutogiberti.it/registro17_18/",
            "http://www.alberghierorosmini.it/Registro/AS_2017_18/",
            "https://www.piccolacasa.org/registro_2017_18/",
            "https://www.icripalimosani.gov.it/registro_2017-2018/",
            "http://www.istitutocomprensivolagonegro.it/lamp17_18/",
            "https://www.icsoave.gov.it/registro/",
            "http://www.csdalbenga.it/registro/",
            "http://www.icmatteottiaprilia.gov.it/registro/",
            "http://www.icmontecchiaronca.gov.it/registro_2017_2018/",
            "http://www.associazionegiuseppeverdimilazzo.it/lampschool/registroelettronico/2017-2018/",
            "http://www.scuolapitagora.com/registro-elettronico/",
            "http://212.237.17.99/registro/",
            "https://www.ic5verona.gov.it/registro17-18/",
            "http://www.istitutodivinaprovvidenza.it/reg2017-18/",
            "http://www.icsgi.com/registro_2017_2018/",
            "http://scuolaagazzi.it/registroelettronico/primaria/as_2017-2018/",
            "http://lnx.istruzionemonteforte.gov.it/zanella/",
            "http://www.iscolevi.it/registro17-18p/",
            "http://www.ciofascuola.it/registro_2017_2018/"};
        private static Dictionary<String, School> schools = new Dictionary<String, School>();
        internal static Dictionary<string, School> Schools { get => schools; set => schools = value; }


        public static string firstPage = "";
        public static bool notify = false;
        public static uint AnimationSpeed = 75;
        public static int DelaySpeed = 150;
        public static DateTime lastRefresh = new DateTime(0);
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        public static DateTime periodChange { get; set; } = new DateTime();


        private static List<Grade> grades = new List<Grade>();
        private static Dictionary<String, Subject> subjects = new Dictionary<string, Subject>();
        private static List<Arguments> arguments = new List<Arguments>();
        private static List<Note> notes = new List<Note>();
        private static List<Absence> absences = new List<Absence>();
        private static Settings settings = new Settings();
        private static List<LateEntry> lateEntries = new List<LateEntry>();
        private static List<EarlyExit> earlyExits = new List<EarlyExit>();

        internal static List<EarlyExit> EarlyExits { get => earlyExits; set => earlyExits = value; }
        internal static List<LateEntry> LateEntries { get => lateEntries; set => lateEntries = value; }
        internal static Settings Settings { get => settings; set => settings = value; }
        internal static List<Absence> Absences { get => absences; set => absences = value; }
        internal static List<Note> Notes { get => notes; set => notes = value; }
        internal static List<Arguments> Arguments { get => arguments; set => arguments = value; }
        internal static List<Grade> Grades { get => grades; set => grades = value; }
        internal static Dictionary<string, Subject> Subjects { get => subjects; set => subjects = value; }



        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            //Search for login data
            if (Application.Current.Properties.ContainsKey("username") &&
                Application.Current.Properties.ContainsKey("password") &&
                Application.Current.Properties.ContainsKey("school") && 
                Application.Current.Properties.ContainsKey("schoolurl")) 
            {
                //Deserialize object lists
                DeserializeObjects();
                System.Diagnostics.Debug.WriteLine("Count App: {0}", App.Grades.Count());

                School school = new School(Application.Current.Properties["schoolurl"] as string, Application.Current.Properties["school"] as string);
                System.Diagnostics.Debug.WriteLine(school.absencesUrl);
                string username = Application.Current.Properties["username"] as string;
                string password = Application.Current.Properties["password"] as string;

                User user = new User(username, password, school);

                if(username != null && password != null && school.loginUrl != null)
                {
                    HomePage.isFirstTime = true;
                    MainPage = new NavigationPage(new HomePage(user));
                }
                else
                {
                    periodChange = GetPeriodChange();
                    MainPage = new NavigationPage(new FirstPage());//new HomePage());  
                }
            }
            else
            {
                periodChange = GetPeriodChange();
                MainPage = new NavigationPage(new FirstPage());//new HomePage());
            }

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            HomePage.isFirstTime = true;
            notify = false;
        }

		protected override void OnResume()
		{
            // Handle when your app resumes
            //Search for login data
            if (Application.Current.Properties.ContainsKey("username") &&
                Application.Current.Properties.ContainsKey("password") &&
                Application.Current.Properties.ContainsKey("school") &&
                Application.Current.Properties.ContainsKey("schoolurl"))
            {
                //Deserialize object lists
                if (firstPage != "" && firstPage != null)
                    MainPage = new NavigationPage(new HomePage());
            }
        }

        private DateTime GetPeriodChange()
        {
            if (DateTime.Now.Month > 7)
                return new DateTime(DateTime.Now.Year + 1, 1, 31);
            else
                return new DateTime(DateTime.Now.Year, 1, 31);
        }

        public void DeserializeObjects()
        {
            if(Application.Current.Properties.ContainsKey("settings"))
            {
                String str = Application.Current.Properties["settings"] as String;
                settings = JsonConvert.DeserializeObject<Settings>(str); 
            }
            
            if (Application.Current.Properties.ContainsKey("periodchange"))
            {
                String str = Application.Current.Properties["periodchange"] as String;
                periodChange = JsonConvert.DeserializeObject<DateTime>(str);
            }
            else
                periodChange = GetPeriodChange();


            if (Application.Current.Properties.ContainsKey("grades"))
            {
                String str = Application.Current.Properties["grades"] as String;
                grades = JsonConvert.DeserializeObject<List<Grade>>(str);

                foreach(Grade g in grades)
                {
                    if(!subjects.ContainsKey(g.subject.name))
                    {
                        subjects.Add(g.subject.name, g.subject); 
                    }
                }
            }

            if (Application.Current.Properties.ContainsKey("arguments"))
            {
                String str = Application.Current.Properties["arguments"] as String;
                arguments = JsonConvert.DeserializeObject<List<Arguments>>(str);
                    
            }

            if (Application.Current.Properties.ContainsKey("notes"))
            {
                String str = Application.Current.Properties["notes"] as String;
                notes = JsonConvert.DeserializeObject<List<Note>>(str);
            }

            if (Application.Current.Properties.ContainsKey("absences"))
            {
                String str = Application.Current.Properties["absences"] as String;
                absences = JsonConvert.DeserializeObject<List<Absence>>(str);
            }
        }

        static public void SerializeObjects()
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };

            Application.Current.Properties["grades"] = JsonConvert.SerializeObject(grades, Formatting.Indented, jsonSettings);
            Application.Current.Properties["arguments"] = JsonConvert.SerializeObject(arguments, Formatting.Indented, jsonSettings);
            Application.Current.Properties["notes"] = JsonConvert.SerializeObject(notes, Formatting.Indented, jsonSettings);
            Application.Current.Properties["absences"] = JsonConvert.SerializeObject(absences, Formatting.Indented, jsonSettings);
        }
    }
}
