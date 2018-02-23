using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Registro.Controls;
using Registro.Models;
using Xamarin.Forms;

namespace Registro.Pages
{
    public partial class ArgumentsPage : ContentPage
    {
        public ArgumentsPage()
        {
            GC.Collect();
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            if(DateTime.Now.CompareTo(App.periodChange) <= 0)
            {
                Selector2.BackgroundColor = Color.FromHex("#B2D235");
                Selector1.BackgroundColor = Color.FromHex("#51d134");
                InfoList.Scale = 1;
                InfoList2.Scale = 0;
                InfoList2.IsVisible = false;
                InfoList.ItemsSource = GetItems1();
                InfoList2.ItemsSource = GetItems2();                
            }else
            {
                Selector1.BackgroundColor = Color.FromHex("#B2D235");
                Selector2.BackgroundColor = Color.FromHex("#51d134");
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
        }

        public ArgumentsPage(int period)
        {
            GC.Collect();
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            if (period == 1)
            {
                Selector2.BackgroundColor = Color.FromHex("#B2D235");
                Selector1.BackgroundColor = Color.FromHex("#51d134");
                InfoList.Scale = 1;
                InfoList2.Scale = 0;
                InfoList2.IsVisible = false;
                InfoList.ItemsSource = GetItems1();
                InfoList2.ItemsSource = GetItems2();
            }
            else
            {
                Selector1.BackgroundColor = Color.FromHex("#B2D235");
                Selector2.BackgroundColor = Color.FromHex("#51d134");
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
        }



        #region setup
        public void gesturesSetup()
        {
            InfoList.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
            InfoList.Refreshing += async (sender, e) => { await RefreshAsync(InfoList); };
            InfoList.ItemTapped += (sender, e) => { ItemTapped(e); };

            InfoList2.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
            InfoList2.Refreshing += async (sender, e) => { await RefreshAsync(InfoList); };
            InfoList2.ItemTapped += (sender, e) => { ItemTapped(e); };

            var settingTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            settingTapGesture.Tapped += (sender, args) => { };
            Setting.GestureRecognizers.Add(settingTapGesture);

            var backTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            backTapGesture.Tapped += (sender, args) => { Navigation.PopAsync(); };
            Back.GestureRecognizers.Add(backTapGesture);

            var secondPeriodGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            secondPeriodGesture.Tapped += (sender, args) =>
            {
                Selector1.BackgroundColor = Color.FromHex("#B2D235");
                Selector2.BackgroundColor = Color.FromHex("#51d134");
                InfoList.Scale = 0;
                InfoList.IsVisible = false;
                InfoList2.Scale = 1;
                InfoList2.IsVisible = true;
            };
            Selector2.GestureRecognizers.Add(secondPeriodGesture);

            var firstPeriodGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            firstPeriodGesture.Tapped += (sender, args) =>
            {
                Selector1.BackgroundColor = Color.FromHex("#51d134");
                Selector2.BackgroundColor = Color.FromHex("#B2D235");
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
            ArgsModel g = e.Item as ArgsModel;
            if (g.Argument == null || g.Argument == "")
                DisplayAlert("Argomento", "Nessuna Descrizione", "Ok");
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(g.Argument);
                if(g.Activity != null && g.Activity != "")
                {
                    sb.AppendLine(" ");
                    sb.AppendLine(" ");
                    sb.AppendLine("Attività:");
                    sb.Append(g.Activity);
                }

                DisplayAlert("Argomento", sb.ToString(), "Ok");  
            }
                
        }

        private async Task RefreshAsync(ListView list)
        {
            await ArgumentsRequests.refreshArguments();
            list.IsRefreshing = false;

            ContentPage page;
            if (InfoList2.IsVisible)
                page = new ArgumentsPage(2);
            else
                page = new ArgumentsPage(1);

            Navigation.InsertPageBefore(page, this);
            await Navigation.PopAsync();
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

        private void MoveDown()
        {
            DoubleUp.IsVisible = true;
            Body.TranslateTo(0, 200, 250, Easing.Linear);
            MenuGrid.TranslateTo(0, 100, 250, Easing.Linear);
            DoubleUp.TranslateTo(0, 180, 250, Easing.Linear);
            TitleLabel.ScaleTo(2, 250, Easing.Linear);
        }

        private void MoveUp()
        {
            Body.TranslateTo(0, 0, 250, Easing.Linear);
            MenuGrid.TranslateTo(0, 0, 250, Easing.Linear);
            DoubleUp.TranslateTo(0, 0, 250, Easing.Linear);
            TitleLabel.ScaleTo(1, 250, Easing.Linear);
            DoubleUp.IsVisible = false;

        }

        #endregion

        private List<ArgsModel> GetItems1()
        {
            List<ArgsModel> list = new List<ArgsModel>();
            foreach (Arguments a in App.Arguments)
            {
                if (a.dateTime.CompareTo(App.periodChange) <= 0)
                    list.Add(new ArgsModel(a, 0));
            }
            list.Sort(new CustomDataTimeComparerArgs());

            int j = 1;
            foreach (ArgsModel g in list)
            {
                g.Id = j;
                g.color = Color.FromHex("#B2D235");
                j++;
            }

            if (list.Count > 0)
                return list;

            Arguments arg = new Arguments("Non ci sono argomenti!", "", "", "Nessun Argomento", false);
            list.Add(new ArgsModel(arg, 1, Color.FromHex("#B2D235")));
            return list;
        }

        private List<ArgsModel> GetItems2()
        {
            List<ArgsModel> list = new List<ArgsModel>();
            foreach (Arguments a in App.Arguments)
            {
                if (a.dateTime.CompareTo(App.periodChange) > 0)
                    list.Add(new ArgsModel(a, 0));
            }
            list.Sort(new CustomDataTimeComparerArgs());

            int j = 1;
            foreach (ArgsModel g in list)
            {
                g.Id = j;
                g.color = Color.FromHex("#B2D235");
                j++;
            }

            if (list.Count > 0)
                return list;

            Arguments arg = new Arguments("Non ci sono argomenti!", "", "", "Nessun Argomento", false);
            list.Add(new ArgsModel(arg, 1, Color.FromHex("#B2D235")));
            return list;
        }
    }

    public class CustomDataTimeComparerArgs : IComparer<ArgsModel>
    {
        public int Compare(ArgsModel x, ArgsModel y)
        {
            return -DateTime.Compare(x.dateTime, y.dateTime);
        }
    }
}
