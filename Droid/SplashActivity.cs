﻿
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using XamarinForms.Droid;

namespace Registro.Droid
{
    [Activity(MainLauncher = true, NoHistory = true, Icon = "@drawable/icon", Theme = "@style/LaunchTheme", 
              ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class LaunchActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            //ActionBar?.Hide();
        }

        protected override void OnResume()
        {
            base.OnResume();

            StartActivity(typeof(MainActivity));
        }
    }
}