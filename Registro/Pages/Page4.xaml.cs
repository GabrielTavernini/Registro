using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registro.Controls;
using Registro.Models;
using SwipeLib;
using Xamarin.Forms;

namespace Registro.Pages
{
    /// <summary>
    /// working on ios and android
    /// </summary>
    public partial class Page4 : ContentPage
    {
        public Page4()
        {
            InitializeComponent();

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            InfoList.ItemsSource = GetItems();

            MenuGrid.HeightRequest = App.ScreenHeight * 0.08;
            Head.HeightRequest = App.ScreenHeight * 0.08;
            Body.HeightRequest = App.ScreenHeight - Head.HeightRequest;



            InfoList.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
            InfoList.Refreshing += async (sender, e) => { await RefreshAsync(InfoList); };
            InfoList.ItemTapped += (sender, e) => { ItemTapped(e); };


            var tgr = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            tgr.Tapped += (sender, args) => { settings(); };
            Setting.GestureRecognizers.Add(tgr);

            if (Device.RuntimePlatform == Device.iOS)
                Setting.Margin = new Thickness(0, 20, 0, 0);
            
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            //PrepareAnimate();
        }

        public void settings()
        {
            //Navigation.PushAsync(new HomePage());
        }

        private void ItemTapped(ItemTappedEventArgs e)
        {
            
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
            IsUpper = true;
            MoveDown();
        }

        /// <summary>
        /// First item Disappearing => animate MoveUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchPageViewCellWithId_OnFirstDisapp(object sender, EventArgs e)
        {
            IsUpper = false;
            MoveUp();
        }

        private void MoveDown()
        {
            Body.TranslateTo(0, 200, 250, Easing.Linear);
            MenuGrid.TranslateTo(0, 100, 250, Easing.Linear);
            TitleLabel.ScaleTo(2, 250, Easing.Linear);
        }

        private void MoveUp()
        {
            Body.TranslateTo(0, 0, 250, Easing.Linear);
            MenuGrid.TranslateTo(0, 0, 250, Easing.Linear);
            TitleLabel.ScaleTo(1, 250, Easing.Linear);
        }

        #endregion



        /// <summary>
        /// Fake items for listView
        /// </summary>
        /// <returns></returns>
        private List<MenuOption> GetItems()
        {
            var list = new List<MenuOption>();

            for (int i = 1; i < 21; i++)
            {
                list.Add(new MenuOption("Voti", ImageSource.FromFile("HomeIcon.png"), Color.Orange, i));
            }
            return list;
        }
    }
}
