using System;
using Xamarin.Forms;
using XFShapeView;

namespace Registro.Models
{
    public class MarkCell : ViewCell
    {
        public MarkCell()
        {
            Label subjectL = new Label() { TextColor = Color.DimGray, FontAttributes = FontAttributes.Bold, FontSize = 24, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start };
            Label dateL = new Label() { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End, TextColor = Color.DimGray, FontSize = 12, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End };
            Label typeL = new Label() { TextColor = Color.DimGray, FontSize = 16, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start };
            Label gradeL = new Label() { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, FontAttributes = FontAttributes.Bold, FontSize = 32, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };

            var gradeF = new ShapeView()
            {
                Color = Color.DarkBlue,
                ShapeType = ShapeType.Circle,
                HeightRequest = 50,
                WidthRequest = 50,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = gradeL
            };

            Grid mainG = new Grid()
            {
                BackgroundColor = Color.Transparent,
                ColumnDefinitions = {
                    new ColumnDefinition{Width = GridLength.Auto},
                    new ColumnDefinition{Width = GridLength.Auto},
                    new ColumnDefinition{Width = GridLength.Auto},
                },

                RowDefinitions = {
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto}
                }
            };

            mainG.Children.Add(gradeF, 0, 0);
            Grid.SetRowSpan(gradeF, 2);

            mainG.Children.Add(subjectL, 1, 0);

            mainG.Children.Add(typeL, 1, 1);

            mainG.Children.Add(dateL, 2, 0);
            Grid.SetRowSpan(dateL, 2);


            View = mainG;
            //-----------------Bindings-----------------

            this.SetBinding(ItemIdProperty, nameof(Grade.Id));
            gradeL.SetBinding(Label.TextProperty, nameof(Grade.gradeString));
            subjectL.SetBinding(Label.TextProperty, nameof(Grade.subjectName));
            typeL.SetBinding(Label.TextProperty, nameof(Grade.type));
            dateL.SetBinding(Label.TextProperty, nameof(Grade.date));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ItemId == 1)
            {
                DoFirstApper();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

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
                                    typeof(MarkCell), 0, BindingMode.TwoWay, null, null);

        public int ItemId
        {
            get { return (Int32)GetValue(ItemIdProperty); }
            set { SetValue(ItemIdProperty, value); }
        }
    }
}
