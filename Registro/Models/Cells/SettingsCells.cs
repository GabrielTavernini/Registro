using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Registro.Models
{
    public class DatePickerCell : ViewCell
    {
        public DatePickerCell()
        {
            StackLayout sl = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            Label l = new Label()
            {
                Text = "Fine primo periodo: ",
                TextColor = Color.Black,
                FontSize = 14,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(15, 0, 0, 0)
            };

            DatePicker dp = new DatePicker()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                Margin = new Thickness(0, 0, 0, 0),
                Date = App.periodChange,
                Scale = 0.9,
                Format = "dd/MM/yyyy"
            };

            dp.DateSelected += (sender, e) => { DataChanged(sender, e); };

            sl.Children.Add(l);
            sl.Children.Add(dp);

            View = sl;
        }


        public event EventHandler DataChanged;
    }

    public class CustomSwitchCell : ViewCell
    {
        public String Text { get; set; }
        public Switch sw;

        public CustomSwitchCell(String Text, Boolean On)
        {
            this.Text = Text;

            StackLayout sl = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            Label l = new Label()
            {
                Text = Text,
                TextColor = Color.Black,
                FontSize = 14,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(15, 0, 0, 0)
            };

            sw = new Switch()
            {
                IsToggled = On,
                Scale = 1.1,
                Margin = new Thickness(0, 0, 5, 0),
            };

            sw.Toggled += (sender, e) => { SwitchChanged(this, e); };

            sl.Children.Add(l);
            sl.Children.Add(sw);

            View = sl;
        }

        public void setSwitch(Boolean b)
        {
            sw.IsToggled = b;
        }

        public event EventHandler SwitchChanged;
    }

    public class CustomExitCell: ViewCell
    {
        public CustomExitCell()
        {
            Frame fr = new Frame()
            {
                Padding = 0,
                BackgroundColor = Color.Red,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 0
            };

            Label l = new Label()
            {
                Text = "ESCI",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            fr.Content = l;

            View = fr;
        }

        protected override async void OnTapped()
        {
            base.OnTapped();

            try
            {
                await View.ScaleTo(1.2, 175);
                await View.ScaleTo(1, 175);
            }
            catch { }

        }
    }

    public class Settings
    {
        public Boolean notifyMarks { get; set; } = true;
        public Boolean notifyNotes { get; set; } = true;
        public Boolean notifyAbsences { get; set; } = false;
        public Boolean startupUpdate { get; set; } = true;

        public Settings(Boolean notifyMarks, Boolean notifyNotes, Boolean notifyAbsences, Boolean startupUpdate)
        {
            this.notifyMarks = notifyMarks;
            this.notifyNotes = notifyNotes;
            this.notifyAbsences = notifyAbsences;
            this.startupUpdate = startupUpdate;
        }

        public Settings(){}
    }
}


