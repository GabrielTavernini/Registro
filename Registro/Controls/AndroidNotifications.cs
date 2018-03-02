using System;
namespace Registro.Controls
{
    public class AndroidNotifications
    {
        public interface INotify
        {
            void NotifyMark(Grade g, int id);
            void NotifyNotes(Note n, int id);
            void NotifyAbsence(Absence a, int id);
            void NotifyArguments(Arguments a, int id);
            void StopAlarm();
        }
    }
}
