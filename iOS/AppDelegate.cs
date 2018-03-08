using System;
using System.Collections.Generic;
using System.Linq;
using XFShapeView.iOS;
using Registro;

using Foundation;
using UIKit;
using System.Threading.Tasks;

namespace Registro.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();

            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;

            //notifications stuff
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                );

                uiApplication.RegisterUserNotificationSettings(notificationSettings);
            }
            //reset app badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            //background fatch
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);




            // IMPORTANT: Initialize XFGloss AFTER calling LoadApplication on the Android platform
            XFGloss.iOS.Library.Init();
            ShapeRenderer.Init();

            try
            {
                LoadApplication(new App());
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }


            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            //UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(okayAlertController, true, null);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            Boolean success = true;
            Task.Run(async () => success = await HttpRequest.RefreshAsync());

            if (success)
                completionHandler(UIBackgroundFetchResult.NewData);
            else
                completionHandler(UIBackgroundFetchResult.Failed);
        }
    }
}







// check for a notification on start
/*if (options != null)
{
    // check for a local notification
    if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
    {
        var localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
        if (localNotification != null)
        {
            UIAlertController okayAlertController = UIAlertController.Create(localNotification.AlertAction, localNotification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            Window.RootViewController.PresentViewController(okayAlertController, true, null);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}*/

//called when the app is in forground and receive a notification
/*public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            //UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(okayAlertController, true, null);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

        }*/