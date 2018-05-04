using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Registro.Classes.HttpRequests;
using Registro.Controls;
using Xamarin.Forms;
using static Registro.Controls.AndroidThemes;
using static Registro.Controls.Mails;
using static Registro.Controls.Notifications;

namespace Registro.Pages
{
    public partial class SchoolUrlPage : ContentPage
    {
        public SchoolUrlPage()
        {
            Initialize();
        }

        protected void Initialize()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<IThemes>().setArgumentsTheme();  //Android Themes
            
            InitializeComponent();
            Title = "Scuola";

            label1.Scale = 0;
            PickerStack.Scale = 0;
            buttonStack.Scale = 0;
        }


        async void AuthButtonClicked(object sender, EventArgs e)
        {
            bool validUrl;

            try { new Uri(SchoolEntry.Text); validUrl = true; }
            catch { validUrl = false; }

            if(!validUrl)
            {
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().DisplayToast("Link non valido");
                else
                    DependencyService.Get<INotifyiOS>().ShowToast("Link non valido", 750);

                return;
            }
                

            if((string)SchoolEntry.Text != null)
            {
                await btnAuthenticate.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
                btnAuthenticate.IsVisible = false;

                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                LodingLabel.IsVisible = true;

                String page = await Utility.GetPageAsync(SchoolEntry.Text);
                System.Diagnostics.Debug.WriteLine(page);

                if(page.Contains("function codifica()"))
                {
                    await Utility.GetPageAsync("http://lampschooltest.altervista.org/newschool.php?link=" + SchoolEntry.Text);

                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                    LodingLabel.IsVisible = false;

                    await label1.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
                    await SchoolEntry.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
                    await buttonStack.FadeTo(0, App.AnimationSpeed, Easing.SinIn);

                    await Navigation.PushAsync(new LoginPage(new School(SchoolEntry.Text, "CustomSchool")));
                }
                else
                {

                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                    LodingLabel.IsVisible = false;

                    btnAuthenticate.IsVisible = true;
                    await btnAuthenticate.FadeTo(1, App.AnimationSpeed, Easing.SinIn);

                    bool wiki = !await DisplayAlert("Link Invalido", "Il link che ha inserito non e valido oppure non è quello di una pagina di login di un registro LAMPSchool",
                                 "Ok", "Istruzioni");

                    if(wiki)
                        Device.OpenUri(new Uri("https://github.com/GabrielTavernini/XFRegistro/wiki"));

                    return;
                }
                    
            }
            else
            {
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().DisplayToast("Inserici un link");
                else
                    DependencyService.Get<INotifyiOS>().ShowToast("Inserici un link", 750);
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();

            await Task.Delay(App.DelaySpeed);
            await label1.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await PickerStack.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await buttonStack.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
        }
    }
}
