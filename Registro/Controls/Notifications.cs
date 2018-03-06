using System;
namespace Registro.Controls
{
    public class Notifications
    {
        public interface INotifyAndroid
        {
            void NotifyMark(Grade g, int id);
            void NotifyNotes(Note n, int id);
            void NotifyAbsence(Absence a, int id);
            void NotifyArguments(Arguments a, int id);
            void StopAlarm();
        }

        public interface INotifyiOS
        {
            void NotifyMark(Grade g);
            void NotifyNotes(Note n);
            void NotifyAbsence(Absence a);
            void NotifyArguments(Arguments a);
            void StopAlarm();
        }
    }
}
