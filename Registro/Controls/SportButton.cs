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
            try
            {
                Clicked += async (sender, e) =>
                {
                    var btn = (SportButton)sender;
                    await btn.ScaleTo(1.2, _animationTime);
                    await btn.ScaleTo(1, _animationTime);
                };

                Pressed += async (sender, e) =>
                {
                    var btn = (SportButton)sender;
                    btn.BackgroundColor = Color.FromHex("#4FFF");
                    await btn.ScaleTo(.99, _animationTime);
                };

                Released += async (sender, e) =>
                {
                    var btn = (SportButton)sender;
                    btn.BackgroundColor = Color.FromHex("#3FFF");
                    await btn.ScaleTo(1, _animationTime);
                };               
            }
            catch{}
		}
	}
}