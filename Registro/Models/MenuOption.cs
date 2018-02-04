using System;

using Xamarin.Forms;

namespace Registro.Models
{
    public class MenuOption
    {
        public String title { get; set; }
        public ImageSource image { get; set; }
        public Color color { get; set; }
        public int Id { get; set; }

        public MenuOption(String title, ImageSource image, Color color, int Id)
        {
            this.title = title;
            this.image = image;
            this.color = color;
            this.Id = Id;
        }
    }
}

