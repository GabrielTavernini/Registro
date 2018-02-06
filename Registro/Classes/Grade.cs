using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Registro
{
    class Grade
    {
        public String date { get; set; }
        public String Description { get; set; }
        public String type { get; set; }
        public float grade { get; set; }
        public Subject subject { get; set; }
        public String subjectName { get => this.subject.name; }
        public String gradeString { get; set; }
        public DateTime dateTime { get; set; }
        public int Id { get; set; }

        public Grade(String date, String type, String grade, String Description, Subject subject, Boolean save)
        {
            this.date = date;
            this.Description = Description;
            this.type = type;
            this.grade = convertGrade(grade);
            this.gradeString = grade;
            this.subject = subject;
            if(date != "") this.dateTime = ConvertDate(date);
            if (save) App.Grades.Add(this);
        }

        public void setGrade(String grade)
        {
            this.grade = convertGrade(grade);
            this.gradeString = grade;
        }

        private float convertGrade(String g)
        {
            String s;

            if (g.Contains("+")) s = g.Replace("+", ".25");
            else if (g.Contains("-"))
            {
                int i = int.Parse(g.Replace("-", "")) - 1;
                s = i.ToString() + ".75";
            }
            else if (g.Contains("½")) s = g.Replace("½", ".5");
            else if ("".Equals(g)) return (float)0;
            else s = g;

            float Grade = float.Parse(s);

            if (Grade > 10)
            {
                Grade = Grade / 10;

                if (Grade > 10)
                    Grade = Grade / 10;
            }
            return Grade;
        }

        private DateTime ConvertDate(String date)
        {
            try { return DateTime.Parse(date); } 
            catch { return new DateTime(); }
        }

    }
}
