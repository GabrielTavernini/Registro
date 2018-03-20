using System;
using Android.Content;
using Xamarin.Forms.PlatformConfiguration;
using static Registro.Controls.Mails;

[assembly: Xamarin.Forms.Dependency(typeof(Registro.Droid.Email))]
namespace Registro.Droid
{
    public class Email : IMailAndroid
    {
        public void SendEmail()
        {
            var email = new Intent(Android.Content.Intent.ActionSend);
            email.PutExtra(Android.Content.Intent.ExtraEmail,
            new string[] { "taverninigabriel@gmail.com" });

            email.PutExtra(Android.Content.Intent.ExtraSubject, "Segnalazione Problema");

            email.PutExtra(Android.Content.Intent.ExtraText,
                           "SEGNALAZIONE PROBLEMA per l'app LAMPSCHOOL (API: "+ Android.OS.Build.VERSION.SdkInt +"):\n\n");

            email.SetType("message/rfc822");
            MainActivity.Instance.StartActivity(email);
        }
    }
}
