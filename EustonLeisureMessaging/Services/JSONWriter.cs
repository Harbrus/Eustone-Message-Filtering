using EustonLeisureMessaging.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Services
{
    [Serializable()]
    public class JSONWriter
    {
        public void SaveFile(List<Message> messageList)
        {
            string directoryName = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\"));
            using (StreamWriter outputFile = File.CreateText(directoryName + @"\_Files\outputmessages.json"))
            {
                JsonSerializer jsonWriter = new JsonSerializer();
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.Serialize(outputFile, messageList);
            }
        }
    }
}
