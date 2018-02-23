using System;
using Android.Content;
using Registro.Droid;
using static Registro.Controls.AndroidClosing;

[assembly: Xamarin.Forms.Dependency(typeof(CloseRender))]
namespace Registro.Droid
{
    public class CloseRender : IClose
    {
        public Boolean CloseApp()
        {
            Context c =  Xamarin.Forms.Forms.Context as global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity;
            Intent startMain = new Intent(Intent.ActionMain);
            startMain.AddCategory(Intent.CategoryHome);
             c.StartActivity(startMain);
            return true;
        }        
       
    }
}
