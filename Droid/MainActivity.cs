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
using Android.Gms.Gcm;
using Firebase.JobDispatcher;

namespace Registro.Droid
{
    [Activity(Label = "Registro.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme.Base", LaunchMode = LaunchMode.SingleTop,
              ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        internal static bool IsForeground { get; private set; } = true;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            IsForeground = true;
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
            IsForeground = false;
			StartBackgroundDataRefreshService();
        }

		protected override void OnResume()
		{
            base.OnResume();
            IsForeground = true;
		}

		protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            App.firstPage = intent.Action;//intent.GetStringExtra("page");
        }

		private void StartBackgroundDataRefreshService()
        {
			System.Diagnostics.Debug.WriteLine("StartBackgroundDataRefreshService");
            
			IDriver driver = new GooglePlayDriver(this);
            FirebaseJobDispatcher dispatcher = new FirebaseJobDispatcher(driver);

			JobTrigger myTrigger = Firebase.JobDispatcher.Trigger.ExecutionWindow(15, 30);
			Job myJob = dispatcher.NewJobBuilder()
                      .SetService<RefreshJob>("refresh-job-tag")
                      .SetConstraints(Firebase.JobDispatcher.Constraint.OnAnyNetwork)
			          .SetRecurring(true)
                      .SetTrigger(myTrigger)
                      .SetLifetime(Lifetime.Forever)
                      .Build();

            // This method will not throw an exception; an integer result value is returned
            int scheduleResult = dispatcher.Schedule(myJob);
			System.Diagnostics.Debug.WriteLine(scheduleResult);
        }
    }
}