using System;
using System.Linq;
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
        public String suffisso
        {
            get
            {
                if (loginUrl.Contains("suffiso="))
                    return loginUrl.Split(new[] { "suffisso=" }, StringSplitOptions.None).Last();
                else
                    return "";
            }
        }

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

        public School(String name, Boolean save) 
        {
            this.name = name;
            if (save)
                if (App.Schools.ContainsKey(this.name))
                    App.Schools[this.name] = this;
                else
                    App.Schools.Add(this.name, this);
        }

        public School(Boolean save) 
        { 
            if (save)
                App.Schools.Add(this.name, this);
        }

        public void setUrl(String loginUrl)
        {
            this.loginUrl = loginUrl;
            this.baseUrl = loginUrl.Split(new[] { "/login/" }, StringSplitOptions.None)[0].Replace("/login/", "");
            this.formUrl = baseUrl + "/login/ele_ges.php";
            this.marksUrl = baseUrl + "/valutazioni/visvaltut.php";
            this.argsUrl = baseUrl + "/lezioni/riepargomgen.php";
            this.noteUrl = baseUrl + "/note/sitnotealu.php";
            this.absencesUrl = baseUrl + "/assenze/sitassalut.php";  
        }
    }
}