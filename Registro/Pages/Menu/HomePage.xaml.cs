using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Registro.Controls;
using Registro.Models;
using Xamarin.Forms;
using static Registro.Controls.AndroidClosing;

namespace Registro.Pages
{
    public partial class HomePage : ContentPage
    {
        User user;
        bool isFirstTime = false;
        Task t = new Task(async () => { await HttpRequest.RefreshAsync(); });

        public HomePage(User user)
        {
            this.user = user;
            HttpRequest.User = user;
            this.isFirstTime = true;
            initialize();
        }

        public HomePage()
        {
            initialize();
        }

        public void initialize()
        {
            InitializeComponent();

            InfoList.ItemsSource = GetItems();
            NavigationPage.SetHasNavigationBar(this, false);

            MenuGrid.HeightRequest = App.ScreenHeight * 0.08;
            Head.HeightRequest = App.ScreenHeight * 0.08;
            MainImage.HeightRequest = App.ScreenWidth;
            Body.HeightRequest = App.ScreenHeight - Head.HeightRequest;

            InfoList.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
            InfoList.Refreshing += async (sender, e) => { await RefreshAsync(InfoList); };
            InfoList.ItemTapped += async (sender, e) => { await ItemTappedAsync(e); };

            /*var settingTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            settingTapGesture.Tapped += (sender, args) => { settings(); };
            Setting.GestureRecognizers.Add(settingTapGesture);

            if (Device.RuntimePlatform == Device.iOS)
                Setting.Margin = new Thickness(0, 20, 0, 0); */
            
            if (Device.RuntimePlatform == Device.iOS)
                MenuGrid.Margin = new Thickness(50, 10, 50, 0);
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(isFirstTime)
            {
                isFirstTime = false;
                //t.Start();
            }
        }

        public void settings()
        {
            //Navigation.PushAsync(new HomePage());
        }

        private async Task ItemTappedAsync(ItemTappedEventArgs e)
        {
            MenuOption mo = e.Item as MenuOption;

            await Task.Delay(100);
            if (mo.title == "Voti")
                await Navigation.PushAsync(new MarksPage());

            if (mo.title == "Medie")
                await Navigation.PushAsync(new AveragesPage());

            if (mo.title == "Argomenti")
                await Navigation.PushAsync(new ArgumentsPage());

            if (mo.title == "Note")
                await Navigation.PushAsync(new NotesPage());

            if (mo.title == "Assenze")
                await Navigation.PushAsync(new AbsencesPage());
        }

        private async Task RefreshAsync(ListView list)
        {
            if ((t != null)
                && (t.IsCompleted != false ||
                t.Status != TaskStatus.Running ||
                t.Status != TaskStatus.WaitingToRun ||
                t.Status != TaskStatus.WaitingForActivation))
                await HttpRequest.RefreshAsync();

            list.IsRefreshing = false;
        }

        protected override bool OnBackButtonPressed()
        {
            if(Device.RuntimePlatform == Device.Android)
                return DependencyService.Get<IClose>().CloseApp();
            
            return base.OnBackButtonPressed();
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

            //LayoutTouchListnerCtrl.IsEnebleScroll = true;
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
            Body.TranslateTo(0, 200, 200, Easing.Linear);
            MenuGrid.TranslateTo(0, 100, 200, Easing.Linear);
            DoubleUp.TranslateTo(0, 180, 200, Easing.Linear);
            TitleLabel.ScaleTo(2, 125, Easing.Linear);
        }

        private void MoveUp()
        {
            Body.TranslateTo(0, 0, 200, Easing.Linear);
            MenuGrid.TranslateTo(0, 0, 200, Easing.Linear);
            DoubleUp.TranslateTo(0, 0, 200, Easing.Linear);
            TitleLabel.ScaleTo(1, 200, Easing.Linear);
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


            list.Add(MenuOption.VoidCell(list.Count + 1));

            list.Add(new MenuOption("Voti", ImageSource.FromFile("VotiIcon.png"), Color.FromHex("#00B1D4"), list.Count + 1));
            list.Add(new MenuOption("Medie", ImageSource.FromFile("MedieIcon.png"), Color.FromHex("#61DDDD"), list.Count + 1));
            list.Add(new MenuOption("Argomenti", ImageSource.FromFile("ArgomentiIcon.png"), Color.FromHex("#B2D235"), list.Count + 1));
            list.Add(new MenuOption("Note", ImageSource.FromFile("NoteIcon.png"), Color.FromHex("#F2AA52"), list.Count + 1));
            list.Add(new MenuOption("Assenze", ImageSource.FromFile("AssenzeIcon.png"), Color.FromHex("#E15B5C"), list.Count + 1));
            list.Add(new MenuOption("Impostazioni", ImageSource.FromFile("PasswordIcon.png"), Color.FromHex("#E15BBB"), list.Count + 1));


            return list;
        }
    }
}
