using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Registro.Pages
{
    public partial class SchoolPage : ContentPage
    {
        public SchoolPage()
        {
            Initialize();
        }

        protected void Initialize()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Title = "Scuola";

            label1.Scale = 0;
            SchoolPicker.Scale = 0;
            buttonStack.Scale = 0;

            if (Device.RuntimePlatform == Device.iOS)
            {
                SchoolPicker.BackgroundColor = Color.FromHex("#3FFF");
            }
        }


        async void AuthButtonClicked(object sender, EventArgs e)
        {
            await label1.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await SchoolPicker.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await buttonStack.FadeTo(0, App.AnimationSpeed, Easing.SinIn);

            await Navigation.PushAsync(new LoginPage((string) SchoolPicker.SelectedItem));
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();

            await Task.Delay(App.DelaySpeed);
            await label1.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await SchoolPicker.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await buttonStack.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
        }
    }
}
