using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Registro.Pages
{
    public partial class FirstPage : ContentPage
    {
        public FirstPage()
        {
            Initialize();
        }

        protected void Initialize()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Title = "Welcome!";

            label1.Scale = 0;
            label2.Scale = 0;
            buttonStack.Scale = 0;
        }

        async void AuthButtonClicked(object sender, EventArgs e)
        {
            await label1.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await label2.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await buttonStack.FadeTo(0, App.AnimationSpeed, Easing.SinIn);

            await Navigation.PushAsync(new SchoolPage());
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();

            await Task.Delay(App.DelaySpeed);
            await label1.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await label2.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await buttonStack.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
        }
    }
}
