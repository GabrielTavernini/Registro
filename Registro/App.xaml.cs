using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Registro.Classes;
using Registro.Classes.JsonRequest;
using Registro.Models;
using Registro.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Registro.Controls.Notifications;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Registro
{
    public partial class App : Application
    {
        private static Dictionary<String, School> schools = new Dictionary<String, School>();
        internal static Dictionary<string, School> Schools { get => schools; set => schools = value; }


        public static string firstPage = "";
        public static uint AnimationSpeed = 75;
        public static int DelaySpeed = 150;
        public static DateTime lastRefresh = new DateTime(0);
        public static Boolean globalRefresh { get; set; } = true;
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        //public static DateTime periodChange { get; set; } = new DateTime();
        //public static Boolean customPeriodChange { get; set; } = false;
        public static Boolean isDebugMode = false;

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
            #if DEBUG
                isDebugMode = true;
            #endif

            
            if(!isDebugMode)
                AppCenter.Start("android=09372489-f33f-4fcc-a58f-9c1a46d130c9;" +
                                "ios=ea6a4d0a-cf70-4bc8-a309-4a85ad0422db;",
                                typeof(Analytics), typeof(Crashes));

            NavigationPage navigationPage;
            //Search for login data
            if (Application.Current.Properties.ContainsKey("username") &&
                Application.Current.Properties.ContainsKey("password") &&
                Application.Current.Properties.ContainsKey("school") && 
                Application.Current.Properties.ContainsKey("schoolurl")) 
            {
                DeserializeObjects();

                School school = new School(Application.Current.Properties["schoolurl"] as string, Application.Current.Properties["school"] as string);
                string username = Application.Current.Properties["username"] as string;
                string password = Application.Current.Properties["password"] as string;

                User user = new User(username, password, school);


                if(username != null && password != null && school.loginUrl != null)
                {
                    JsonRequest.user = user;
                    navigationPage = new NavigationPage(new HomePage());
                }
                else
                {
                    //periodChange = GetPeriodChange();
                    navigationPage = new NavigationPage(new FirstPage());//new HomePage());  
                }
            }
            else
            {
                //periodChange = GetPeriodChange();
                navigationPage = new NavigationPage(new FirstPage());//new HomePage());
            }

            navigationPage.Popped += (sender, e) => { firstPage = null; };
            MainPage = navigationPage;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

		protected override void OnResume()
		{
            //Reset Badge on iOS
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                DependencyService.Get<INotifyiOS>().ResetBadge();


            //Search for login data
            if (Application.Current.Properties.ContainsKey("username") &&
                Application.Current.Properties.ContainsKey("password") &&
                Application.Current.Properties.ContainsKey("school") &&
                Application.Current.Properties.ContainsKey("schoolurl"))
            {
                if (firstPage != "" && firstPage != null)
                    MainPage = new NavigationPage(new HomePage());
            }
        }

        public void DeserializeObjects()
        {
            if(Application.Current.Properties.ContainsKey("settings"))
            {
                String str = Application.Current.Properties["settings"] as String;
                settings = JsonConvert.DeserializeObject<Settings>(str); 
            }

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

            if (Application.Current.Properties.ContainsKey("lateentries"))
            {
                String str = Application.Current.Properties["lateentries"] as String;
                lateEntries = JsonConvert.DeserializeObject<List<LateEntry>>(str);
            }

            if (Application.Current.Properties.ContainsKey("earlyexits"))
            {
                String str = Application.Current.Properties["earlyexits"] as String;
                earlyExits = JsonConvert.DeserializeObject<List<EarlyExit>>(str);
            }
        }

        static public void SerializeObjects()
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };

            Application.Current.Properties["grades"] = JsonConvert.SerializeObject(grades, Formatting.Indented, jsonSettings);
            Application.Current.Properties["arguments"] = JsonConvert.SerializeObject(arguments, Formatting.Indented, jsonSettings);
            Application.Current.Properties["notes"] = JsonConvert.SerializeObject(notes, Formatting.Indented, jsonSettings);
            Application.Current.Properties["absences"] = JsonConvert.SerializeObject(absences, Formatting.Indented, jsonSettings);
            Application.Current.Properties["lateentries"] = JsonConvert.SerializeObject(lateEntries, Formatting.Indented, jsonSettings);
            Application.Current.Properties["earlyexits"] = JsonConvert.SerializeObject(earlyExits, Formatting.Indented, jsonSettings);
            Application.Current.Properties["settings"] = JsonConvert.SerializeObject(settings, Formatting.Indented, jsonSettings);

        }
    }
}
