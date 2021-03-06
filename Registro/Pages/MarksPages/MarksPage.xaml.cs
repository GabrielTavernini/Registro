﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MarcTron.Plugin.Controls;
using Registro.Classes.JsonRequest;
using Registro.Controls;
using Registro.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Registro.Controls.AndroidThemes;

namespace Registro.Pages
{
    public partial class MarksPage : ContentPage
    {
        void CreateAd()
        {
            if (App.AdsAvailable)
            {
                MTAdView adView = new MTAdView();
                adView.AdsId = "ca-app-pub-4070857653436842/8461173324";
                adView.PersonalizedAds = true;
                adView.BackgroundColor = Color.LightGray;
                adView.HeightRequest = 50;
                adView.AdsFailedToLoad += (s, e) => {
                    adView.ScaleTo(0);
                };
                MainGrid.Children.Add(adView, 0, 1);
            }
        }

        public MarksPage()
        {
            GC.Collect();
            InitializeComponent();
            CreateAd();

            NavigationPage.SetHasNavigationBar(this, false);       //Page saetup


            if (DateTime.Now.CompareTo(App.Settings.periodChange) <= 0)
            {
                Selector2.BackgroundColor = Color.FromHex("#00B1D4");
                Selector1.BackgroundColor = Color.FromHex("#0082D4");
                InfoList.Scale = 1;
                InfoList2.Scale = 0;
                InfoList2.IsVisible = false;
                InfoList.ItemsSource = GetItems1();
                InfoList2.ItemsSource = GetItems2();
            }
            else
            {
                Selector1.BackgroundColor = Color.FromHex("#00B1D4");
                Selector2.BackgroundColor = Color.FromHex("#0082D4");
                InfoList.Scale = 0;
                InfoList2.Scale = 1;
                InfoList.IsVisible = false;
                InfoList.ItemsSource = GetItems1();
                InfoList2.ItemsSource = GetItems2();
            }

            MenuGrid.HeightRequest = App.ScreenHeight * 0.08;
            Head.HeightRequest = App.ScreenHeight * 0.08;
            MainImage.HeightRequest = App.ScreenWidth;
            Body.HeightRequest = App.ScreenHeight - Head.HeightRequest;

            if (App.multipleUsers)
            {
                String nameText = JsonRequest.user.username;
                if (!String.IsNullOrWhiteSpace(JsonRequest.user.name))
                    nameText = JsonRequest.user.name + (String.IsNullOrWhiteSpace(JsonRequest.user.surname) ? "" : " " + JsonRequest.user.surname.Substring(0, 1) + ".");
                NameLabel.Text = nameText;
            }

            gesturesSetup();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Setting.Margin = new Thickness(0, 25, 10, 0);
                Back.Margin = new Thickness(0, 25, 0, 0);
                MenuGrid.Margin = new Thickness(50, 10, 50, 0);
            }
            else
            {
                Setting.Margin = new Thickness(0, 32, 0, 0);
                Back.Margin = new Thickness(0, 32, 0, 0);
                MenuGrid.Margin = new Thickness(50, 24, 50, 0);
            }
        }

