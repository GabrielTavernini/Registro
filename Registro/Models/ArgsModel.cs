using System;
using Xamarin.Forms;

namespace Registro.Models
{
    public class ArgsModel
    {
        public String date { get; set; } = "";
        public String Argument { get; set; } = "";
        public string Activity { get; set; } = "";
        public String Preview { get; set; } = "";
        public String FirstLetter { get; set; } = "";
        public String subject { get; set; } = "";
        public DateTime dateTime { get; set; }
        public Boolean Void { get; set; } = false;
        public int Id { get; set; }
        public Color color { get; set; } = Color.Green;

        public ArgsModel(Arguments a, int Id)
        {
            this.date = a.date;
            this.subject = a.subject;
            this.Argument = a.Argument;
            this.Activity = a.Activity;
            this.Preview = getPreview();         
            this.FirstLetter = a.subject.Substring(0, 1);
            this.dateTime = a.dateTime;
            this.Id = Id;
        }

        public ArgsModel(Arguments a, int Id, Color color)
        {
            this.date = a.date;
            this.subject = a.subject;
            this.Argument = a.Argument;
            this.Activity = a.Activity;
			this.Preview = getPreview();
            this.FirstLetter = a.subject.Substring(0, 1);
            this.dateTime = a.dateTime;
            this.Id = Id;
            this.color = color;
        }

		private string getPreview()
		{
			if (Argument != "")
				if (Argument.Length >= 23)
					return Argument.Substring(0, 22) + "...";
				else
					return Argument;
			else if (Activity != "")
				if (Activity.Length >= 23)
					return Activity.Substring(0, 22) + "...";
				else
					return Activity;
			else
				return "Nessuna Descrizione";
		}

        public static ArgsModel VoidCell(int Id)
        {
            ArgsModel gm = new ArgsModel(new Arguments("", "", "", ""), Id);
            gm.Id = Id;
            gm.Void = true;
            return gm;
        }
    }
}
