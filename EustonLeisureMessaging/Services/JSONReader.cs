using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Services
{
    public class JSONReader
    {
        public string JSONRead(string filepath)
        {
            string json = "";
            using (var reader = new StreamReader(filepath))
            {
                json = reader.ReadToEnd();
            }

            return json;
        }
    }
}
