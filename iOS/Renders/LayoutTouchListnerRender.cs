using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Foundation;
using Registro;
using Registro.Controls;
using Registro.iOS.Renders;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LayoutTouchListner), typeof(LayoutTouchListnerRender))]
namespace Registro.iOS.Renders
{
    [Preserve(AllMembers = true)]
    public class LayoutTouchListnerRender : ViewRenderer
    {

        LayoutTouchListner MainElement;
        private float difference;
        private float start;

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;
            start = (float)touch.LocationInView(this).Y;
            System.Diagnostics.Debug.WriteLine("Began");
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            UITouch touch = touches.AnyObject as UITouch;

            difference = (float)touch.LocationInView(this).Y - start;
            System.Diagnostics.Debug.WriteLine("--------------dif = " + difference);
            MainElement.DoTouchEvent((difference / 10));
        }


        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            MainElement = Element as LayoutTouchListner;
        }
    }
}



/*public override void TouchesEnded(NSSet touches, UIEvent evt)
{
    base.TouchesEnded(touches, evt);
    // get the touch
    UITouch touch = touches.AnyObject as UITouch;
    if (touch != null)
    {
        //MainElement.DoTouchEvent((difference / 10));
    }
    // reset our tracking flags
    touchStartedInside = false;
}*/