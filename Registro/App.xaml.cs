using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private static List<Grade> grades = new List<Grade>();
        private static Dictionary<String, Subject> subjects = new Dictionary<string, Subject>();

        internal static List<Grade> Grades { get => grades; set => grades = value; }
        internal static Dictionary<string, Subject> Subjects { get => subjects; set => subjects = value; }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            // Handle when your app starts

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
    }
}
