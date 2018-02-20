using System;

using Xamarin.Forms;

namespace Registro
{
    public class School : ContentPage
    {
        public String marksUrl { get; set; }
        public String formUrl { get; set; }
        public String loginUrl { get; set; }
        public String argsUrl { get; set; }
        public String noteUrl { get; set; }
        public String absencesUrl { get; set; }

        public String baseUrl { get; set; }
        public String name { get; set; }

        public School(String loginUrl, String name)
        {
            this.loginUrl = loginUrl;
            this.baseUrl = loginUrl.Split(new[] { "/login/" }, StringSplitOptions.None)[0].Replace("/login/", "");
            this.name = name;
            this.formUrl = baseUrl + "/login/ele_ges.php";
            this.marksUrl = baseUrl + "/valutazioni/visvaltut.php";
            this.argsUrl = baseUrl + "/lezioni/riepargomgen.php";
            this.noteUrl = baseUrl + "/note/sitnotealu.php";
            this.absencesUrl = baseUrl + "/assenze/sitassalut.php";
        }
    }
}