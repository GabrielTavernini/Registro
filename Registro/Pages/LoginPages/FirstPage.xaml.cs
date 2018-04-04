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
            await btnAuthenticate.FadeTo(0, App.AnimationSpeed, Easing.SinIn);
            btnAuthenticate.IsVisible = false;

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LodingLabel.IsVisible = true;

            if (App.Schools.Count < 1)
            {
                /*if (!await SchoolRequests.extractAllSchoolsAsync())
                {
                    System.Diagnostics.Debug.WriteLine("Connection Error!");
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                    LodingLabel.IsVisible = false;

                    btnAuthenticate.IsVisible = true;
                    await btnAuthenticate.FadeTo(1, App.AnimationSpeed, Easing.SinIn);
                    return;
                }
                System.Diagnostics.Debug.WriteLine(App.Schools.Count);*/
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
            Initialize();

            await Task.Delay(App.DelaySpeed);
            await label1.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await label2.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
            await buttonStack.ScaleTo(1, App.AnimationSpeed, Easing.SinIn);
        }

        protected override bool OnBackButtonPressed()
        {
            if (Device.RuntimePlatform == Device.Android)
                return DependencyService.Get<IClose>().CloseApp();

            return base.OnBackButtonPressed();
        }
    }
}
