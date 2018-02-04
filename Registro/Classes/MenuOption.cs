using System;

using Xamarin.Forms;

namespace Registro
{
    public class MenuOption
    {
        public ImageSource immage { get; set; }
        public String title { get; set; }
        public String description { get; set; }

        public MenuOption(String title, String description, ImageSource immage)
        {
            this.title = title;
            this.description = description;
            this.immage = immage;
        }
    }
}

