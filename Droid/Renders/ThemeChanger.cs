using System;
using Android.Content;
using Android.Content.Res;
using Registro.Droid;
using static Registro.Controls.AndroidThemes;

[assembly: Xamarin.Forms.Dependency(typeof(ThemeChanger))]
namespace Registro.Droid
{
    public class ThemeChanger : IThemes
    {
        public void setMarksTheme()
        {
            MainActivity.Instance.SetTheme(Resource.Style.MarksTheme);
        }

        public void setAveragesTheme()
        {
            MainActivity.Instance.SetTheme(Resource.Style.AveragesTheme);
        }

        public void setAbsencesTheme()
        {
            MainActivity.Instance.SetTheme(Resource.Style.AbsencesTheme);
        }

        public void setArgumentsTheme()
        {
            MainActivity.Instance.SetTheme(Resource.Style.ArgumentsTheme);
        }

        public void setNotesTheme()
        {
            MainActivity.Instance.SetTheme(Resource.Style.NotesTheme);
        }

        public void setSettingsTheme()
        {
            MainActivity.Instance.SetTheme(Resource.Style.SettingsTheme);
        }
    }
}
