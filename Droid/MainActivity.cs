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
using Android.Gms.Common;
using Registro.Pages;
using Xamarin.Forms;
using static Android.Content.Res.Resources;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Registro.Classes.HttpRequests;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Registro.Droid
{
    [Activity(Label = "Registro.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme.Base", LaunchMode = LaunchMode.SingleTop,
              ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        internal static bool IsForeground { get; private set; } = true;
        private static AlarmManager manager;
        private static PendingIntent pendingIntent;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            IsForeground = true;
            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density); // real pixels
            App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density); // real pixels

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            AppCenter.Start("09372489-f33f-4fcc-a58f-9c1a46d130c9", typeof(Analytics), typeof(Crashes));
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            try
            {
                LoadApplication(new App());  
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                Crashes.TrackError(e);
            }

            // IMPORTANT: Initialize XFGloss AFTER calling LoadApplication on the Android platform
            XFGloss.Droid.Library.Init(this, savedInstanceState);
        }

        protected override void OnPause()
        {
            base.OnPause();
            IsForeground = false;
            if (manager == null)
                StartAlarm(this);
        }

		protected override void OnResume()
		{
            base.OnResume();
            IsForeground = true;
		}

		protected override void OnDestroy()
        {
            base.OnDestroy();
            if (manager != null)
                manager.Cancel(pendingIntent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            App.firstPage = intent.Action;//intent.GetStringExtra("page");
        }

        internal static void StartAlarm(Context context)
        {
            manager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent myIntent;
            myIntent = new Intent(context, typeof(AlarmRefreshReceiver));
            pendingIntent = PendingIntent.GetBroadcast(context, 0, myIntent, 0);

            manager.SetInexactRepeating(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + 1000, 60 * 30 * 1000, pendingIntent);
        }

        internal static void StopAlarm()
        {
            if (manager != null)
                manager.Cancel(pendingIntent);
        }
    }
}