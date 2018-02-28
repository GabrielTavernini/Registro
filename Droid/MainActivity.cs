using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XFShapeView.Droid;
using Java.Lang;
using Android.App.Job;

namespace Registro.Droid
{
    [Activity(Label = "Registro.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density); // real pixels
            App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density); // real pixels

            StartAlarm(this);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // IMPORTANT: Initialize XFGloss AFTER calling LoadApplication on the Android platform
            XFGloss.Droid.Library.Init(this, savedInstanceState);
        }

        internal static void StartAlarm(Context context)
        {
            AlarmManager manager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent myIntent;
            PendingIntent pendingIntent;
            myIntent = new Intent(context, typeof(AlarmRefreshReceiver));
            pendingIntent = PendingIntent.GetBroadcast(context, 0, myIntent, 0);

            manager.SetRepeating(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + (3600 * 1000), 3600 * 1000, pendingIntent);
        }
    }
}
