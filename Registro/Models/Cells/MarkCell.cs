﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFShapeView;

namespace Registro.Models
{
    public class MarkCell : ViewCell
    {
        public static readonly BindableProperty VoidProperty =
            BindableProperty.Create("Void", typeof(Boolean),
            typeof(MarkCell), false, BindingMode.TwoWay, null, null);

        public Boolean Void
        {
            get { return (Boolean)GetValue(VoidProperty); }
            set { SetValue(VoidProperty, value); }
        }

        public MarkCell()
        {
            Label subjectL = new Label()
            {
                TextColor = Color.DimGray,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                VerticalTextAlignment = TextAlignment.End,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(10, 10, 0, -5)
            };
            Label dateL = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                TextColor = Color.DimGray,
                FontSize = 14,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.End,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Label typeL = new Label()
            {
                Margin = new Thickness(10, -5, 0, 10),
                TextColor = Color.DimGray,
                FontSize = 16,
                VerticalTextAlignment = TextAlignment.Start,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Start
            };
            Label gradeL = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontSize = 18,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };



            var gradeF = new ShapeView()
            {
                ShapeType = ShapeType.Circle,
                HeightRequest = 40,
                WidthRequest = 40,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = gradeL,
                Margin = new Thickness(10, 0, 0, 0)
            };

            Grid mainG = new Grid()
            {
                Scale = 0,
                BackgroundColor = Color.Transparent,
                ColumnDefinitions = {
                    new ColumnDefinition{Width = GridLength.Auto},
                    new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{Width = GridLength.Auto}
                },

                RowDefinitions = {
                    new RowDefinition{Height = new GridLength(1, GridUnitType.Star)},
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
            View.BackgroundColor = Color.White;

            //-----------------Bindings-----------------
            this.SetBinding(VoidProperty, nameof(GradeModel.Void));
            this.SetBinding(ItemIdProperty, nameof(GradeModel.Id));
            gradeF.SetBinding(ShapeView.ColorProperty, nameof(GradeModel.color));
            gradeL.SetBinding(Label.TextProperty, nameof(GradeModel.gradeString));
            subjectL.SetBinding(Label.TextProperty, nameof(GradeModel.subject));
            typeL.SetBinding(Label.TextProperty, nameof(GradeModel.type));
            dateL.SetBinding(Label.TextProperty, nameof(GradeModel.date));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
                
            try
            {
                await Task.Delay(75);
                await View.ScaleTo(1, 75, Easing.SpringOut);               
            }
            catch{}


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

        protected override async void OnTapped()
        {
            base.OnTapped();

            try
            {
                await View.ScaleTo(1.2, 125);
                await View.ScaleTo(1, 125);               
            }
            catch{}

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
