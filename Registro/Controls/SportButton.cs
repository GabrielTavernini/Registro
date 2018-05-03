using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace Registro.Controls
{
    public class SportButton : Button
    {
        public SportButton() : base()
        {
            const int _animationTime = 100;
            Clicked += async (sender, e) =>
            {
                try
                {
                    var btn = (SportButton)sender;
                    await btn.ScaleTo(1.2, _animationTime);
                    await btn.ScaleTo(1, _animationTime);
                }
                catch { }
            };

            Pressed += async (sender, e) =>
            {
                try
                {
                    var btn = (SportButton)sender;
                    btn.BackgroundColor = btn.BackgroundColor.MultiplyAlpha(1.2);
                    await btn.ScaleTo(.99, _animationTime);
                }
                catch { }
            };

            Released += async (sender, e) =>
            {
                try
                {
                    var btn = (SportButton)sender;
                    btn.BackgroundColor = btn.BackgroundColor.MultiplyAlpha(0.833333333);
                    await btn.ScaleTo(1, _animationTime);
                }
                catch { }
            };
        }
    }
}