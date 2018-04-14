using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registro
{
    public class Subject
    {
        public String name { get; set; }
        public List<Grade> grades { get; set; }


        public Subject(String name, List<Grade> grades)
        {
            this.name = name;
            this.grades = grades;
        }

        public Subject(String name)
        {
            this.name = name;
            this.grades = new List<Grade>();
        }

        public Grade getMedia()
        {
            Grade media = new Grade("", "", "", "Media", this);
            float sum = (float)0.0;
            int i = 0;

            foreach (Grade grade in this.grades)
            {
                sum += grade.grade;
                i++;
            }

            float mediaFloat = sum / i;
            media.setGrade(mediaFloat.ToString("0.00"));

            if (mediaFloat == 10.00)
                media.setGrade("10");

            return media;
        }

        public Grade getMedia1()
        {
            Grade media = new Grade("", "", "", "Media", this);
            float sum = (float)0.0;
            int i = 0;

            foreach (Grade grade in this.grades)
            {
                if (grade.dateTime.CompareTo(App.Settings.periodChange) <= 0)
                {
                    sum += grade.grade;
                    i++;
                }
            }

            float mediaFloat = sum / i;
            media.setGrade(mediaFloat.ToString("0.00"));

            if (mediaFloat == 10.00)
                media.setGrade("10");
            
            return media;
        }

        public Grade getMedia2()
        {
            Grade media = new Grade("", "", "", "Media", this);
            float sum = (float)0.0;
            int i = 0;

            foreach (Grade grade in this.grades)
            {
                if (grade.dateTime.CompareTo(App.Settings.periodChange) > 0)
                {
                    sum += grade.grade;
                    i++;   
                }
                    
            }

            float mediaFloat = sum / i;
            media.setGrade(mediaFloat.ToString("0.00"));

            if (mediaFloat == 10.00)
                media.setGrade("10");
            
            return media;
        }

        static public Subject getSubjectByString(String s)
        {
            Subject sub;
            App.Subjects.TryGetValue(s, out sub);
            return sub;
        }
    }

}

