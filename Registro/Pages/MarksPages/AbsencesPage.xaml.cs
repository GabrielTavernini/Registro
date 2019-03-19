using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Registro.Classes;
using Registro.Classes.HttpRequests;
using Registro.Classes.JsonRequest;
using Registro.Controls;
using Registro.Models;
using Xamarin.Forms;
using Xamarin.Essentials;
using static Registro.Controls.AndroidClosing;
using static Registro.Controls.AndroidThemes;

namespace Registro.Pages
{
    public partial class AbsencesPage : ContentPage
    {
        public AbsencesPage()
        {
            GC.Collect();
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);


            if(DateTime.Now.CompareTo(App.Settings.periodChange) <= 0)
            {
                Selector2.BackgroundColor = Color.FromHex("#E15B5C");
                Selector1.BackgroundColor = Color.FromHex("#ad3233");
                InfoList.Scale = 1;
                InfoList2.Scale = 0;
                InfoList2.IsVisible = false;
                InfoList.ItemsSource = GetItems1();
                InfoList2.ItemsSource = GetItems2();                
            }else
            {
                Selector1.BackgroundColor = Color.FromHex("#E15B5C");
                Selector2.BackgroundColor = Color.FromHex("#ad3233");
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

            gesturesSetup();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Setting.Margin = new Thickness(0, 20, 0, 0);
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

        public AbsencesPage(int period)
        {
            GC.Collect();
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            if (period == 1)
            {
                Selector2.BackgroundColor = Color.FromHex("#E15B5C");
                Selector1.BackgroundColor = Color.FromHex("#ad3233");
                InfoList.Scale = 1;
                InfoList2.Scale = 0;
                InfoList2.IsVisible = false;
                InfoList.ItemsSource = GetItems1();
                InfoList2.ItemsSource = GetItems2();
            }
            else
            {
                Selector1.BackgroundColor = Color.FromHex("#E15B5C");
                Selector2.BackgroundColor = Color.FromHex("#ad3233");
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

            gesturesSetup();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Setting.Margin = new Thickness(0, 20, 0, 0);
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
                DependencyService.Get<IThemes>().setAbsencesTheme();  //Android Themes

            base.OnAppearing();


            //Hide add if there is no internet
            if (Connectivity.NetworkAccess != NetworkAccess.Internet || !App.AdsAvailable)
            {
                AdView.Scale = 0;
                AdView.IsVisible = false;
            }
                else {
            AdView.Scale = 1;
                AdView.IsVisible = true;
            }
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
                Selector1.BackgroundColor = Color.FromHex("#E15B5C");
                Selector2.BackgroundColor = Color.FromHex("#ad3233");
                InfoList.Scale = 0;
                InfoList.IsVisible = false;
                InfoList2.Scale = 1;
                InfoList2.IsVisible = true;
            };
            Selector2.GestureRecognizers.Add(secondPeriodGesture);

            var firstPeriodGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            firstPeriodGesture.Tapped += (sender, args) =>
            {
                Selector1.BackgroundColor = Color.FromHex("#ad3233");
                Selector2.BackgroundColor = Color.FromHex("#E15B5C");
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
            AbsenceModel g = e.Item as AbsenceModel;
            if (g.Type != "Nessuna Assenza")
            {
                DisplayAlert(g.Type, g.Preview, "Ok");
            }
                
        }

        private void Refresh()
        {
            InfoList.IsRefreshing = true;
            InfoList2.IsRefreshing = true;
            Boolean success = true;

            Task.Run(async () => success = await JsonRequest.JsonLogin())//await new AbsencesRequests().refreshAbsence())
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
                                    page = new AbsencesPage(2);
                                else
                                    page = new AbsencesPage(1);

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

        private async void MoveDown()
        {
            DoubleUp.IsVisible = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            MenuGrid.TranslateTo(0, 100, 250, Easing.Linear);
            DoubleUp.TranslateTo(0, 180, 250, Easing.Linear);
            TitleLabel.ScaleTo(2, 250, Easing.Linear);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await Body.TranslateTo(0, 200, 250, Easing.Linear);

            if (Device.RuntimePlatform == Device.iOS)
                Body.HeightRequest = App.ScreenHeight - (200 + (App.ScreenHeight * 0.08));
        }

        private void MoveUp()
        {
            if (Device.RuntimePlatform == Device.iOS)
                Body.HeightRequest = App.ScreenHeight - (App.ScreenHeight * 0.08);
            Body.TranslateTo(0, 0, 250, Easing.Linear);
            MenuGrid.TranslateTo(0, 0, 250, Easing.Linear);
            DoubleUp.TranslateTo(0, 0, 250, Easing.Linear);
            TitleLabel.ScaleTo(1, 250, Easing.Linear);
            DoubleUp.IsVisible = false;
        }

        #endregion

        private List<AbsenceModel> GetItems1()
        {
            List<AbsenceModel> list = new List<AbsenceModel>();
            foreach (Absence n in App.Absences)
            {
                if (n.dateTime.CompareTo(App.Settings.periodChange) <= 0)
                    list.Add(new AbsenceModel(n, 0));
            }

            foreach (LateEntry n in App.LateEntries)
            {
                if (n.dateTime.CompareTo(App.Settings.periodChange) <= 0)
                    list.Add(new AbsenceModel(n, 0));
            }

            foreach (EarlyExit n in App.EarlyExits)
            {
                if (n.dateTime.CompareTo(App.Settings.periodChange) <= 0)
                    list.Add(new AbsenceModel(n, 0));
            }
            list.Sort(new CustomDataTimeComparerAbsence());

            int j = 1;
            foreach (AbsenceModel n in list)
            {
                n.Id = j;
                n.color = Color.FromHex("#E15B5C");
                j++;
            }

            if (list.Count > 0)
                return list;

            Absence absence = new Absence(" ", "");
            AbsenceModel absenceModel = new AbsenceModel(absence, 1, Color.FromHex("#E15B5C"));
            absenceModel.Type = "Nessuna Assenza";
            absenceModel.FirstLetter = "N";
            absenceModel.Preview = "Lo studente non ha assenze!";
            list.Add(absenceModel);
            return list;
        }

        private List<AbsenceModel> GetItems2()
        {
            List<AbsenceModel> list = new List<AbsenceModel>();
            foreach (Absence n in App.Absences)
            {
                if (n.dateTime.CompareTo(App.Settings.periodChange) > 0)
                    list.Add(new AbsenceModel(n, 0));
            }

            foreach (LateEntry n in App.LateEntries)
            {
                if (n.dateTime.CompareTo(App.Settings.periodChange) > 0)
                    list.Add(new AbsenceModel(n, 0));
            }

            foreach (EarlyExit n in App.EarlyExits)
            {
                if (n.dateTime.CompareTo(App.Settings.periodChange) > 0)
                    list.Add(new AbsenceModel(n, 0));
            }
            list.Sort(new CustomDataTimeComparerAbsence());

            int j = 1;
            foreach (AbsenceModel n in list)
            {
                n.Id = j;
                n.color = Color.FromHex("#E15B5C");
                j++;
            }

            if (list.Count > 0)
                return list;

            Absence absence = new Absence(" ", "");
            AbsenceModel absenceModel = new AbsenceModel(absence, 1, Color.FromHex("#E15B5C"));
            absenceModel.Type = "Nessuna Assenza";
            absenceModel.FirstLetter = "N";
            absenceModel.Preview = "Lo studente non ha assenze!";
            list.Add(absenceModel);
            return list;
        }
    }

    public class CustomDataTimeComparerAbsence : IComparer<AbsenceModel>
    {
        public int Compare(AbsenceModel x, AbsenceModel y)
        {
            return -DateTime.Compare(x.dateTime, y.dateTime);
        }
    }
}
