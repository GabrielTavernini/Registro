using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Widget;
using Registro.Droid;
using static Registro.Controls.AndroidNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(Notifications))]
namespace Registro.Droid
{
    public class Notifications : INotify
    {
        public void NotifyMark(Grade g, int id)
        {
            Context c = MainActivity.Instance;

            NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
                .SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
                .SetContentTitle("Nuovo Voto")               // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.icon_transparent)
                .SetLargeIcon(BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.icon))// Display this icon
                .SetContentText(String.Format(
                    "Hai preso {0} di {1}", g.gradeString, g.subject.name)); // The message to display.

            // Finally, publish the notification:
            NotificationManager notificationManager =
                (NotificationManager)c.GetSystemService(Context.NotificationService);
            notificationManager.Notify(id, builder.Build());
        }

        public void NotifyNotes(Note n, int id)
        {
            Context c = MainActivity.Instance;

            NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
                .SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
                .SetContentTitle("Nuova Nota")               // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.icon_transparent)
                .SetLargeIcon(BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.icon))// Display this icon
                .SetContentText(String.Format(
                    "Hai preso una nota da {0}", n.Nome)); // The message to display.

            // Finally, publish the notification:
            NotificationManager notificationManager =
                (NotificationManager)c.GetSystemService(Context.NotificationService);
            notificationManager.Notify(id, builder.Build());
        }

        public void NotifyAbsence(Absence a, int id)
        {
            Context c = MainActivity.Instance;

            NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
                .SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
                .SetContentTitle("Nuova Assenza")               // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.icon_transparent)
                .SetLargeIcon(BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.icon));// Display this icon

            if (a.Type == "Assenza")
                builder.SetContentText(String.Format(
                    "Sei stato assente il {0}", a.date));

            if (a.Type == "Uscita")
                builder.SetContentText(String.Format(
                    "Sei uscito in anticipo il {0}", a.date));

            if (a.Type == "Ritardo")
                builder.SetContentText(String.Format(
                    "Sei entrato in ritardo il {0}", a.date));

            // Finally, publish the notification:
            NotificationManager notificationManager =
                (NotificationManager)c.GetSystemService(Context.NotificationService);
            notificationManager.Notify(id, builder.Build());
        }
    }


    [BroadcastReceiver(Enabled = true)]
    public class AlarmRefreshReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Task.Run(async () => await HttpRequest.RefreshAsync());
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class BootReciver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            MainActivity.StartAlarm(MainActivity.Instance);
        }
    }
}


/*
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        Context c = MainActivity.Instance;

        if (MainActivity.manager == null)
        {
            MainActivity.manager = (AlarmManager)c.GetSystemService(Context.AlarmService);
            Intent myIntent;
            PendingIntent pendingIntent;
            myIntent = new Intent(c, typeof(AlarmRefreshReceiver));
            pendingIntent = PendingIntent.GetBroadcast(c, 0, myIntent, 0);

            MainActivity.manager.SetRepeating(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + 1, 1000 * 60, pendingIntent);
        }
    }
}*/