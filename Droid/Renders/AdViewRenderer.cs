using System;
using Android.Widget;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Registro.Droid.Renders;
using Android.Net;

[assembly: ExportRenderer(typeof(Registro.Controls.AdControlView), typeof(AdViewRenderer))]
namespace Registro.Droid.Renders
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class AdViewRenderer : ViewRenderer<Controls.AdControlView, AdView>
    {
        AdSize adSize = AdSize.SmartBanner;
        AdView adView;

        AdView CreateAdView()
        {
            string adUnitId;
            if (!App.isDebugMode)
                adUnitId = "ca-app-pub-4070857653436842/7938242493";
            else
                adUnitId = "ca-app-pub-3940256099942544/6300978111";

            if (adView != null)
                return adView;

            // This is a string in the Resources/values/strings.xml that I added or you can modify it here. This comes from admob and contains a / in it
            adView = new AdView(Forms.Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            adView.LayoutParameters = adParams;
            adView.AdListener = new MyAdListener();
            adView.LoadAd(new AdRequest
                            .Builder()
                            .Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Controls.AdControlView> e)
        {
            base.OnElementChanged(e);

            bool NoAds = Application.Current.Properties.ContainsKey("noAds") && (Application.Current.Properties["noAds"] as string).Equals("true");
            App.AdsAvailable = !NoAds;
            if (Control == null && !NoAds)
            {
                CreateAdView();
                SetNativeControl(adView);
            } 
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete

    public class MyAdListener : AdListener
    {
        public override void OnAdLoaded()
        {
            base.OnAdLoaded();
            App.AdsAvailable = true;
        }

        public override void OnAdFailedToLoad(int errorCode)
        {
            base.OnAdFailedToLoad(errorCode);
            App.AdsAvailable = false;
        }
    }
}