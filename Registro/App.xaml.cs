using System;
using System.Collections.Generic;
using Registro.Pages;
using Xamarin.Forms;

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
            //Lol
            MainPage = new NavigationPage(new Page4());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
