using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Registro.Classes.JsonRequest;
using Xamarin.Forms;
using static Registro.Controls.AndroidClosing;
using static Registro.Controls.AndroidThemes;

namespace Registro.Pages
{
    public partial class FirstPage : ContentPage
    {
        bool newUser = false;

        public FirstPage()
        {
            Initialize();
        }

        public FirstPage(bool newUser)
        {
            this.newUser = newUser;
            Initialize();
        }


        protected void Initialize()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            var backTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            backTapGesture.Tapped += (sender, args) => { Navigation.PopAsync(); };
            BackBtn.GestureRecognizers.Add(backTapGesture);

            label1.Scale = 0;
            label2.Scale = 0;
            buttonStack.Scale = 0;
            if (!newUser || Device.RuntimePlatform == Device.Android)
                BackBtn.Scale = 0;
        }

        async void AuthButtonClicked(object sender, EventArgs e)
        {
            await btnAuthenticate.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            btnAuthenticate.IsVisible = false;

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LodingLabel.IsVisible = true;

            if (App.Schools.Count < 1)
            {
                await SchoolsRequest.RequestSchools();
                System.Diagnostics.Debug.WriteLine(App.Schools.Count);
            }

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            LodingLabel.IsVisible = false;

            await label1.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            await label2.FadeTo(0, App.AnimationSpeed, Easing.SinIn);

            await Navigation.PushAsync(new SchoolPage());
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(App.DelaySpeed);
            await label1.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await label2.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await buttonStack.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
        }

        protected override bool OnBackButtonPressed()
        {
            if(newUser)
                Navigation.PopAsync();
            else if (Device.RuntimePlatform == Device.Android)
                return DependencyService.Get<IClose>().CloseApp();
            else
                return base.OnBackButtonPressed();

            return true;
        }
    }
}
