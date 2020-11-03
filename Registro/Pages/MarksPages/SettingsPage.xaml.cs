using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using Registro.Classes.JsonRequest;
using Registro.Controls;
using Registro.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using XFGloss;
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
        CustomSwitchCell coloredMarks;
        CustomExitCell exitCell;

        public SettingsPage()
        {
            GC.Collect();
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

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
            switchesSetup();
            creditsSetup();


			User.Text = String.Format("Utente Attuale: {0}", JsonRequest.user.username); //.nome);

            exitCell = new CustomExitCell();
            exitCell.Tapped += async (sender, e) => { await TappedExitAsync(); };
            LoginSection.Add(exitCell);

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


        protected override void OnAppearing()
        {
            if (Device.RuntimePlatform == Device.Android)
                DependencyService.Get<IThemes>().setSettingsTheme();  //Android Themes

            base.OnAppearing();
        }

        void DataChanged(object sender, System.EventArgs e)
        {
            DateChangedEventArgs args = e as DateChangedEventArgs;
            App.Settings.periodChange = args.NewDate;
            App.Settings.customPeriodChange = true;
            Application.Current.Properties["settings"] = JsonConvert.SerializeObject(App.Settings, Formatting.Indented);
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

            if (sender == coloredMarks)
                App.Settings.coloredMarks = args.Value;


            Application.Current.Properties["settings"] = JsonConvert.SerializeObject(App.Settings, Formatting.Indented);
        }

        async Task TappedExitAsync()
        {
            var answer = await DisplayAlert(
                            "Logout",
                            "Sei sicuro di voler uscire dall'applicazione? Se uscirai per poter riaccedere dovrai reinserire i tuoi dati d'accesso.",
                            "Esci",
                            "Annulla");

            if (answer)
            {
                if (Application.Current.Properties.ContainsKey("userbackups"))
                {
                    String str = Application.Current.Properties["userbackups"] as String;
                    Dictionary<String, UserBackUp>  userBackUps = JsonConvert.DeserializeObject<Dictionary<String, UserBackUp>>(str);

                    if(userBackUps.Count > 1){
                        userBackUps.Remove(Application.Current.Properties["username"].ToString());
                        JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                        Application.Current.Properties["userbackups"] = JsonConvert.SerializeObject(userBackUps, Formatting.Indented, jsonSettings);
                        App.multipleUsers = userBackUps.Count > 1;

                        //Set the current user as the first of the dictionary
                        UserBackUp newUser = userBackUps.First().Value;
                        Application.Current.Properties["username"] = newUser.username;
                        Application.Current.Properties["password"] = newUser.password;
                        Application.Current.Properties["schoolurl"] = newUser.schoolUrl;
                        Application.Current.Properties["school"] = newUser.schoolName;

                        JsonRequest.user.username = newUser.username;
                        JsonRequest.user.password = newUser.password;
                        JsonRequest.user.school = new School(newUser.schoolUrl, newUser.schoolName);

                        //Loading View
                        Frame f = new Frame
                        {
                            BackgroundColor = Color.Transparent,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            Content = new ActivityIndicator { IsRunning = true, Color = Color.White, Scale = 0.30 }
                        };

                        var bkgrndGradient = new Gradient()
                        {
                            Rotation = 150,
                            Steps = new GradientStepCollection()
                            {
                                new GradientStep(Color.FromHex("#ed80ce"), 0),
                                new GradientStep(Color.FromHex("#7e5be1"), 1)
                            }
                        };
                        ContentPageGloss.SetBackgroundGradient(this, bkgrndGradient);

                        this.Content = f;
                        await JsonRequest.JsonLogin();
                        await Navigation.PopAsync();
                        return; //Exit the method
                    }
                }

                //If no multipleUsers or just one... delete and login
                Application.Current.Properties["username"] = null;
                Application.Current.Properties.Clear();
                App.Arguments.Clear();
                App.Absences.Clear();
                App.Grades.Clear();
                App.Notes.Clear();
                App.Subjects.Clear();
                //HttpRequest.User = null;
                JsonRequest.user = null;
                JsonRequest.lastRequest = new DateTime();
                App.Settings = new Settings();

                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<INotifyAndroid>().StopAlarm();
                await Navigation.PushAsync(new FirstPage());
            }
                
        }


        public async void Purchase1(object sender, EventArgs e) { await PurchaseItem("registrolampschooldonation1", "registrolampschooldonation1"); }
        public async void Purchase2(object sender, EventArgs e) { await PurchaseItem("registrolampschooldonation2", "registrolampschooldonation2"); }
        public async void Purchase3(object sender, EventArgs e) { await PurchaseItem("registrolampschooldonation3", "registrolampschooldonation3"); }
        public async void Purchase4(object sender, EventArgs e) { await PurchaseItem("registrolampschooldonation4", "registrolampschooldonation4"); }
        public async void DonationInfoTapped(object sender, EventArgs e) { await DisplayAlert("Informazioni sulle Donazioni", "Donando una qualsiasi quota, oltre a consentirmi di continuare ad aggiornare e mantenere disponibile l'app, tutti i banner pubblicitari presenti verranno rimossi!", "OK"); }

        public async Task<bool> PurchaseItem(string productId, string payload)
        {
            var billing = CrossInAppBilling.Current;
            try
            {
                var connected = await billing.ConnectAsync(ItemType.InAppPurchase);
                if (!connected) //we are offline or can't connect, don't try to purchase
                    return false;

                //check purchases
                var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchase, payload);

                //possibility that a null came through.
                if (purchase == null)
                    return false;
                else if (purchase.State == PurchaseState.Purchased)
                {
                    await DisplayAlert("Operazione Completata", "Grazie mille per la sua donazione, è grazie ad esse che posso continuare a mantenere l'app aggiornata e disponibile. Da adesso le pubblicità non la disturberanno più!", "OK");
                    Application.Current.Properties["noAds"] = "true";
                    App.AdsAvailable = false;
                    return true;
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                //Billing Exception handle this based on the type
                Debug.WriteLine("Error: " + purchaseEx);
            }
            catch (Exception ex)
            {
                //Something else has gone wrong, log it
                Debug.WriteLine("Issue connecting: " + ex);
            }
            finally
            {
                await billing.DisconnectAsync();
            }
            return false;
        }




            //------------------------------------------------------------------------------------------
            //-------------------------------------Multiple Users---------------------------------------
            //------------------------------------------------------------------------------------------

        async void TappedChangeUserAsync(object sender, EventArgs e)
        {
            JsonRequest.lastRequest = new DateTime();
            Dictionary<String, UserBackUp> userBackUps = new Dictionary<String, UserBackUp>();
            List<String> array = new List<String>();

            if (Application.Current.Properties.ContainsKey("userbackups"))
            {
                String str = Application.Current.Properties["userbackups"] as String;
                userBackUps = JsonConvert.DeserializeObject<Dictionary<String, UserBackUp>>(str);

                foreach (String k in userBackUps.Keys)
                    if (k != Application.Current.Properties["username"].ToString())
                        if(!String.IsNullOrWhiteSpace(userBackUps[k].name) && !String.IsNullOrWhiteSpace(userBackUps[k].surname))
                            array.Add(k + " - " + userBackUps[k].name + " " + userBackUps[k].surname.Substring(0, 1) + ".");
                        else
                            array.Add(k);
            }

            var action = await DisplayActionSheet("Seleziona Utente", "Annulla", null, array.ToArray());
            if (action == "Annulla" || action == null)
                return;


            UserBackUp userBackUp = new UserBackUp();
            userBackUp.name = Application.Current.Properties["name"].ToString();
            userBackUp.surname = Application.Current.Properties["surname"].ToString();
            userBackUp.username = Application.Current.Properties["username"].ToString(); 
            userBackUp.password = Application.Current.Properties["password"].ToString();
            userBackUp.schoolUrl = Application.Current.Properties["schoolurl"].ToString();
            userBackUp.schoolName = Application.Current.Properties["school"].ToString();
            userBackUp.grades = App.Grades;
            userBackUp.absences = App.Absences;
            userBackUp.arguments = App.Arguments;
            userBackUp.earlyExits = App.EarlyExits;
            userBackUp.lateEntries = App.LateEntries;
            userBackUp.notes = App.Notes;
            userBackUp.settings = App.Settings;
            //userBackUp.subjects = App.Subjects;



            if (!userBackUps.ContainsKey(userBackUp.username))
                userBackUps.Add(userBackUp.username, userBackUp);
            else
                userBackUps[userBackUp.username] = userBackUp;

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            Application.Current.Properties["userbackups"] = JsonConvert.SerializeObject(userBackUps, Formatting.Indented, jsonSettings);


            UserBackUp newUser = userBackUps[action.Split(" -".ToCharArray())[0]];
            Application.Current.Properties["name"] = newUser.name;
            Application.Current.Properties["surname"] = newUser.surname;
            Application.Current.Properties["username"] = newUser.username;
            Application.Current.Properties["password"] = newUser.password;
            Application.Current.Properties["schoolurl"] = newUser.schoolUrl;
            Application.Current.Properties["school"] = newUser.schoolName;

            JsonRequest.user.username = newUser.username;
            JsonRequest.user.password = newUser.password;
            JsonRequest.user.school = new School(newUser.schoolUrl, newUser.schoolName);

            //Loading View
            Frame f = new Frame
            {
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = new ActivityIndicator { IsRunning = true, Color = Color.White, Scale = 0.30 }
            };

            var bkgrndGradient = new Gradient()
            {
                Rotation = 150,
                Steps = new GradientStepCollection()
                {
                    new GradientStep(Color.FromHex("#ed80ce"), 0),
                    new GradientStep(Color.FromHex("#7e5be1"), 1)
                }
            };
            ContentPageGloss.SetBackgroundGradient(this, bkgrndGradient);

            this.Content = f;
            await JsonRequest.JsonLogin();
            await Navigation.PopAsync();
        }

        void TappedAddUser(object sender, EventArgs e)
        {
            JsonRequest.lastRequest = new DateTime();
            Dictionary<String, UserBackUp> userBackUps = new Dictionary<String, UserBackUp>();

            if (Application.Current.Properties.ContainsKey("userbackups"))
            {
                String str = Application.Current.Properties["userbackups"] as String;
                userBackUps = JsonConvert.DeserializeObject<Dictionary<String, UserBackUp>>(str);
            }

            UserBackUp userBackUp = new UserBackUp();
            userBackUp.name = Application.Current.Properties["name"].ToString();
            userBackUp.surname = Application.Current.Properties["surname"].ToString();
            userBackUp.username = Application.Current.Properties["username"].ToString();
            userBackUp.password = Application.Current.Properties["password"].ToString();
            userBackUp.schoolUrl = Application.Current.Properties["schoolurl"].ToString();
            userBackUp.schoolName = Application.Current.Properties["school"].ToString();
            userBackUp.grades = App.Grades;
            userBackUp.absences = App.Absences;
            userBackUp.arguments = App.Arguments;
            userBackUp.earlyExits = App.EarlyExits;
            userBackUp.lateEntries = App.LateEntries;
            userBackUp.notes = App.Notes;
            userBackUp.settings = App.Settings;
            //userBackUp.subjects = App.Subjects;



            if (!userBackUps.ContainsKey(userBackUp.username))//.name))
                userBackUps.Add(userBackUp.username, userBackUp);
            else
                userBackUps[userBackUp.username] = userBackUp;

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            Application.Current.Properties["userbackups"] = JsonConvert.SerializeObject(userBackUps, Formatting.Indented, jsonSettings);

            Navigation.PushAsync(new FirstPage(true)); //With back button
        }

        #region setup
        public void gesturesSetup()
        {
            var backTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            backTapGesture.Tapped += (sender, args) => { Navigation.PopAsync(); };
            Back.GestureRecognizers.Add(backTapGesture);
        }

        public void switchesSetup()
        {
            notifyMarks = new CustomSwitchCell("Notifiche per nuovi voti", App.Settings.notifyMarks);
            notifyMarks.Appearing += (sender, e) => { SearchPageViewCellWithId_OnFirstApper(sender, e); };
            notifyMarks.Disappearing += (sender, e) => { SearchPageViewCellWithId_OnFirstDisapp(sender, e); };

            notifyNotes = new CustomSwitchCell("Notifiche per nuove note", App.Settings.notifyNotes);
            notifyAbsences = new CustomSwitchCell("Notifiche per nuovi assenze", App.Settings.notifyAbsences);
            notifyArguments = new CustomSwitchCell("Notifiche per nuovi argomenti", App.Settings.notifyArguments);
            startupUpdate = new CustomSwitchCell("Aggiorna all'avvio", App.Settings.startupUpdate);
            coloredMarks = new CustomSwitchCell("Colore dinamico dei voti", App.Settings.coloredMarks);

            notifyMarks.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            notifyMarks.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            notifyNotes.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            notifyAbsences.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            notifyArguments.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            startupUpdate.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };
            coloredMarks.SwitchChanged += (sender, e) => { SwitchChanged(sender, e); };

            NotificationSection.Add(notifyMarks);
            NotificationSection.Add(notifyNotes);
            NotificationSection.Add(notifyAbsences);
            NotificationSection.Add(notifyArguments);

            GeneralSection.Add(startupUpdate);
            GeneralSection.Add(coloredMarks);
        }

        public void creditsSetup()
        {
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

            Info.Tapped += (sender, e) =>
            {
                Device.OpenUri(new Uri("https://github.com/GabrielTavernini/XFRegistro/wiki"));
            };

            Me.Tapped += (sender, e) =>
            {
                Device.OpenUri(new Uri("http://gabrieltavernini.github.io/"));
            };
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

    }
}
