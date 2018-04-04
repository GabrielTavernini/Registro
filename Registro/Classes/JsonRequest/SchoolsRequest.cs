using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Registro.Classes.HttpRequests;
using Registro.Pages;

namespace Registro.Classes.JsonRequest
{
    public class SchoolsRequest
    {
        static private String json;
        static private Dictionary<string, string> temp;

        static public async Task RequestSchools()
        {
            try
            {
                String QueryLogin = "http://lampschooltest.altervista.org/";
                json = await Utility.GetPageAsync(QueryLogin);

                if (json == null || json == "")
                    throw new Exception("Null Json");

                System.Diagnostics.Debug.WriteLine(json);
                temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch
            {
                var assembly = typeof(FirstPage).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.schools.json");

                using (var reader = new System.IO.StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                    temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
            }

            foreach (KeyValuePair<string, string> kv in temp)
            {
                School s = new School(kv.Key, true);
                s.setUrl(kv.Value);
            }
        }
    }
}
