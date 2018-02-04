using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace Registro.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;

            // IMPORTANT: Initialize XFGloss AFTER calling LoadApplication on the Android platform
            XFGloss.iOS.Library.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
