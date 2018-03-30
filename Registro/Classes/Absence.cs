using System;
using System.Globalization;

namespace Registro
{
    public class Absence
    {
        public String Type { get; set; } = "";
        public String date { get; set; } = "";
        public DateTime dateTime { get; set; }
        public bool justified { get; set; } = false;
        public bool justifiedUnknown = false;

        public Absence(String Type, String date, bool justified)
        {
            this.justified = justified;
            this.justifiedUnknown = false;
            this.date = date;
            this.Type = Type;
            if (date != "")
                this.dateTime = ConvertDate(date);
        }

        public Absence(String Type, String date)
        {
            this.date = date;
            this.Type = Type;
            this.justifiedUnknown = true;
            System.Diagnostics.Debug.WriteLine(Type + " " + date);
            if (date != "")
                this.dateTime = ConvertDate(date);
        }

        public Absence() { }

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
