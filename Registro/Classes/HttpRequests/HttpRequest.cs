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

namespace Registro
{
    public class HttpRequest
    {
        static public User User;

        static public String seed;
        static public String cookies;

        static public async Task<Boolean> extractAllAsync()
        {
            if (!await LoginAsync())
                return false;

            await MarksRequests.extractAll();
            await ArgumentsRequests.extractAll();

            return true;
        }


        static public async Task<Boolean> LoginAsync()
        {
            if(cookies == null)
                await getCookiesAsync();

            string formParams = "utente=" + User.username + "&pass=&OK=Accedi&password=" + await cryptPasswordAsync(User.password);

            HttpRequestMessage req = new HttpRequestMessage();
            Uri uri = new Uri(User.school.formUrl);
            req.RequestUri = uri;
            req.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            req.Headers.Add("Cookie", cookies);
            req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            req.Headers.Add("UserAgent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.84 Safari / 537.36");
            req.Headers.Add("Referer", User.school.loginUrl);
            req.Method = HttpMethod.Post;

            byte[] bytes = Encoding.UTF8.GetBytes(formParams);
            req.Headers.TryAddWithoutValidation("Content-Length", bytes.Length.ToString());
            req.Content = new StringContent(formParams, Encoding.UTF8, "application/x-www-form-urlencoded");

            try{
                HttpResponseMessage resp = await new HttpClient(new NativeMessageHandler()).SendAsync(req);
                resp.Dispose();
                req.Dispose();
                return true;
            }catch{
                req.Dispose();
                return false; 
            }
        }

        static public void clearLists()
        {
            App.Subjects = new Dictionary<string, Subject>();
            App.Grades = new List<Grade>();
            App.Arguments = new List<Arguments>();
        }

        static public async Task<Boolean> RefreshAsync()
        {
            if (!await LoginAsync())
                return false;
            clearLists();
            await MarksRequests.extractAll();
            await ArgumentsRequests.extractAll();

            return true;
        }







        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------TechnicalStuff-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        static public async Task<string> getSeedAsync()
        {
            HttpClient client = new HttpClient(new NativeMessageHandler());

            using (client)
            {
                using (HttpResponseMessage response = await client.GetAsync(User.school.loginUrl))
                {
                    using (HttpContent content = response.Content)
                    {
                        var page = await content.ReadAsStringAsync();
                        seed = page.Split(new[] { "seme='" }, StringSplitOptions.None)[1].Substring(0, 32);
                        response.Dispose();
                    }
                }
            }
            return seed;
        }

        static public async Task<string> getCookiesAsync()
        {
            string cookieHeader = "";
            string url = "https://www.lampschool.it/hosting_trentino_17_18/login/login.php?suffisso=scuola_27";
            HttpRequestMessage req = new HttpRequestMessage();
            req.RequestUri = new Uri(url);

            HttpResponseMessage resp = await new HttpClient(new NativeMessageHandler()).SendAsync(req);
            try
            {
                HttpHeaders headers = resp.Headers;
                IEnumerable<string> values;
                if (headers.TryGetValues("Set-Cookie", out values))
                {
                    cookieHeader = values.First();
                }
            }
            catch { }

            String[] temp = cookieHeader.Split(';');
            cookies = temp[0];

            req.Dispose();
            resp.Dispose();
            System.Diagnostics.Debug.WriteLine("<--------------------------------Cookies--------------------------------->");
            System.Diagnostics.Debug.WriteLine(cookies);
            return cookies;
        }

        static public async Task<string> cryptPasswordAsync(String password)
        {
            await getSeedAsync();
            return hex_md5(hex_md5(hex_md5(password)) + seed);
        }

        static public string hex_md5(string input)

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
    }
}
