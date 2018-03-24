using System;
using System.Globalization;

namespace Registro.Classes
{
    public class EarlyExit
    {
        public String exitHour { get; set; } = "";
        public String date { get; set; } = "";
        public DateTime dateTime { get; set; }


        public EarlyExit(String date, String exitHour)
        {
            this.exitHour = exitHour;
            this.date = date;
            if (date != "")
                this.dateTime = ConvertDate(date);
        }

        public EarlyExit() { }

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
