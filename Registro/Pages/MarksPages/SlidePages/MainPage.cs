using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Registro.Pages
{
    public class MainPage : CarouselPage
    {
        User user;
        static public int pagesNumber = 6;

        public MainPage()
        {
            /*NavigationPage.SetHasNavigationBar(this, false);

            Children.Add(new TestPage("Voti", Color.FromHex("#00B1D4"), 72));
            Children.Add(new TestPage("Medie", Color.FromHex("#61DDDD"), 72));
            Children.Add(new TestPage("Argomenti", Color.FromHex("#B2D235"), 56));
            Children.Add(new TestPage("Note", Color.FromHex("#F2AA52"), 72));
            Children.Add(new TestPage("Assenze", Color.FromHex("#E15B5C"), 72));
            Children.Add(new TestPage("Impostazioni", Color.FromHex("#E15BBB"), 50)); */
        }

        public MainPage(User user)
        {
            this.user = user;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NavigationPage.SetHasNavigationBar(this, false);

            //HttpRequest.User = user; 
            //await HttpRequest.extractAllAsync();

            Children.Add(new TestPage("Voti", Color.FromHex("#00B1D4"), 72, 1));
            Children.Add(new TestPage("Medie", Color.FromHex("#61DDDD"), 72, 2));
            Children.Add(new TestPage("Argomenti", Color.FromHex("#B2D235"), 56, 3));
            Children.Add(new TestPage("Note", Color.FromHex("#F2AA52"), 72, 4));
            Children.Add(new TestPage("Assenze", Color.FromHex("#E15B5C"), 72, 5));
            Children.Add(new TestPage("Impostazioni", Color.FromHex("#E15BBB"), 50, 6));  
        }
    }
}
