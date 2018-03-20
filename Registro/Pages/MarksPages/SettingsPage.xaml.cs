using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Registro.Classes.HttpRequests;
using Registro.Controls;
using Registro.Models;
using Xamarin.Forms;
using static Registro.Controls.AndroidThemes;
using static Registro.Controls.Mails;
using static Registro.Controls.Notifications;

namespace Registro.Pages
{
    public partial class SettingsPage : ContentPage
    {
        
        CustomSwitchCell notifyMarks;
        CustomSwitchCell notifyNotes;
        CustomSwitchCell notifyAbsences;
        CustomSwitchCell notifyArguments;
        CustomSwitchCell startupUpdate;
        CustomExitCell exitCell;

        public SettingsPage()
        {
            GC.Collect();
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<IThemes>().setSettingsTheme();  //Android Themes
            

            MenuGrid.HeightRequest = App.ScreenHeight * 0.08;
            Head.HeightRequest = App.ScreenHeight * 0.08;
            MainImage.HeightRequest = App.ScreenWidth;
            Body.HeightRequest = App.ScreenHeight - Head.HeightRequest;

            gesturesSetup();


            notifyMarks = new CustomSwitchCell("Notifiche per nuovi voti", App.Settings.notifyMarks);
            notifyMarks.Appearing += (sender, e) => { SearchPageViewCellWithId_OnFirstApper(sender, e); };
            notifyMarks.Disappearing += (sender, e) => { SearchPageViewCellWithId_OnFirstDisapp(sender, e); };
            notifyMarks.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };

            notifyNotes = new CustomSwitchCell("Notifiche per nuove note", App.Settings.notifyNotes);
            notifyAbsences = new CustomSwitchCell("Notifiche per nuovi assenze", App.Settings.notifyAbsences);
            notifyArguments = new CustomSwitchCell("Notifiche per nuovi argomenti", App.Settings.notifyArguments);

            startupUpdate = new CustomSwitchCell("Aggiorna all'avvio", App.Settings.startupUpdate);

            notifyMarks.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            notifyNotes.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            notifyAbsences.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            notifyArguments.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            startupUpdate.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };

            NotificationSection.Add(notifyMarks);
            NotificationSection.Add(notifyNotes);
            NotificationSection.Add(notifyAbsences);
            NotificationSection.Add(notifyArguments);

            GeneralSection.Add(startupUpdate);

            exitCell = new CustomExitCell();
            exitCell.Tapped += (sender, e) => {TappedExitAsync();};
            LoginSection.Add(exitCell);


            Credits.Tapped += (sender, e) => 
            {
                DisplayAlert("Crediti", "Sviluppata utilizzando:\n+XFGloss\n+ModernHttpClient\n+Nexsoft.Json\n+Dcsoup\n+XFShapeView", "Ok");
            };

            Bug.Tapped += (sender, e) =>
            {
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<IMailAndroid>().SendEmail();
                else
                    DependencyService.Get<IMailiOS>().SendEmail();
            };

            if (Device.RuntimePlatform == Device.iOS)
            {
                Back.Margin = new Thickness(0, 25, 0, 0);
                MenuGrid.Margin = new Thickness(50, 10, 50, 0);
            }
            else
            {
                Back.Margin = new Thickness(0, 32, 0, 0);
                MenuGrid.Margin = new Thickness(50, 24, 50, 0);
            }
        }



        void DataChanged(object sender, System.EventArgs e)
        {
            DateChangedEventArgs args = e as DateChangedEventArgs;
            App.periodChange = args.NewDate;
            Application.Current.Properties["periodchange"] = JsonConvert.SerializeObject(App.periodChange, Formatting.Indented);
        }

        void SwitchChanged(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SwitchChanged");
            ToggledEventArgs args = e as ToggledEventArgs;

            if (sender == notifyMarks)
                App.Settings.notifyMarks = args.Value;
            
            if (sender == notifyNotes)
                App.Settings.notifyNotes = args.Value;
            
            if (sender == notifyAbsences)
                App.Settings.notifyAbsences = args.Value;

            if(sender == notifyArguments)
                App.Settings.notifyArguments = args.Value;
            
            if (sender == startupUpdate)
                App.Settings.startupUpdate = args.Value;
                

            Application.Current.Properties["settings"] = JsonConvert.SerializeObject(App.Settings, Formatting.Indented);
        }

        async void TappedExitAsync()
        {
            var answer = await DisplayAlert(
                "Logout",
                "Sei sicuro di voler uscire dall'applicazione? Se uscirai per poter riaccedere dovrai reinserire i tuoi dati d'accesso.",
                "Esci",
                "Annulla");
            
            if(answer)
            {
                Application.Current.Properties["username"] = null;
                Application.Current.Properties.Clear();
                App.Arguments.Clear();
                App.Absences.Clear();
                App.Grades.Clear();
                App.Notes.Clear();
                App.Subjects.Clear();
                HttpRequest.User = null;
                App.Settings = new Settings();
                App.notify = false;

                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().StopAlarm();
                else
                    DependencyService.Get<INotifyiOS>().StopAlarm();
                await Navigation.PushAsync(new FirstPage());  
            }
        }




        #region setup
        public void gesturesSetup()
        {
            var backTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            backTapGesture.Tapped += (sender, args) => { Navigation.PopAsync(); };
            Back.GestureRecognizers.Add(backTapGesture);
        }

        public void settings()
        {
            Navigation.PopAsync();
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

    }
}
