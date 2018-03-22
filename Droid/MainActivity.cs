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

namespace Registro.Droid
{
    [Activity(Label = "Registro.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme.Base", LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        private static AlarmManager manager;
        private static PendingIntent pendingIntent;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density); // real pixels
            App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density); // real pixels

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // IMPORTANT: Initialize XFGloss AFTER calling LoadApplication on the Android platform
            XFGloss.Droid.Library.Init(this, savedInstanceState);
        }

		protected override void OnPause()
        {
            base.OnPause();
            if (manager == null)
                StartAlarm(this);
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
            if(manager != null)
                manager.Cancel(pendingIntent);
        }
    }
}