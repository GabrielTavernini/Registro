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
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                var notification = new UILocalNotification();
                notification.FireDate = NSDate.FromTimeIntervalSinceNow(10);

                // configure the alert
                notification.AlertTitle = "Nuovo Voto";
                notification.AlertBody = String.Format("Hai preso {0} di {1}", g.gradeString, g.subject.name);

                // modify the badge
                UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

                // set the sound to be the default sound
                notification.SoundName = UILocalNotification.DefaultSoundName;

                // schedule it
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                Console.WriteLine("Scheduled...");
            });
        }

        public void NotifyNotes(Note n)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                var notification = new UILocalNotification();
                notification.FireDate = NSDate.FromTimeIntervalSinceNow(10);

                // configure the alert
                notification.AlertTitle = "Nuova Nota";
                notification.AlertBody = String.Format("Hai preso una nota da {0}", n.Nome);

                // modify the badge
                UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

                // set the sound to be the default sound
                notification.SoundName = UILocalNotification.DefaultSoundName;

                // schedule it
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                Console.WriteLine("Scheduled...");
            });
        }

        public void NotifyAbsence(Absence a)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                var notification = new UILocalNotification();
                notification.FireDate = NSDate.FromTimeIntervalSinceNow(10);

                // configure the alert
                if (a.Type == "Assenza")
                {
                    notification.AlertTitle = "Nuova Assenza";
                    notification.AlertBody = String.Format("Sei stato assente il {0}", a.date);
                }
                else if (a.Type == "Uscita")
                {
                    notification.AlertTitle = "Nuova Uscita Anticipata";
                    notification.AlertBody = String.Format("Sei uscito in anticipo il {0}", a.date);
                }
                else if (a.Type == "Ritardo")
                {
                    notification.AlertTitle = "Nuova Entrata in Ritardo";
                    notification.AlertBody = String.Format("Sei entrato in ritardo il {0}", a.date);
                }


                // modify the badge
                UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

                // set the sound to be the default sound
                notification.SoundName = UILocalNotification.DefaultSoundName;

                // schedule it
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                Console.WriteLine("Scheduled...");
            });
        }

        public void NotifyArguments(Arguments a)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                var notification = new UILocalNotification();
                notification.FireDate = NSDate.FromTimeIntervalSinceNow(10);

                // configure the alert
                notification.AlertTitle = String.Format("Nuovo Argomento di {0}", a.subject);
                notification.AlertBody = a.Argument;

                // modify the badge
                UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

                // set the sound to be the default sound
                notification.SoundName = UILocalNotification.DefaultSoundName;

                // schedule it
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                Console.WriteLine("Scheduled...");             
            });
        }

        public void StopAlarm()
        {
        }
    }

}
