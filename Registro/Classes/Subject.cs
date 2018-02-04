using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registro
{
    class Subject
    {
        public String name { get; set; }
        public List<Grade> grades { get; set; }


        public Subject(String name, List<Grade> grades, Boolean save)
        {
            this.name = name;
            this.grades = grades;
            if (save) App.Subjects.Add(this.name, this);

        }

        public Subject(String name, Boolean save)
        {
            this.name = name;
            if (save) App.Subjects.Add(this.name, this);
        }

        public Grade getMedia()
        {
            Grade media = new Grade("", "", "", "Media", this, false);
            float sum = (float)0.0;
            int i = 0;

            foreach (Grade grade in this.grades)
            {
                sum += grade.grade;
                i++;
            }

            float mediaFloat = sum / i;
            media.setGrade(mediaFloat.ToString("0.00"));

            return media;
        }

        static Subject getSubjectByString(String s)
        {
            Subject sub;
            App.Subjects.TryGetValue(s, out sub);
            return sub;
        }
    }

}

