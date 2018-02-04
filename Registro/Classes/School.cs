using System;

using Xamarin.Forms;

namespace Registro
{
    public class School : ContentPage
    {
        public String marksUrl { get; set; }
        public String formUrl { get; set; }
        public String loginUrl { get; set; }
        public String name { get; set; }

        public School(String loginUrl, String formUrl, String marksUrl, String name)
        {
            this.loginUrl = loginUrl;
            this.name = name;
            this.formUrl = formUrl;
            this.marksUrl = marksUrl;
        }
    }
}

