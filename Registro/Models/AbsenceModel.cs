using System;
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

        public AbsenceModel(Absence n, int id)
        {
            this.date = n.date;
            this.Type = n.Type;
            this.Preview = getPreview(n.Type);
            this.FirstLetter = Type.Substring(0, 1);
            this.dateTime = n.dateTime;
            this.Id = id;
        }

        public AbsenceModel(Absence n, int id, Color c)
        {
            this.date = n.date;
            this.Type = n.Type;
            this.Preview = getPreview(n.Type);
            this.FirstLetter = Type.Substring(0, 1);
            this.dateTime = n.dateTime;
            this.Id = id;
            this.color = c;
        }

        private string getPreview(String type)
        {
            if (type == "Ritardo")
                return "Entrata in ritardo";
            else if (type == "Assenza")
                return "Assenza giornaliera";
            else if (type == "Uscita")
                return "Uscita anticipata";
            else
                return "";
        }
    }
}
