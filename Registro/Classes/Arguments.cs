using System;
using System.Globalization;
using Xamarin.Forms;

namespace Registro
{
    public class Arguments
    {
        public String date { get; set; } = "";
        public String Argument { get; set; } = "";
        public String Activity { get; set; } = "";
        public String subject { get; set; } = "";
        public DateTime dateTime { get; set; }

        public Arguments(String Argument, String Activity, String date, String sub, Boolean save)
        {
            this.date = date;
            this.subject = sub;
            this.Argument = Argument;
            this.Activity = Activity;
            if (date != "") this.dateTime = ConvertDate(date);
            if (save) App.Arguments.Add(this);
        }

        public Arguments() { }

        private DateTime ConvertDate(String date)
        {
            try { return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture); }
            catch { return new DateTime(); }
        }
    }
}
