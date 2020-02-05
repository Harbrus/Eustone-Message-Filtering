using EustonLeisureMessaging.Commands;
using EustonLeisureMessaging.Model;
using EustonLeisureMessaging.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EustonLeisureMessaging.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private int sirKey = 1;
        private Message selectedMessage;

        public bool test = false; 

        public Message SelectedMessage
        {
            get => selectedMessage;
            set
            {
                selectedMessage = value;

                if(!test)
                {
                    OnChanged(nameof(SelectedMessage));
                }
                
                if(SelectedMessage != null)
                {
                    ProcessMessageButtonClick();
                }
            }
        }

        public List<Message> tempMessageList { get; }
        public string SendMessageHeader { get; set; }
        public string SendMessageBody { get; set; }
        public string ProcessedMessage { get; set; }

        public List<string> TrendList { get; set; }
        public List<string> MentionList { get; set; }
        public List<string> QuarantinedURLs { get; set; }
        public List<string> SIRList { get; set; }
        public List<Message> MessageFromFileList { get; set; }

        public ICommand LoadMessage { get; private set; }
        public ICommand ProcessMessage { get; private set; }
        public ICommand ClearSendMessage { get; private set; }
        public ICommand SaveMessage { get; private set; }
        public ICommand ClearProcessedMessage { get; private set; }

        public MainViewModel()
        {
            SendMessageHeader = "Header";
            SendMessageBody = "Body";
            ProcessedMessage = string.Empty;
            MessageFromFileList = new List<Message>();
            tempMessageList = new List<Message>();
            TrendList = new List<string>();
            MentionList = new List<string>();
            QuarantinedURLs = new List<string>();
            SIRList = new List<string>();

            LoadMessage = new RelayCommand(LoadFileButtonClick);
            ProcessMessage = new RelayCommand(ProcessMessageButtonClick);
            ClearSendMessage = new RelayCommand(ClearSendMessageButtonClick);
            SaveMessage = new RelayCommand(SaveMessageButtonClick);
            ClearProcessedMessage = new RelayCommand(ClearProcessedMessageButtonClick);
        }

        private void LoadFileButtonClick()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            MessageFromFileList.Clear();

            if (fileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(fileDialog.FileName);
                switch (extension)
                {
                    case ".json":
                        DataManagerSingleton.Instance.LoadListFromJSONFile(fileDialog.FileName);
                        List<Message> tempList = new List<Message>();
                        foreach (Message message in DataManagerSingleton.Instance.LoadMessageList)
                        {
                            tempList.Add(message);
                        }
                        MessageFromFileList = tempList;
                        OnChanged(nameof(MessageFromFileList));
                        break;
                    case ".txt":
                        DataManagerSingleton.Instance.LoadListFromTextFile(fileDialog.FileName);
                        List<Message> tempList2 = new List<Message>();
                        foreach (Message message in DataManagerSingleton.Instance.LoadMessageList)
                        {
                            tempList2.Add(message);
                        }
                        MessageFromFileList = tempList2;
                        OnChanged(nameof(MessageFromFileList));
                        break;
                    default:
                        break;
                }
            }
        }

        private void ProcessMessageButtonClick()
        {
            tempMessageList.Clear();
            try
            {
                if (SelectedMessage == null)
                {
                    selectedMessage = new Message();
                    selectedMessage.Header = SendMessageHeader.ToUpper();
                    selectedMessage.Body = SendMessageBody;
                }
                SelectedMessage.Header.ToUpper();
                switch (SelectedMessage.Header[0])
                {
                    case 'S':
                        ProcessSMS(SelectedMessage);
                        break;
                    case 'T':
                        ProcessTweet(SelectedMessage);
                        break;
                    case 'E':
                        ProcessEmail(SelectedMessage);
                        break;
                    default:
                        break;
                }
                tempMessageList.Add(selectedMessage);
                SelectedMessage = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ProcessSMS(Message message)
        {
            SMS sms = new SMS();

            try
            {
                sms.Header = message.Header;
                sms.Body = message.Body;
                sms.Sender = sms.Body.Split(' ')[0];
                try
                {
                    sms.Message = sms.Body.Substring(sms.Sender.Length + 1);
                }
                catch
                {
                    sms.Message = string.Empty;
                }

                sms.Message = ProcessAbbreaviations(sms.Message);
                ProcessedMessage = "Message ID: " + sms.Header + "\nSender: " + sms.Sender + "\nMessage Content: " + sms.Message;
                
                if(!test)
                {
                    OnChanged(nameof(ProcessedMessage));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ProcessTweet(Message message)
        {
            try
            {
                Tweet tweet = new Tweet();
                tweet.Header = message.Header;
                tweet.Body = message.Body;
                tweet.Sender = tweet.Body.Split(' ')[0];
                try
                {
                    tweet.Message = tweet.Body.Substring(tweet.Sender.Length + 1);
                }
                catch
                {
                    tweet.Message = string.Empty;
                }

                try
                {
                    tweet.Message = ProcessAbbreaviations(tweet.Message);
                    ProcessTweetMessage(tweet.Message);
                    ProcessedMessage = "Message ID: " + tweet.Header + "\nSender: " + tweet.Sender + "\nMessage Content: " + tweet.Message;
                    if (!test)
                    {
                        OnChanged(nameof(ProcessedMessage));
                    }
                    UpdateMentionsList();
                    UpdateTrendList();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ProcessTweetMessage(string tweet)
        {
            // Check for mentions and add them to the list of mentions.
            foreach (string word in tweet.Split(' '))
            {
                try
                {
                    switch (word[0])
                    {
                        case '@':
                            if (DataManagerSingleton.Instance.MentionsList.Contains(word))
                            {
                                continue;
                            }
                            else
                            {
                                DataManagerSingleton.Instance.MentionsList.Add(word);
                            }
                            continue;

                        case '#':
                            if (DataManagerSingleton.Instance.HashtagsDict.ContainsKey(word))
                            {
                                DataManagerSingleton.Instance.HashtagsDict[word] += 1;
                            }
                            else
                            {
                                DataManagerSingleton.Instance.HashtagsDict.Add(word, 1);
                            }
                            continue;

                        default:
                            continue;
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        private void ProcessEmail(Message message)
        {
            Email email = new Email();
            email.Header = message.Header;
            email.Body = message.Body;
            try
            {
                email.Sender = email.Body.Split(' ')[0];
                email.Subject = email.Body.Split(' ')[1];

                if (email.Subject.Contains("SIR"))
                {
                    ProcessSIR(email);
                }
                else
                {
                    email.Message = email.Body.Substring(email.Body.IndexOf(email.Body.Split(' ')[2]));
                    string processedMessage = SanitizeURLs(email);
                    email.Message = processedMessage;
                    ProcessedMessage = "Standard Email Message\n" + "Sender: " + email.Sender + "\nSubject: " + email.Subject + "\nMessage: " + email.Message;
                    if (!test)
                    {
                        OnChanged(nameof(ProcessedMessage));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ProcessSIR(Email email)
        {
            SIR sir = new SIR();
            sir.Header = email.Header;
            sir.Body = email.Body;
            sir.Sender = email.Sender;
            try
            {
                sir.SIRSubject = sir.Body.Split(' ')[1] + " " + sir.Body.Split(' ')[2];
                sir.SortCode = sir.Body.Split(' ')[3] + " " + sir.Body.Split(' ')[4] + " " + sir.Body.Split(' ')[5] + " " + sir.Body.Split(' ')[6];
                sir.NatureOfIncident = sir.Body.Split(' ')[7] + " " + sir.Body.Split(' ')[8] + " " + sir.Body.Split(' ')[9] + " " + sir.Body.Split(' ')[10];
               
                if(!DataManagerSingleton.Instance.SIRDict.ContainsKey(sirKey))
                {
                    DataManagerSingleton.Instance.SIRDict.Add(sirKey, sir.Body.Split(' ')[6] + " " + sir.Body.Split(' ')[2] + " " + sir.Body.Split(' ')[10]);
                    sirKey++;
                }

                sir.Message = sir.Body.Substring(sir.Body.IndexOf(sir.Body.Split(' ')[11]));
                string processedMessage = SanitizeURLs(sir);
                sir.Message = processedMessage;
                ProcessedMessage = "Significant Incident Report\n" + "Sender: " + sir.Sender + "\nSubject:" + sir.Subject +
                    "\n" + sir.SortCode + "\n" + sir.NatureOfIncident + "\nMessage:\n" + sir.Message;
                if (!test)
                {
                    OnChanged(nameof(ProcessedMessage));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private string SanitizeURLs(Email email)
        {
            QuarantinedURLs.Clear();
            List<string> tempUrls = new List<string>();
            foreach (string word in email.Message.Split(' '))
            {
                if (ValidateUrl(word, true))
                {
                    string newMessage = email.Message.Replace(word, "<URL Quarantined>");
                    DataManagerSingleton.Instance.QuarantinedURLs.Add(word);
                    tempUrls = DataManagerSingleton.Instance.QuarantinedURLs;
                    email.Message = newMessage;
                }
            }
            this.QuarantinedURLs = tempUrls;
            
            if(!test)
                OnChanged(nameof(QuarantinedURLs));
            
            return email.Message;
        }

        public static bool ValidateUrl(string value, bool required)
        {
            value = value.Trim();
            if (required == false && value == "") return true;
            if (required && value == "") return false;

            Regex pattern = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
            Match match = pattern.Match(value);
            if (match.Success == false) return false;
            return true;
        }

        private string ProcessAbbreaviations(string message)
        {
            int wordIndex = 0;
            foreach (string word in message.Split(' '))
            {
                try
                {
                    wordIndex += (word.Length + 1);
                }
                catch
                {
                    break;
                }

                if (DataManagerSingleton.Instance.AbbreviationsDict.ContainsKey(word.ToUpper()))
                {
                    int abbreviationIndex = wordIndex;
                    string definition = "<" + DataManagerSingleton.Instance.AbbreviationsDict[word.ToUpper()] + "> ";

                    try
                    {
                        if (message[abbreviationIndex + word.Length + 1].Equals('<'))
                        {
                            break;
                        }

                        message = message.Insert(abbreviationIndex, definition);
                        wordIndex += definition.Length;
                    }
                    catch
                    {
                        string newMessage = message + " " + definition;
                        message = newMessage;
                    }
                }
            }
            return message;
        }

        private void UpdateTrendList()
        {
            var sortedHashtagsDict = DataManagerSingleton.Instance.HashtagsDict.OrderByDescending(x => x.Value);
            TrendList.Clear();
            List<string> tempList = new List<string>();

            foreach (var hashtag in sortedHashtagsDict)
            {
                string trend = hashtag.Key.ToString() + " " + hashtag.Value.ToString();
                tempList.Add(trend);
            }

            TrendList = tempList;
            
            if (!test)
                OnChanged(nameof(TrendList));
        }
        private void UpdateMentionsList()
        {
            MentionList.Clear();
            List<string> tempList = new List<string>();

            foreach (string word in DataManagerSingleton.Instance.MentionsList)
            {
                tempList.Add(word);
            }

            this.MentionList = tempList;
            
            if (!test)
                OnChanged(nameof(MentionList));
        }

        private void UpdateSIRList()
        {
            SIRList.Clear();
            List<string> tempList = new List<string>();

            foreach (var sirItem in DataManagerSingleton.Instance.SIRDict)
            {
                string sir = "N." + sirItem.Key.ToString() + " " + sirItem.Value.ToString();
                tempList.Add(sir);
            }

            this.SIRList = tempList;

            if (!test)
                OnChanged(nameof(SIRList));
        }

        private void ClearSendMessageButtonClick()
        {
            SendMessageHeader = "Header";
            SendMessageBody = "Body";

            OnChanged(nameof(SendMessageHeader));
            OnChanged(nameof(SendMessageBody));
        }

        private void SaveMessageButtonClick()
        {
            try
            {
                Message save = new Message();
                save = tempMessageList[0];
                DataManagerSingleton.Instance.SaveMessageList.Add(save);
                DataManagerSingleton.Instance.SaveListToJSONFile();
                tempMessageList.Clear();
                ProcessedMessage = string.Empty;
                UpdateSIRList();
                OnChanged(nameof(ProcessedMessage));
            }
            catch (Exception e)
            {
                MessageBox.Show("Message is empty. Please provide a message.");
            }
        }

        private void ClearProcessedMessageButtonClick()
        {
            ProcessedMessage = string.Empty;
            tempMessageList.Clear();
            OnChanged(nameof(ProcessedMessage));
        }
    }
}
