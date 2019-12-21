using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Registro.Models
{
    public class GradeModel
    {
        public String date { get; set; } = "";
        public DateTime dateTime { get; set; } = new DateTime();
        public String Description { get; set; } = "";
        public String type { get; set; } = "";
        public String subject { get; set; } = "";
        public String gradeString { get; set; } = "";
        public float grade { get; set; } = 0.0f;
        public Boolean ShapeVisible { get; set; } = true;
        public int Id { get; set; } = 0;
        private Color _color = Color.DarkBlue;
        public Color color { get { return _color; } set { SetColor(value); } }

        public GradeModel(Grade g, int Id)
        {
            this.Description = g.Description;
            this.date = g.date;
            this.type = g.type;
            this.subject = g.subject.name;
            this.gradeString = g.gradeString;
            this.dateTime = g.dateTime;
            this.grade = g.grade;
            this.Id = Id;
            SetColor(color);
        }

        public GradeModel(Grade g, int Id, Color color)
        {
            this.Description = g.Description;
            this.date = g.date;
            this.type = g.type;
            this.subject = g.subject.name;
            this.gradeString = g.gradeString;
            this.dateTime = g.dateTime;
            this.grade = g.grade;
            this.Id = Id;
            SetColor(color);
        }

        private void SetColor(Color setColor)
        {
            Debug.WriteLine(gradeString);
            if (!App.Settings.coloredMarks || subject == "NESSUN VOTO")
            {
                this._color = setColor;
            }
            else
            {
                Debug.WriteLine(grade.GetType());
                if (grade >= 6.0)
                    this._color = Color.Green;
                else if (grade >= 5.0)
                    this._color = Color.Orange;
                else
                    this._color = Color.Red;
            }
        }

        public static GradeModel VoidCell(int Id)
        {
            GradeModel gm = new GradeModel(new Grade(), Id);
            gm.subject = " ";
            gm.type = " ";
            gm.ShapeVisible = false;
            gm.color = Color.White;
            return gm;
        }
    }
}
