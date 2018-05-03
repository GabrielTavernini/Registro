using System;
using Android.Content;
using Android.Graphics.Drawables;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using XamarinForms;
using XamarinForms.Droid.Renderes;
using Registro.Controls;
using Android.Support.V4.View;

[assembly: ExportRenderer(typeof(SportButton), typeof(CustomButtonRenderer))]
namespace XamarinForms.Droid.Renderes
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        public CustomButtonRenderer(Context context) : base(context) {}

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Control != null)
            {
                var roundableShape = new GradientDrawable();
                roundableShape.SetShape(ShapeType.Rectangle);
                roundableShape.SetStroke(1, Element.BorderColor.ToAndroid());
                roundableShape.SetColor(Element.BackgroundColor.ToAndroid());
                roundableShape.SetCornerRadius(25);
                ViewCompat.SetBackground(Control, roundableShape);
                //Control.TransformationMethod = null;
                ViewCompat.SetElevation(Control, 0);
            }
            base.OnElementPropertyChanged(sender, e);
        }
            /*var roundableShape = new GradientDrawable();
            roundableShape.SetShape(ShapeType.Rectangle);
            roundableShape.SetStroke(1, Color.FromHex("#AFFF").ToAndroid());
            roundableShape.SetColor(color.ToAndroid());
            roundableShape.SetCornerRadius(25);
            Control.Background = roundableShape;
            Control.TransformationMethod = null;
            Control.Elevation = 0;*/
    }
}
