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
    class HttpRequest
    {
        User User;

        String seed;
        String cookies;

        public HttpRequest(User user)
        {
            this.User = user;
        }

        public async void extractAllAsync()
        {
            LoginAsync();

            String marksPage = await getMarksPageAsync();
            extratMarks(marksPage);
        }


        public async Task LoginAsync()
        {
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

            HttpResponseMessage resp = await new HttpClient(new NativeMessageHandler()).SendAsync(req);
            resp.Dispose();
        }


        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getSeed-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        async Task<string> getSeedAsync()
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
                    }
                }
            }
            return seed;
        }



        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getCookies-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<string> getCookiesAsync()
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

            resp.Dispose();
            System.Diagnostics.Debug.WriteLine("<--------------------------------Cookies--------------------------------->");
            System.Diagnostics.Debug.WriteLine(cookies);
            return cookies;
        }



        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------getMarksPage-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<string> getMarksPageAsync()
        {
            string pageSource;
            HttpRequestMessage getRequest = new HttpRequestMessage();
            getRequest.RequestUri = new Uri(User.school.marksUrl);
            getRequest.Headers.Add("Cookie", cookies);
            getRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            getRequest.Headers.Add("UserAgent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 63.0.3239.84 Safari / 537.36");

            HttpResponseMessage getResponse = await new HttpClient(new NativeMessageHandler()).SendAsync(getRequest);

            pageSource = await getResponse.Content.ReadAsStringAsync();
            return pageSource;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------extratMarks-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public void extratMarks(String html)
        {
            Document doc = Dcsoup.ParseBodyFragment(html, "");

            Subject currentSubject = new Subject("", false);
            Grade currentGrade = new Grade("", "", "", "", currentSubject, false);

            int Column = 0;
            for (int i = 2; ; i++)
            {
                Element table = doc.Select("body > div.contenuto > table > tbody > tr:nth-child(" + i + ")").First;

                if (table == null) break;

                Elements inputElements = table.GetElementsByTag("td");


                foreach (Element inputElement in inputElements)
                {
                    if ("4".Equals(inputElement.Attr("colspan")))
                    {
                        Column = 1;
                        currentSubject = new Subject(inputElement.Text.Replace("[", "").Replace("]", ""), true);
                    }
                    else if (Column == 1)
                    {
                        currentGrade = new Grade(inputElement.Text, "", "", "", currentSubject, true);
                        currentSubject.grades.Add(currentGrade);
                        Column++;
                    }
                    else if (Column == 2)
                    {
                        currentGrade.type = (inputElement.Text);
                        Column++;
                    }
                    else if (Column == 3)
                    {
                        currentGrade.setGrade(inputElement.Text);
                        Column++;
                    }
                    else if (Column == 4)
                    {
                        currentGrade.Description = (inputElement.Text);
                        Column = 1;
                    }

                }
            }
        }




        //------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------password-----------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------


        public async Task<string> cryptPasswordAsync(String password)
        {
            await getSeedAsync();
            return hex_md5(hex_md5(hex_md5(password)) + seed);
        }

        public string hex_md5(string input)

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