        public MarksPage(int period)
        {
            GC.Collect();
            InitializeComponent();
            CreateAd();

            NavigationPage.SetHasNavigationBar(this, false);

            if (period == 1)
            {
                Selector2.BackgroundColor = Color.FromHex("#00B1D4");
                Selector1.BackgroundColor = Color.FromHex("#0082D4");
                InfoList.Scale = 1;
                InfoList2.Scale = 0;
                InfoList2.IsVisible = false;
                InfoList.ItemsSource = GetItems1();
                InfoList2.ItemsSource = GetItems2();
            }
            else
            {
                Selector1.BackgroundColor = Color.FromHex("#00B1D4");
                Selector2.BackgroundColor = Color.FromHex("#0082D4");
                InfoList.Scale = 0;
                InfoList2.Scale = 1;
                InfoList.IsVisible = false;
                InfoList.ItemsSource = GetItems1();
                InfoList2.ItemsSource = GetItems2();
            }

            MenuGrid.HeightRequest = App.ScreenHeight * 0.08;
            Head.HeightRequest = App.ScreenHeight * 0.08;
            MainImage.HeightRequest = App.ScreenWidth;
            Body.HeightRequest = App.ScreenHeight - Head.HeightRequest;

            if (App.multipleUsers)
            {
                String nameText = JsonRequest.user.username;
                if (!String.IsNullOrWhiteSpace(JsonRequest.user.name))
                    nameText = JsonRequest.user.name + (String.IsNullOrWhiteSpace(JsonRequest.user.surname) ? "" : " " + JsonRequest.user.surname.Substring(0, 1) + ".");
                NameLabel.Text = nameText;
            }

            gesturesSetup();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Setting.Margin = new Thickness(0, 25, 10, 0);
                Back.Margin = new Thickness(10, 30, 0, 0);
                MenuGrid.Margin = new Thickness(50, 10, 50, 0);
            }
            else
            {
                Setting.Margin = new Thickness(0, 32, 0, 0);
                Back.Margin = new Thickness(0, 32, 0, 0);
                MenuGrid.Margin = new Thickness(50, 24, 50, 0);
            }
        }

        protected override void OnAppearing()
        {
            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<IThemes>().setMarksTheme();  //Android Themes

            base.OnAppearing();
        }

        #region setup
        public void gesturesSetup()
        {
            InfoList.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
            InfoList.Refreshing += (sender, e) => { Refresh(); };
            InfoList.ItemTapped += (sender, e) => { ItemTapped(e); };

            InfoList2.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
            InfoList2.Refreshing += (sender, e) => { Refresh(); };
            InfoList2.ItemTapped += (sender, e) => { ItemTapped(e); };

            var settingTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            settingTapGesture.Tapped += (sender, args) => { Navigation.PushAsync(new SettingsPage()); };
            Setting.GestureRecognizers.Add(settingTapGesture);

            var backTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            backTapGesture.Tapped += (sender, args) => { Navigation.PopAsync(); };
            Back.GestureRecognizers.Add(backTapGesture);

            var secondPeriodGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            secondPeriodGesture.Tapped += (sender, args) =>
            {
                Selector1.BackgroundColor = Color.FromHex("#00B1D4");
                Selector2.BackgroundColor = Color.FromHex("#0082D4");
                InfoList.Scale = 0;
                InfoList.IsVisible = false;
                InfoList2.Scale = 1;
                InfoList2.IsVisible = true;
            };
            Selector2.GestureRecognizers.Add(secondPeriodGesture);

            var firstPeriodGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            firstPeriodGesture.Tapped += (sender, args) =>
            {
                Selector1.BackgroundColor = Color.FromHex("#0082D4");
                Selector2.BackgroundColor = Color.FromHex("#00B1D4");
                InfoList.Scale = 1;
                InfoList.IsVisible = true;
                InfoList2.Scale = 0;
                InfoList2.IsVisible = false;
            };
            Selector1.GestureRecognizers.Add(firstPeriodGesture);
        }

        public void settings()
        {
            Navigation.PopAsync();
        }

        private void ItemTapped(ItemTappedEventArgs e)
        {
            GradeModel g = e.Item as GradeModel;
            if (g.Description == null || g.Description == "")
                DisplayAlert("Descrizione Voto", "Nessuna Descrizione", "Ok");
            else
                DisplayAlert("Descrizione Voto", g.Description, "Ok");
        }

        private void Refresh()
        {
            InfoList.IsRefreshing = true;
            InfoList2.IsRefreshing = true;
            Boolean success = true;

            Task.Run(async () => success = await JsonRequest.JsonLogin())//await new MarksRequests().refreshMarks())
            .ContinueWith((end) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        InfoList.IsRefreshing = false;
                        InfoList2.IsRefreshing = false;

                        if (success)
                        {
                            ContentPage page;
                            if (InfoList2.IsVisible)
                                page = new MarksPage(2);
                            else
                                page = new MarksPage(1);

                            Navigation.InsertPageBefore(page, this);
                            Navigation.PopAsync(false);
                        }
                    }
                    catch { }

                });
            });
        }
        #endregion

        #region MoveList

        private bool IsUpper = false;

        /// <summary>
        /// OnTouch screen choose animation - move Up or Down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void LayoutTouchListner_OnTouchEvent(object sender, EventArgs eventArgs)
        {
            var a = eventArgs as EvArg;

            LayoutTouchListnerCtrl.IsEnebleScroll = true;
            System.Diagnostics.Debug.WriteLine("ddddddddddd ---> " + App.ScreenHeight);

            // ignore the weak touch
            if (a.Val > 10 || a.Val < -10)
            {
                if (a.Val > 0)
                {
                    if (IsUpper)
                    {
                        MoveDown();
                    }
                }
                else
                {
                    MoveUp();
                }
            }
        }

        /// <summary>
        /// First item Appearing => animate MoveDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchPageViewCellWithId_OnFirstApper(object sender, EventArgs e)
        {
            IsUpper = false;
            MoveDown();
        }

        /// <summary>
        /// First item Disappearing => animate MoveUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchPageViewCellWithId_OnFirstDisapp(object sender, EventArgs e)
        {
            IsUpper = true;
            MoveUp();
        }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        private async void MoveDown()
        {
            DoubleUp.IsVisible = true;
            MenuGrid.TranslateTo(0, App.multipleUsers ? 85 : 100, 250, Easing.Linear);
            DoubleUp.TranslateTo(0, 180, 250, Easing.Linear);
            TitleLabel.ScaleTo(2, 250, Easing.Linear);
            if (App.multipleUsers)
            {
                NameLabel.IsVisible = true;
                NameLabel.TranslateTo(0, 110, 250, Easing.Linear);
            }

            await Body.TranslateTo(0, 200, 250, Easing.Linear);

            if (Device.RuntimePlatform == Device.iOS)
                Body.HeightRequest = App.ScreenHeight - (200 + (App.ScreenHeight * 0.08));
        }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        private void MoveUp()
        {
            if (Device.RuntimePlatform == Device.iOS)
                Body.HeightRequest = App.ScreenHeight - (App.ScreenHeight * 0.08);
            Body.TranslateTo(0, 0, 250, Easing.Linear);
            MenuGrid.TranslateTo(0, 0, 250, Easing.Linear);
            DoubleUp.TranslateTo(0, 0, 250, Easing.Linear);
            TitleLabel.ScaleTo(1, 250, Easing.Linear);
            DoubleUp.IsVisible = false;

            NameLabel.TranslateTo(0, 25, 250, Easing.Linear);
            NameLabel.IsVisible = false;
        }

        #endregion

        #region items

        private List<GradeModel> GetItems1()
        {

            List<GradeModel> list = new List<GradeModel>();

            foreach (Grade g in App.Grades)
            {
                if (g.dateTime.CompareTo(App.Settings.periodChange) <= 0)
                    list.Add(new GradeModel(g, 1));
            }
            list.Sort(new CustomDataTimeComparer());


            int j = 1;
            foreach (GradeModel g in list)
            {
                g.Id = j;
                g.color = Color.FromHex("#00B1D4");
                j++;
            }

            if (list.Count > 0)
                return list;

            list.Clear();
            GradeModel nope = new GradeModel(
                new Grade("", "Non ci sono voti per questo periodo", "", "Non ci sono voti per questo periodo",
                          new Subject("NESSUN VOTO")), 1, Color.FromHex("#00B1D4"));
            nope.gradeString = "N";
            list.Add(nope);
            return list;
        }

        private List<GradeModel> GetItems2()
        {
            List<GradeModel> list = new List<GradeModel>();

            foreach (Grade g in App.Grades)
            {
                if (g.dateTime.CompareTo(App.Settings.periodChange) > 0)
                    list.Add(new GradeModel(g, 1));
            }
            list.Sort(new CustomDataTimeComparer());

            int j = 1;
            foreach (GradeModel g in list)
            {
                g.Id = j;
                g.color = Color.FromHex("#00B1D4");
                j++;
            }

            if (list.Count > 0)
                return list;

            list.Clear();
            GradeModel nope = new GradeModel(
                new Grade("", "Non ci sono voti per questo periodo", "", "Non ci sono voti per questo periodo",
                          new Subject("NESSUN VOTO")), 1, Color.FromHex("#00B1D4"));
            nope.gradeString = "N";
            list.Add(nope);
            return list;
        }
        #endregion
    }



    public class CustomDataTimeComparer : IComparer<GradeModel>
    {
        public int Compare(GradeModel x, GradeModel y)
        {
            return -DateTime.Compare(x.dateTime, y.dateTime);
        }
    }
}
