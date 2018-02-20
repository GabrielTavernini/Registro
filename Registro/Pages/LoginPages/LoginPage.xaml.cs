using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Registro.Pages
{
    public partial class LoginPage : ContentPage
    {
        School school;

        public LoginPage(String schoolString)
        {
            Initialize();
            if(schoolString == "IC Valle dei Laghi Dro - SSPG Dro")
            {
                school = new School(
                     "https://www.lampschool.it/hosting_trentino_17_18/login/login.php?suffisso=scuola_27",
                     "Dro"
                 ); 
            }
        }

        protected void Initialize()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Title = "Login";

            label1.Scale = 0;
            UserEntry.Scale = 0;
            PassEntry.Scale = 0;
            buttonStack.Scale = 0;
            if (Device.RuntimePlatform == Device.iOS)
            {
                LoginStack.Spacing = 4;
                UserEntry.BackgroundColor = Color.FromHex("#3FFF");
                PassEntry.BackgroundColor = Color.FromHex("#3FFF");
            }
        }

        async void AuthButtonClicked(object sender, EventArgs e)
        {

            await btnAuthenticate.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            btnAuthenticate.IsVisible = false;

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LodingLabel.IsVisible = true;

            User user = new User(UserEntry.Text, PassEntry.Text, school);

            HttpRequest.User = user;
            if(!await HttpRequest.extractAllAsync())
            {
                System.Diagnostics.Debug.WriteLine("Connection Error!");
                return;
            }

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            LodingLabel.IsVisible = false;

            await label1.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await UserEntry.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await PassEntry.FadeTo(0, App.AnimationSpeed, Easing.SinIn);

            Application.Current.Properties["username"] = UserEntry.Text;
            Application.Current.Properties["password"] = PassEntry.Text;
            Application.Current.Properties["school"] = school.name;

            await Navigation.PushAsync(new HomePage());
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();

            await Task.Delay(App.DelaySpeed);
            await label1.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await UserEntry.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await PassEntry.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await buttonStack.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
        }
    }
}
