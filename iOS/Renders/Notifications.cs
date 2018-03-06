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

            notification.FireDate = NSDate.FromTimeIntervalSinceNow(10);

            // configure the alert
            notification.AlertAction = "Nuovo Voto";
            notification.AlertBody = String.Format("Hai preso {0} di {1}", g.gradeString, g.subject.name);

            // modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            Console.WriteLine("Scheduled...");
        }

        public void NotifyNotes(Note n)
        {
            
        }

        public void NotifyAbsence(Absence a)
        {
            
        }

        public void NotifyArguments(Arguments a)
        {
            
        }

        public void StopAlarm()
        {
            
        }
    }

}
