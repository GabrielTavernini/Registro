using System;
using System.Globalization;

namespace Registro
{
    public class Note
    {
        public String Nome { get; set; } = "";
        public String Text { get; set; } = "";
        public String Measures { get; set; } = "";
        public String date { get; set; } = "";
        public DateTime dateTime { get; set; }


        public Note(String Nome, String Text, String Measures, String date)
        {
            this.date = date;
            this.Text = Text;
            this.Nome = Nome;
            this.Measures = Measures;
            if (date != "") 
                this.dateTime = ConvertDate(date);
        }

        public Note() { }

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
