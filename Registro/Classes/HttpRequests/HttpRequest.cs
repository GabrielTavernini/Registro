using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using Supremes.Nodes;
using Supremes;
using System.Net.Http;
using ModernHttpClient;
using System.Collections;
using System.Net.Http.Headers;
using Registro.Classes.HttpRequests;
using Xamarin.Forms;

namespace Registro
{
    public class HttpRequest
    {
        static public User User;

        static public String seed;
        static public String cookies;
        static public bool globalRefresh;

        static public async Task<Boolean> extractAllAsync()
        {
            if (!await LoginAsync())
                return false;

            await new MarksRequests().extractAllMarks();
            await new ArgumentsRequests().extractAllArguments();
            await new NotesRequests().extractAllNotes();
            //await new AbsencesRequests().extractAllAbsences();

            App.SerializeObjects();
            App.lastRefresh = DateTime.Now;
            return true;
        }

        static public async Task<Boolean> RefreshAsync()
        {
            if (!await LoginAsync())
                return false;

            globalRefresh = true;
            await new MarksRequests().refreshMarks();
            await new NotesRequests().refreshNotes();
            //await new AbsencesRequests().refreshAbsence();
            await new ArgumentsRequests().refreshArguments();

            App.SerializeObjects();
            App.lastRefresh = DateTime.Now;
            return true;
        }

        static public async Task<Boolean> LoginAsync()
        {
            System.Diagnostics.Debug.WriteLine("Login");
            
            if (cookies == null)
                if (await getCookiesAsync() == "failed")
                    return false;

            try
            {
                string formParams = "utente=" + User.username + "&pass=&OK=Accedi&password=" + cryptPasswordAsync(User.password);
                await Utility.PostPageAsync(User.school.formUrl, formParams, User.school.loginUrl);
            }
            catch
            {
                cookies = null;
                return false;
            }

            return true;
        }








        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------TechnicalStuff-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        #region stuff

        static private void getSeedAsync()
        {
            String s = DateTime.Now.ToString("yyyy-MM-dd");
            System.Diagnostics.Debug.WriteLine(s);
            seed = hex_md5(s);
        }

        static private async Task<string> getCookiesAsync()
        {
            try
            {
                string url = User.school.loginUrl;
                HttpRequestMessage req = new HttpRequestMessage();
                req.RequestUri = new Uri(url);
                HttpResponseMessage resp = new HttpResponseMessage();
                resp = await new HttpClient(new NativeMessageHandler()).SendAsync(req);

                IEnumerable<string> values;
                if (resp.Headers.TryGetValues("Set-Cookie", out values))
                {
                    String[] temp = values.First().Split(';');
                    cookies = temp[0];
                }
                req.Dispose();
                resp.Dispose();
            }
            catch { return "failed"; }
            return cookies;
        }

        /*
        static public async Task<string> getCookiesAndSeed()
        {
            string cookieHeader = "";
            string url = User.school.loginUrl;
            HttpRequestMessage req = new HttpRequestMessage();
            req.RequestUri = new Uri(url);

            HttpResponseMessage resp = new HttpResponseMessage();
            try
            {
                resp = await new HttpClient(new NativeMessageHandler()).SendAsync(req);
                HttpHeaders headers = resp.Headers;
                IEnumerable<string> values;
                if (headers.TryGetValues("Set-Cookie", out values))
                {
                    cookieHeader = values.First();
                }

                var page = await resp.Content.ReadAsStringAsync();
                seed = page.Split(new[] { "seme='" }, StringSplitOptions.None)[1].Substring(0, 32);
            }
            catch { return "failed"; }

            String[] temp = cookieHeader.Split(';');
            cookies = temp[0];

            req.Dispose();
            resp.Dispose();
            System.Diagnostics.Debug.WriteLine("<--------------------------------Cookies--------------------------------->");
            System.Diagnostics.Debug.WriteLine(cookies);
            return cookies; 
        }*/

        static private string cryptPasswordAsync(String password)
        {
            getSeedAsync();
            return hex_md5(hex_md5(hex_md5(password)) + seed);
        }

        static private string hex_md5(string input)

        {

            // step 1, calculate MD5 hash from input

            MD5 md5 = MD5.Create();

            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("x2"));

            }

            return sb.ToString();
        }

        #endregion
    }
}
