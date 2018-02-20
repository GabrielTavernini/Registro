using System;
using System.Collections.Generic;
using System.Linq;
using XFShapeView.iOS;
using Registro;

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
            ShapeRenderer.Init();

            try{
                LoadApplication(new App()); 
            }catch(Exception e){
                System.Diagnostics.Debug.WriteLine(e);
            }


            return base.FinishedLaunching(app, options);
        }
    }
}
