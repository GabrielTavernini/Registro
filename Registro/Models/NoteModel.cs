using System;
using Xamarin.Forms;

namespace Registro.Models
{
    public class NoteModel
    {
        public String Nome { get; set; } = "";
        public String Text { get; set; } = "";
        public String Measures { get; set; } = "";
        public String date { get; set; } = "";
        public DateTime dateTime { get; set; }
        public Boolean Void { get; set; } = false;
        public int Id { get; set; }
        public Color color { get; set; } = Color.Orchid;
        public String Preview { get; set; } = "";
        public String FirstLetter { get; set; } = "";

        public NoteModel(Note n, int id)
        {
            this.date = n.date;
            this.Text = n.Text;
            this.Nome =  n.Nome;
            this.Measures = n.Measures;
            if (Text.Length >= 23)
                this.Preview = Text.Substring(0, 22) + "...";
            else
                this.Preview = Text;
            this.FirstLetter = Nome.Substring(0, 1);
            this.dateTime = n.dateTime;
            this.Id = id;
        }

        public NoteModel(Note n, int id, Color c)
        {
            this.date = n.date;
            this.Text = n.Text;
            this.Nome = n.Nome;
            this.Measures = n.Measures;
            if (Text.Length >= 23)
                this.Preview = Text.Substring(0, 22) + "...";
            else
                this.Preview = Text;
            this.FirstLetter = Nome.Substring(0, 1);
            this.dateTime = n.dateTime;
            this.Id = id;
            this.color = c;
        }
    }
}
