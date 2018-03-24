using System;
using System.Globalization;

namespace Registro.Classes
{
    public class LateEntry
    {
        public String entryHour { get; set; } = "";
        public String date { get; set; } = "";
        public DateTime dateTime { get; set; }
        public bool justified { get; set; } = false;

        public LateEntry(String date, String entryHour, bool justified)
        {
            this.justified = justified;
            this.entryHour = entryHour;
            this.date = date;
            if (date != "")
                this.dateTime = ConvertDate(date);
        }

        public LateEntry() { }

        public void setDate(String date)
        {
            this.date = date;
            this.dateTime = ConvertDate(date);
        }

        private DateTime ConvertDate(String date)
        {
            try { return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
            catch { return new DateTime(); }
        }
    }
}
