using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Registro.UITest
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void AppLaunches()
        {
            app.Screenshot("First screen.");
            NewTest();
        }

        public void NewTest()
        {
            app.Tap(x => x.Marked("authButton"));
            app.Tap(x => x.Class("EditText"));
            app.ScrollDownTo("IC Valle dei Laghi Dro - SSPG Dro", withinMarked: "select_dialog_listview");
            app.Tap(x => x.Text("IC Valle dei Laghi Dro - SSPG Dro"));
            app.Tap(x => x.Text("CONTINUA"));
            app.Tap(x => x.Class("FormsEditText"));
            app.EnterText(x => x.Class("FormsEditText"), "gen220");
            app.Tap(x => x.Class("FormsEditText").Index(1));
            app.EnterText(x => x.Class("FormsEditText").Index(1), "2006stella");
            app.PressEnter();
            app.Tap(x => x.Text("LOGIN"));
            app.WaitForElement(x => x.Text("Voti"));
            app.TouchAndHold(x => x.Text("Voti"));
            app.Screenshot("Long press on view with class: FormsTextView");
        }
    }
}
