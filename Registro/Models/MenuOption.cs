using System;

using Xamarin.Forms;

namespace Registro.Models
{
    public class MenuOption
    {
        public String title { get; set; }
        public ImageSource image { get; set; }
        public Color color { get; set; }
        public int Height { get; set; } = 150;
        public int Id { get; set; }

        public MenuOption(String title, ImageSource image, Color color, int Id)
        {
            this.title = title;
            this.image = image;
            this.color = color;
            this.Id = Id;
        }

        public static MenuOption VoidCell(int Id)
        {
            MenuOption mo = new MenuOption("", ImageSource.FromFile(""), Color.Transparent, Id);
            mo.Height = 10;
            return mo;
        }

        public static MenuOption VoidCell2(int Id)
        {
            MenuOption mo = new MenuOption("", ImageSource.FromFile(""), Color.Transparent, Id);
            mo.Height = 1;
            return mo;
        }
    }
}

