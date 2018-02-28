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

            await new MarksRequests().extractAllMarks();
            await new ArgumentsRequests().extractAllArguments();
            await new NotesRequests().extractAllNotes();
            await new AbsencesRequests().extractAllAbsences();

            App.SerializeObjects();
            return true;
        }


        static public async Task<Boolean> LoginAsync()
        {
            if(cookies == null)
                if( await getCookiesAsync() == "failed")
                    return false;



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

        static public async Task<Boolean> RefreshAsync()
        {
            System.Diagnostics.Debug.WriteLine("Count App Refresh: {0}", App.Grades.Count());
            if (!await LoginAsync())
                return false;

            await new MarksRequests().refreshMarks();
            await new ArgumentsRequests().refreshArguments();
            await new NotesRequests().refreshNotes();
            await new AbsencesRequests().refreshAbsence();

            App.SerializeObjects();
            return true;
        }







        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------TechnicalStuff-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        static public async Task<string> getSeedAsync()
        {
            HttpClient client = new HttpClient(new NativeMessageHandler());

            try
            {
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
            }
            catch { return "failed"; }
            return seed;
        }

        static public async Task<string> getCookiesAsync()
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
            }
            catch { return "failed"; }

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
