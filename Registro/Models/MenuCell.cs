using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Registro.Models
{
    public class MenuCell : ViewCell
    {

        public MenuCell()
        {
            Frame mainF;
            RelativeLayout rl = new RelativeLayout() { Scale = 0, BackgroundColor = Color.Transparent};

            if (Device.RuntimePlatform == Device.Android)
            {
                Frame shadow = new Frame() { CornerRadius = 8, BackgroundColor = Color.Gray, Margin = new Thickness(10,0,10,0) };
                    rl.Children.Add(shadow, Constraint.RelativeToParent((parent) =>
                    {
                        return parent.X + 4;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Y + 4;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width - 4;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Height - 14;
                    }));

                mainF = new Frame() { CornerRadius = 8, Margin = new Thickness(10, 0, 10, 0) };
                    rl.Children.Add(mainF, Constraint.RelativeToParent((parent) =>
                    {
                        return parent.X;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Y;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width - 4;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Height - 14;
                    }));
            }
            else
            {
                mainF = new Frame() { CornerRadius = 8, Margin = new Thickness(10, 0, 10, 0) };
                    rl.Children.Add(mainF, Constraint.RelativeToParent((parent) =>
                    {
                        return parent.X;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Y;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    }), Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Height - 10;
                    }));
            }

            Label title = new Label() 
            { 
                TextColor = Color.White, 
                FontSize = 32,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center, 
                HorizontalOptions = LayoutOptions.Center, 
                VerticalTextAlignment = TextAlignment.Center, 
                HorizontalTextAlignment = TextAlignment.Center 
            };
            Image icon = new Image() 
            { 
                HeightRequest = 75, 
                WidthRequest = 75, 
                VerticalOptions = LayoutOptions.Center, 
                HorizontalOptions = LayoutOptions.Center 
            };

            Grid mainG = new Grid()
            {
                BackgroundColor = Color.Transparent,
                ColumnDefinitions = {
                    new ColumnDefinition{Width = new GridLength(2, GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(4, GridUnitType.Star)},
                }
            };
             
            mainG.Children.Add(icon, 0, 0);
            mainG.Children.Add(title, 1, 0);

            mainF.Content = mainG;

            //View.BackgroundColor = Color.Transparent;
            View = rl;

            //-----------------Bindings-----------------

            this.SetBinding(ItemIdProperty, nameof(MenuOption.Id));
            mainF.SetBinding(Frame.BackgroundColorProperty, nameof(MenuOption.color));
            title.SetBinding(Label.TextProperty, nameof(MenuOption.title));
            icon.SetBinding(Image.SourceProperty, nameof(MenuOption.image));
            rl.SetBinding(RelativeLayout.HeightRequestProperty, nameof(MenuOption.Height));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                await Task.Delay(100);
                await View.ScaleTo(1, 125, Easing.SpringOut);
            }
            catch { } 

            System.Diagnostics.Debug.WriteLine("Appear " + ItemId);

            if (ItemId == 1 || ItemId == 2 || ItemId == 3)
            {
                DoFirstApper();
            }
        }

        protected override async void OnTapped()
        {
            base.OnTapped();

            try
            {
                await View.ScaleTo(1.04, 50, Easing.SpringOut);
                await View.ScaleTo(1, 50);                
            }
            catch{}
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            System.Diagnostics.Debug.WriteLine("Disappear " + ItemId);

            if (ItemId == 1)
            {
                DoFirstDisapp();
            }
        }

        public event EventHandler FirstDisapp;
        public void DoFirstDisapp()
        {
            EventHandler eh = FirstDisapp;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }


        public event EventHandler FirstApper;
        public void DoFirstApper()
        {
            EventHandler eh = FirstApper;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }


        public static readonly BindableProperty ItemIdProperty =
            BindableProperty.Create("ItemId", typeof(Int32),
                                    typeof(MenuCell), 0, BindingMode.TwoWay, null, null);

        public int ItemId
        {
            get { return (Int32)GetValue(ItemIdProperty); }
            set { SetValue(ItemIdProperty, value); }
        }
    }
}
