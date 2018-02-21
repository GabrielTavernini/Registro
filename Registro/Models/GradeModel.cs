using System;
using Xamarin.Forms;

namespace Registro.Models
{
    public class GradeModel
    {
        public String date { get; set; }
        public DateTime dateTime { get; set; }
        public String Description { get; set; }
        public String type { get; set; }
        public String subject { get; set; }
        public String gradeString { get; set; }
        public float grade { get; set; }
        public Boolean Void { get; set; } = false;
        public int Id { get; set; }
        public Color color { get; set; } = Color.DarkBlue;

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
            this.color = color;
        }

        public static GradeModel VoidCell(int Id)
        {
            GradeModel gm = new GradeModel(new Grade(), Id);
            gm.Void = true;
            return gm;
        }
    }
}
