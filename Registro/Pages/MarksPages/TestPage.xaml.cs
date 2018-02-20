using System;
using System.Collections.Generic;
using Registro.Models;
using Xamarin.Forms;
using XFGloss;
using XFShapeView;

namespace Registro.Pages
{
    public partial class TestPage : ContentPage
    {
        Color color;
        ShapeView thisCircle;
        int pageNumber;

        public TestPage(String page, Color color, int fontSize, int pageNumber)
        {
            this.color = color;
            this.pageNumber = pageNumber;

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            List<GradeModel> displayList = new List<GradeModel>();
            if(page == "Voti")
                displayList = GetItems();
            
            if (page == "Medie")
                displayList = GetItems2();
            


            InfoList.Header = GetCover(page, fontSize, displayList);
            InfoList.HeightRequest = App.ScreenHeight;
            InfoList.ItemsSource = displayList;
            

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            thisCircle.ScaleTo(1.3, 150, Easing.Linear);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            thisCircle.Scale = 1;
        }

        private List<GradeModel> GetItems()
        {
            List<GradeModel> list = new List<GradeModel>();

            foreach (Grade g in App.Grades)
            {
                list.Add(new GradeModel(g, 1, color));
            }
            list.Sort(new CustomDataTimeComparer());

            int i = 1;
            foreach (GradeModel g in list)
            {
                g.Id = i;
                i++;
            }


            return list;
        }

        private List<GradeModel> GetItems2()
        {
            List<GradeModel> list = new List<GradeModel>();

            float sum = 0;
            foreach (Grade g in App.Grades)
            {
                sum += g.grade;
            }

            Grade globalAverage = new Grade("", "Media globale dell'alunno", (sum / App.Grades.Count).ToString("0.00"), "", new Subject("MEDIA GLOBALE", false), false);
            list.Add(new GradeModel(globalAverage, list.Count + 1, color));

            foreach (Subject s in App.Subjects.Values)
            {
                Grade g = s.getMedia();
                g.type = "Media della materia";
                list.Add(new GradeModel(g, list.Count + 1, color));
            }

            return list;
        }

        private Frame GetCover(String page, int fontSize, List<GradeModel> displayList)
        {
            Grid g = new Grid() { Padding = 0 };

            /*
            Image settings = new Image()
            {
                Source = ImageSource.FromFile("settings_icon.png"),
                HeightRequest = 32,
                WidthRequest = 32,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
            };
            */

            StackLayout stack = new StackLayout()
            {
                Margin = new Thickness(0, 0, 0, 50),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Label title = new Label()
            {
                Text = page,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                FontSize = fontSize,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.End,
                HorizontalTextAlignment = TextAlignment.Center,
            };

            Label subtitle = new Label()
            {
                Text = "Hai " + displayList.Count + " " + page.ToLower(),
                FontAttributes = FontAttributes.Italic,
                BackgroundColor = Color.Transparent,
                TextColor = Color.FromHex("#e2e2e2"),
                FontSize = 18,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Start,
                HorizontalTextAlignment = TextAlignment.Center,
            };

            stack.Children.Add(title);
            stack.Children.Add(subtitle);

            StackLayout indicators = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(50),
            };

            thisCircle = getCircle();
            thisCircle.Color = Color.White;

            for (int i = 0; i < MainPage.pagesNumber; i++)
            {
                if (i + 1 == pageNumber)
                    indicators.Children.Add(thisCircle);
                else
                    indicators.Children.Add(getCircle());
            }

            g.Children.Add(stack);
            g.Children.Add(indicators);

            Frame cover = new Frame()
            {
                Content = g
            };

            cover.HeightRequest = App.ScreenHeight;
            cover.WidthRequest = App.ScreenWidth;
            cover.BackgroundColor = color;

            return cover;
        }

        public ShapeView getCircle()
        {
            var circle = new ShapeView()
            {
                HeightRequest = 10,
                WidthRequest = 10,
                Color = Color.LightGray,
                ShapeType = ShapeType.Circle
            };

            return circle;
        }
    }
}




