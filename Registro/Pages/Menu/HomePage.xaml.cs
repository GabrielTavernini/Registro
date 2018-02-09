using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Registro.Controls;
using Registro.Models;
using Xamarin.Forms;

namespace Registro.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            MenuGrid.HeightRequest = App.ScreenHeight * 0.08;
            Head.HeightRequest = App.ScreenHeight * 0.08;
            Body.HeightRequest = App.ScreenHeight - Head.HeightRequest;

            InfoList.ItemSelected += async (sender, e) => { ((ListView)sender).SelectedItem = null; };
            InfoList.Refreshing += async (sender, e) => { await RefreshAsync(InfoList); };
            InfoList.ItemTapped += async (sender, e) => { ItemTapped(e); };


            var tgr = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            tgr.Tapped += (sender, args) => { settings(); };
            Setting.GestureRecognizers.Add(tgr);

            if (Device.RuntimePlatform == Device.iOS)
                Setting.Margin = new Thickness(0, 20, 0, 0);
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            InfoList.ItemsSource = GetItems();
        }

        public void settings()
        {
            //Navigation.PushAsync(new HomePage());
        }

        private async void ItemTapped(ItemTappedEventArgs e)
        {
            MenuOption mo = e.Item as MenuOption;

            await Task.Delay(100);
            if (mo.title == "Voti")
                await Navigation.PushAsync(new MarksPage());
            
            if (mo.title == "Medie")
                await Navigation.PushAsync(new AveragesPage());
            
        }

        private async Task RefreshAsync(ListView list)
        {
            await Task.Delay(2000);
            list.IsRefreshing = false;
        }







        //--------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------Animations--------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------------


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



        /// <summary>
        /// Fake items for listView
        /// </summary>
        /// <returns></returns>
        private List<MenuOption> GetItems()
        {
            var list = new List<MenuOption>();

            list.Add(new MenuOption("Voti", ImageSource.FromFile("VotiIcon.png"), Color.FromHex("#fd8469"), 1));
            list.Add(new MenuOption("Medie", ImageSource.FromFile("MedieIcon.png"), Color.FromHex("#324a5e"), 2));
            list.Add(new MenuOption("Argomenti", ImageSource.FromFile("ArgomentiIcon.png"), Color.FromHex("#90dfaa"), 3));
            list.Add(new MenuOption("Note", ImageSource.FromFile("NoteIcon.png"), Color.FromHex("#ffd05b"), 4));
            list.Add(new MenuOption("Assenze", ImageSource.FromFile("AssenzeIcon.png"), Color.FromHex("#4cdbc4"), 5));
            list.Add(new MenuOption("Cambia Password", ImageSource.FromFile("VotiIcon.png"), Color.FromHex("#84dbff"), 6));

            if (Device.RuntimePlatform == Device.iOS)
            {
                if (((150 * list.Count) - (235)) < App.ScreenHeight)
                {
                    double nd = (double)((App.ScreenHeight - ((150.0 * list.Count) - (235.0))) + 20.0) / 1.0;
                    int n = (int)Math.Ceiling(nd);
                    System.Diagnostics.Debug.WriteLine(nd);
                    System.Diagnostics.Debug.WriteLine(n);

                    for (int i = 0; i < n; i++)
                    {
                        MenuOption mo = new MenuOption("", ImageSource.FromFile(""), Color.Transparent, list.Count + 1);
                        mo.Height = 1;
                        list.Add(mo);
                    }
                }
            }

            return list;
        }
    }
}
