using System;
using Foundation;
using UIKit;
using static Registro.Controls.Notifications;

[assembly: Xamarin.Forms.Dependency(typeof(Registro.iOS.Renders.Notifications))]
namespace Registro.iOS.Renders
{
    public class Notifications : INotifyiOS
    {
        public void NotifyMark(Grade g)
        {
            var notification = new UILocalNotification();

            // configure the alert
            notification.AlertAction = "Nuovo Voto";
            notification.AlertBody = String.Format("Hai preso {0} di {1}", g.gradeString, g.subject.name);

            // modify the badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            Console.WriteLine("Scheduled...");
        }

        public void NotifyNotes(Note n)
        {
            var notification = new UILocalNotification();

            // configure the alert
            notification.AlertAction = "Nuova Nota";
            notification.AlertBody = String.Format("Hai preso una nota da {0}", n.Nome);

            // modify the badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            Console.WriteLine("Scheduled..."); 
        }

        public void NotifyAbsence(Absence a)
        {
            var notification = new UILocalNotification();

            // configure the alert
            if (a.Type == "Assenza")
            {
                notification.AlertAction = "Nuova Assenza";
                notification.AlertBody = String.Format("Sei stato assente il {0}", a.date);
            }
            else if (a.Type == "Uscita")
            {
                notification.AlertAction = "Nuova Uscita Anticipata";
                notification.AlertBody = String.Format("Sei uscito in anticipo il {0}", a.date);
            }
            else if (a.Type == "Ritardo")
            {
                notification.AlertAction = "Nuova Entrata in Ritardo";
                notification.AlertBody = String.Format("Sei entrato in ritardo il {0}", a.date);
            }


            // modify the badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            Console.WriteLine("Scheduled..."); 
        }

        public void NotifyArguments(Arguments a)
        {
            var notification = new UILocalNotification();

            // configure the alert
            notification.AlertAction = String.Format("Nuovo Argomento di {0}", a.subject);
            notification.AlertBody = a.Argument;

            // modify the badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            Console.WriteLine("Scheduled..."); 
        }

        public void StopAlarm()
        {
        }
    }

}
