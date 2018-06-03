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
using Android.Gms.Common;
using Android.Widget;
using Registro.Droid;
using static Registro.Controls.Notifications;
using Registro.Pages;
using System.Text;
using Registro.Classes.JsonRequest;
using Registro.Classes;
using Android.Gms.Gcm;
using Firebase.JobDispatcher;

[assembly: Xamarin.Forms.Dependency(typeof(Registro.Droid.Notifications))]
namespace Registro.Droid
{
	public class Notifications : INotifyAndroid
	{
		public void NotifyMark(Grade g, int id)
		{
			Context c = MainActivity.Instance;
			Intent inte = new Intent(c, typeof(MainActivity));
			inte.SetAction(String.Format("{0}:{1}", g.subject.name, g.date));
			PendingIntent pe = PendingIntent.GetActivity(c, 0, inte, PendingIntentFlags.UpdateCurrent);

			NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
				.SetContentIntent(pe)
				.SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
				.SetContentTitle("Nuovo Voto")               // Display the count in the Content Info
				.SetSmallIcon(Resource.Drawable.NotificationsIcon)
				.SetColor(Color.Argb(255, 0, 177, 212))
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
			Intent inte = new Intent(c, typeof(MainActivity));
			inte.SetAction("NotesPage");
			PendingIntent pe = PendingIntent.GetActivity(c, 0, inte, PendingIntentFlags.UpdateCurrent);

			NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
				.SetContentIntent(pe)
				.SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
				.SetContentTitle("Nuova Nota")               // Display the count in the Content Info
				.SetSmallIcon(Resource.Drawable.NotificationsIcon)
				.SetColor(Color.Argb(255, 0, 177, 212))
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
			Intent inte = new Intent(c, typeof(MainActivity));
			inte.SetAction("AbsencesPage");
			PendingIntent pe = PendingIntent.GetActivity(c, 0, inte, PendingIntentFlags.UpdateCurrent);

			NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
				.SetContentIntent(pe)
				.SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
				.SetSmallIcon(Resource.Drawable.NotificationsIcon)
				.SetColor(Color.Argb(255, 0, 177, 212))
				.SetLargeIcon(BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.icon));// Display this icon

			builder.SetContentText(String.Format("Sei stato assente il {0}", a.date));
			builder.SetContentTitle("Nuova Assenza");


			// Finally, publish the notification:
			NotificationManager notificationManager =
				(NotificationManager)c.GetSystemService(Context.NotificationService);
			notificationManager.Notify(id, builder.Build());
		}

		public void NotifyLateEntry(LateEntry a, int id)
		{
			Context c = MainActivity.Instance;
			Intent inte = new Intent(c, typeof(MainActivity));
			inte.SetAction("AbsencesPage");
			PendingIntent pe = PendingIntent.GetActivity(c, 0, inte, PendingIntentFlags.UpdateCurrent);

			NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
				.SetContentIntent(pe)
				.SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
				.SetSmallIcon(Resource.Drawable.NotificationsIcon)
				.SetColor(Color.Argb(255, 0, 177, 212))
				.SetLargeIcon(BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.icon));// Display this icon

			builder.SetContentText(String.Format("Sei entrato in ritardo il {0}", a.date));
			builder.SetContentTitle("Nuova Entrata in Ritardo");


