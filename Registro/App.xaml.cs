using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Registro.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Registro
{
    public partial class App : Application
    {
        public static uint AnimationSpeed = 75;
        public static int DelaySpeed = 150;
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        public static DateTime periodChange { get; set; } = new DateTime();
        private static List<Grade> grades = new List<Grade>();
        private static Dictionary<String, Subject> subjects = new Dictionary<string, Subject>();
        private static List<Arguments> arguments = new List<Arguments>();
        private static List<Note> notes = new List<Note>();
        private static List<Absence> absences = new List<Absence>();


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
            // Handle when your app starts

            if (Application.Current.Properties.ContainsKey("periodchange"))
                periodChange = DateTime.ParseExact(Application.Current.Properties["periodchange"] as string, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else
                periodChange = GetPeriodChange();

            DeserializeObjects();

            if (Application.Current.Properties.ContainsKey("username") &&
                Application.Current.Properties.ContainsKey("password") &&
                Application.Current.Properties.ContainsKey("school"))
            {

                School school = new School(
                     "https://www.lampschool.it/hosting_trentino_17_18/login/login.php?suffisso=scuola_27",
                     "Dro"
                 );

                string username = Application.Current.Properties["username"] as string;
                string password = Application.Current.Properties["password"] as string;

                User user = new User(username, password, school);

                MainPage = new NavigationPage(new HomePage(user));//new MainPage(user));//new HomePage(user));
            }
            else
            {
                MainPage = new NavigationPage(new FirstPage());//new HomePage());
            }

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private DateTime GetPeriodChange()
        {
            if(DateTime.Now.Month > 7)
            {
                return new DateTime(DateTime.Now.Year + 1, 1, 31);
            }
            else
            {
                return new DateTime(DateTime.Now.Year, 1, 31);
            }
        }


        static public void DeserializeObjects()
        {

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
