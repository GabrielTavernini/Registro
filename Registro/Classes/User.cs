using System;
using System.Collections.Generic;
using Registro.Classes;
using Registro.Models;
using Xamarin.Forms;

namespace Registro
{
    public class User
    {
		public String nome { get; set; }
        public String username { get; set; }
        public String password { get; set; }
        public School school { get; set; }

        public User(String username, String password, School school)
        {
            this.username = username;
            this.password = password;
            this.school = school;
        }
    }

    public class UserBackUp
    {
        public String name { get; set; }
        public String username { get; set; }
        public String password { get; set; }
        public String schoolName { get; set; }
        public String schoolUrl { get; set; }

        public List<Grade> grades = new List<Grade>();
        //public Dictionary<String, Subject> subjects = new Dictionary<string, Subject>();
        public List<Arguments> arguments = new List<Arguments>();
        public List<Note> notes = new List<Note>();
        public List<Absence> absences = new List<Absence>();
        public Settings settings = new Settings();
        public List<LateEntry> lateEntries = new List<LateEntry>();
        public List<EarlyExit> earlyExits = new List<EarlyExit>();
    }
}

