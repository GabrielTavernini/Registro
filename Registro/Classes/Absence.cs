using System;
using System.Globalization;

namespace Registro
{
    public class Absence
    {
        public String Type { get; set; } = "";
        public String date { get; set; } = "";
        public DateTime dateTime { get; set; }

        public Absence(String Type, String date, Boolean save)
        {
            this.date = date;
            date = date.Substring(0, 10);
            this.Type = Type;
            if (date != "")
                this.dateTime = ConvertDate(date);
            if (save)
                App.Absences.Add(this);
        }

        public Absence() { }

        public void setDate(String date)
        {
            this.date = date;
            date = date.Substring(0, 10);
            this.dateTime = ConvertDate(date);
        }

        private DateTime ConvertDate(String date)
        {
            try { return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
            catch { return new DateTime(); }
        }
    }
}
