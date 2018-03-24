using System;
using Registro.Classes;
using Xamarin.Forms;

namespace Registro.Models
{
    public class AbsenceModel
    {
        public String Type { get; set; } = "";
        public String date { get; set; } = "";
        public DateTime dateTime { get; set; }
        public Boolean Void { get; set; } = false;
        public int Id { get; set; }
        public Color color { get; set; } = Color.Orchid;
        public String Preview { get; set; } = "";
        public String FirstLetter { get; set; } = "";

        public AbsenceModel(Absence a, int id)
        {
            this.date = a.date;
            this.Type = "Assenza";
            this.Preview = "Assenza giornaliera";
            if (a.justified)
                this.Preview += " giustificata";
            this.FirstLetter = Type.Substring(0, 1);
            this.dateTime = a.dateTime;
            this.Id = id;
        }

        public AbsenceModel(Absence a, int id, Color c)
        {
            this.date = a.date;
            this.Type = "Assenza";
            this.Preview = "Assenza giornaliera";
            this.FirstLetter = Type.Substring(0, 1);
            this.dateTime = a.dateTime;
            this.Id = id;
            this.color = c;
        }

        public AbsenceModel(LateEntry l, int id)
        {
            this.date = l.date;
            this.Type = "Ritardo";
            if (l.justified)
                this.Preview += " giustificata";
            this.Preview = "Entrata in ritardo alle " + l.entryHour;
            this.FirstLetter = Type.Substring(0, 1);
            this.dateTime = l.dateTime;
            this.Id = id;
        }

        public AbsenceModel(EarlyExit e, int id)
        {
            this.date = e.date;
            this.Type = "Uscita";
            this.Preview = "Uscita anticipata alle " + e.exitHour;
            this.FirstLetter = Type.Substring(0, 1);
            this.dateTime = e.dateTime;
            this.Id = id;
        }
    }
}
