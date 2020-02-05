using EustonLeisureMessaging.Model;
using EustonLeisureMessaging.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EustonLeisureMessaging.Services
{
    public class DataManagerSingleton
    {
        private static DataManagerSingleton instance;
        private List<string> quarantinedURLs = new List<string>();
        private Dictionary<string, string> abbreviationsDict = new Dictionary<string, string>();
        private Dictionary<int, string> sirDict = new Dictionary<int, string>();
        private List<string> mentionsList = new List<string>();
        private Dictionary<string, int> hashtagsDict = new Dictionary<string, int>();
        private List<Message> loadMessageList = new List<Message>();
        private List<Message> saveMessageList = new List<Message>();
        private JSONWriter jsonSave = new JSONWriter();
        private JSONReader jsonLoad = new JSONReader();
        public MainViewModel MainViewModel = new MainViewModel();

        private DataManagerSingleton() { }

        public static DataManagerSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataManagerSingleton();
                }

                return instance;
            }
        }

        public List<string> QuarantinedURLs { get => quarantinedURLs;}
        public Dictionary<string, string> AbbreviationsDict { get => abbreviationsDict;}
        public Dictionary<int, string> SIRDict { get => sirDict; }
        public List<string> MentionsList { get => mentionsList;}
        public Dictionary<string, int> HashtagsDict { get => hashtagsDict;}
        public List<Message> LoadMessageList { get => loadMessageList; }
        public JSONReader LoadFile { get => jsonLoad; }
        public List<Message> SaveMessageList { get => saveMessageList; }
        public JSONWriter SaveFile { get => jsonSave; }

        public void LoadAbbreviations()
        {
            string directoryName = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\"));
            using (var reader = new StreamReader(directoryName + @"\_Files\abbreviations.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    abbreviationsDict.Add(values[0], values[1]);
                }
            }
        }

        public void SaveListToJSONFile()
        {
            jsonSave.SaveFile(saveMessageList);
        }

        public void LoadListFromJSONFile(string filepath)
        {
            loadMessageList.Clear();
            string json = jsonLoad.JSONRead(filepath);
            loadMessageList = JsonConvert.DeserializeObject<List<Message>>(json);
        }
        public void LoadListFromTextFile(string filepath)
        {
            loadMessageList.Clear();
            
            foreach (string line in File.ReadAllLines(filepath))
            {
                try
                {
                    Message message = new Message();
                    message.Header = line.Split(' ')[0].ToString().ToUpper();
                    try
                    {
                        message.Body = line.Substring(line.IndexOf(line.Split(' ')[1]));
                    }
                    catch
                    {
                        throw new Exception(message.Header + " has an invalid message body, please fix this in the file.");
                    }
                    loadMessageList.Add(message);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

    }
}
