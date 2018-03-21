using System;
using MessageUI;
using UIKit;
using static Registro.Controls.Mails;

[assembly: Xamarin.Forms.Dependency(typeof(Registro.iOS.Renders.Email))]
namespace Registro.iOS.Renders
{
    public class Email : IMailiOS
    {
        MFMailComposeViewController mailController;

        public void SendEmail()
        {
            if (MFMailComposeViewController.CanSendMail)
            {
                // do mail operations here
                mailController = new MFMailComposeViewController();

                mailController.SetToRecipients(new string[] { "taverninigabriel@gmail.com" });
                mailController.SetSubject("Segnalazione Problema");
                mailController.SetMessageBody("SEGNALAZIONE PROBLEMA per l'app LAMPSCHOOL (iOS: " 
                                              + UIDevice.CurrentDevice.SystemVersion 
                                              + "):\n\n", false);
            }
        }
    }
}
