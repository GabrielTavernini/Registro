using System;

using Xamarin.Forms;

namespace Registro
{
    public class User : ContentPage
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
}

