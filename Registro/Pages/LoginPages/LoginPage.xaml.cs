using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Registro.Controls.AndroidThemes;
using static Registro.Controls.Notifications;

namespace Registro.Pages
{
    public partial class LoginPage : ContentPage
    {
        School school;

        public LoginPage(School selectedSchool)
        {
            Initialize();
            school = selectedSchool;
        }

        protected void Initialize()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<IThemes>().setSettingsTheme(); 
            
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
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().DisplayToast("Autenticazione non riuscita");
                
                System.Diagnostics.Debug.WriteLine("Connection Error!");
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                LodingLabel.IsVisible = false;

                btnAuthenticate.IsVisible = true;
                await btnAuthenticate.FadeTo(1, App.AnimationSpeed, Easing.SinIn);
                return;
            }

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            LodingLabel.IsVisible = false;

            Application.Current.Properties["username"] = UserEntry.Text;
            Application.Current.Properties["password"] = PassEntry.Text;
            Application.Current.Properties["school"] = school.name;
            Application.Current.Properties["schoolurl"] = school.loginUrl;

            await label1.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await UserEntry.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await PassEntry.FadeTo(0, App.AnimationSpeed, Easing.SinIn);

            await Application.Current.SavePropertiesAsync();
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