			// Finally, publish the notification:
			NotificationManager notificationManager =
				(NotificationManager)c.GetSystemService(Context.NotificationService);
			notificationManager.Notify(id, builder.Build());
		}

		public void NotifyEarlyExit(EarlyExit a, int id)
		{
			Context c = MainActivity.Instance;
			Intent inte = new Intent(c, typeof(MainActivity));
			inte.SetAction("AbsencesPage");
			PendingIntent pe = PendingIntent.GetActivity(c, 0, inte, PendingIntentFlags.UpdateCurrent);

			NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
				.SetContentIntent(pe)
				.SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
				.SetSmallIcon(Resource.Drawable.NotificationsIcon)
				.SetColor(Color.Argb(255, 0, 177, 212))
				.SetLargeIcon(BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.icon));// Display this icon

			builder.SetContentText(String.Format("Sei uscito in anticipo il {0}", a.date));
			builder.SetContentTitle("Nuova Uscita Anticipata");


			// Finally, publish the notification:
			NotificationManager notificationManager =
				(NotificationManager)c.GetSystemService(Context.NotificationService);
			notificationManager.Notify(id, builder.Build());
		}

		public void NotifyArguments(Arguments a, int id)
		{
			Context c = MainActivity.Instance;
			Intent inte = new Intent(c, typeof(MainActivity));
			inte.SetAction("ArgsPage");
			PendingIntent pe = PendingIntent.GetActivity(c, 0, inte, PendingIntentFlags.UpdateCurrent);

			StringBuilder sb = new StringBuilder();
			sb.Append(a.Argument);
			if (a.Activity != null && a.Activity != "")
			{
				sb.AppendLine(" ");
				sb.AppendLine(" ");
				sb.AppendLine("Attività:");
				sb.Append(a.Activity);
			}


			NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
				.SetContentIntent(pe)
				.SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
				.SetContentTitle(String.Format("Nuovo Argomento di {0}", a.subject))               // Display the count in the Content Info
				.SetSmallIcon(Resource.Drawable.NotificationsIcon)
				.SetColor(Color.Argb(255, 0, 177, 212))
				.SetLargeIcon(BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.icon))
				.SetStyle(new NotificationCompat.BigTextStyle()
						  .BigText(sb.ToString()));

			builder.SetContentText(a.Argument);

			// Finally, publish the notification:
			NotificationManager notificationManager =
				(NotificationManager)c.GetSystemService(Context.NotificationService);
			notificationManager.Notify(id, builder.Build());
		}

		public void StopAlarm()
		{
			//MainActivity.StopAlarm();
		}

		public void DisplayToast(string text)
		{
			if (MainActivity.IsForeground)
			{
				MainActivity.Instance.RunOnUiThread(() =>
				{
					Toast.MakeText(MainActivity.Instance, text, ToastLength.Short).Show();
				});
			}
		}
	}


	[Service(Name = "com.gabriel.Registro")]
    [IntentFilter(new[] { FirebaseJobServiceIntent.Action })]
    public class RefreshJob : Firebase.JobDispatcher.JobService
	{
        static readonly string TAG = "X:RefreshService";

        public override bool OnStartJob(IJobParameters jobParameters)
        {
			System.Threading.Tasks.Task.Run(async () =>
            {
				// Work is happening asynchronously (code omitted)
				//await JsonRequest.JsonLogin();
            });

			MainActivity.Instance.RunOnUiThread(() =>
            {
                Toast.MakeText(MainActivity.Instance, "Jobnfsdfg", ToastLength.Short).Show();
            });

			Context c = MainActivity.Instance;
            Intent inte = new Intent(c, typeof(MainActivity));
            PendingIntent pe = PendingIntent.GetActivity(c, 0, inte, PendingIntentFlags.UpdateCurrent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(c)
                .SetContentIntent(pe)
                .SetAutoCancel(true)  // Start 2nd activity when the intent is clicked.
                .SetContentTitle("Nuovo Voto")               // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.NotificationsIcon)
                .SetColor(Color.Argb(255, 0, 177, 212))
                .SetLargeIcon(BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.icon))// Display this icon
                .SetContentText(String.Format(
                    "Hai preso {0} di {1}", "3", "sdadfagdv")); // The message to display.

            // Finally, publish the notification:
            NotificationManager notificationManager =
                (NotificationManager)c.GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, builder.Build());
            // Return true because of the asynchronous work
            return true;
        }

        public override bool OnStopJob(IJobParameters jobParameters)
        {
            // nothing to do.
            return false;
        }
    }

	
}



/*
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class BootReciver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            AlarmManager manager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent myIntent;
            myIntent = new Intent(context, typeof(AlarmRefreshReceiver));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, 0, myIntent, 0);

            manager.SetInexactRepeating(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + 1000, 60 * 30 * 1000, pendingIntent);  
        }
    }
*/