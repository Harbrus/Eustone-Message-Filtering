using System;
using System.Collections.Generic;
using EustonLeisureMessaging;
using EustonLeisureMessaging.Model;
using EustonLeisureMessaging.Services;
using EustonLeisureMessaging.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EustonLeisureMessagingUnitTest
{
    [TestClass]
    public class TestClass
    {
        [TestMethod]
        public void TestMessageProcessing()
        {
            DataManagerSingleton.Instance.LoadAbbreviations();
            Message message = new Message();
            message.Header = "S123123123";
            message.Body = "A message test";

            string expectedHeader = "S123123123";
            string expectedBody = "A message test";

            Assert.AreEqual(message.Header, expectedHeader);
            Assert.AreEqual(message.Body, expectedBody);
        }

        [TestMethod]
        public void TestSMSProcessing()
        {
            Message sms = new Message();
            sms.Header = "S123123123";
            sms.Body = "+447501434545 That is so funny LOL";
            DataManagerSingleton.Instance.MainViewModel.test= true;
            DataManagerSingleton.Instance.MainViewModel.SelectedMessage = sms;

            string expectedProcessedMessage = "Message ID: S123123123\nSender: +447501434545\nMessage Content: That is so funny LOL <Laughing out loud> ";

            Assert.AreEqual(expectedProcessedMessage, DataManagerSingleton.Instance.MainViewModel.ProcessedMessage);
        }

        [TestMethod]
        public void TestTweetProcessing()
        {
            Message tweet = new Message();
            tweet.Header = "T123123123";
            tweet.Body = "@john That is so funny LOL";
            DataManagerSingleton.Instance.MainViewModel.test = true;
            DataManagerSingleton.Instance.MainViewModel.SelectedMessage = tweet;

            string expectedProcessedMessage = "Message ID: T123123123\nSender: @john\nMessage Content: That is so funny LOL <Laughing out loud> ";

            Assert.AreEqual(expectedProcessedMessage, DataManagerSingleton.Instance.MainViewModel.ProcessedMessage);
        }

        [TestMethod]
        public void TestStandardEmailProcessing()
        {
            Message email = new Message();
            email.Header = "E123123123";
            email.Body = "ttt@gmail.com News That is so funny";
            DataManagerSingleton.Instance.MainViewModel.test = true;
            DataManagerSingleton.Instance.MainViewModel.SelectedMessage = email;

            string expectedProcessedMessage = "Standard Email Message\nSender: ttt@gmail.com\nSubject: News\nMessage: That is so funny";

            Assert.AreEqual(expectedProcessedMessage, DataManagerSingleton.Instance.MainViewModel.ProcessedMessage);
        }

        [TestMethod]
        public void TestSIRProcessing()
        {
            Message sir = new Message();
            sir.Header = "E123123123";
            sir.Body = "jkon@gmail.com SIR 12/02/2012 Sport Centre Code: 12-123-12 Nature of Incident: Theft Robbed us twitter.com/home goole.com";
            DataManagerSingleton.Instance.MainViewModel.test = true;
            DataManagerSingleton.Instance.MainViewModel.SelectedMessage = sir;

            string expectedProcessedMessage = "Significant Incident Report\nSender: jkon@gmail.com\nSubject:\nSport Centre Code: 12-123-12\nNature of Incident: Theft\nMessage:\nRobbed us <URL Quarantined> <URL Quarantined>";

            Assert.AreEqual(expectedProcessedMessage, DataManagerSingleton.Instance.MainViewModel.ProcessedMessage);
        }

        [TestMethod]
        public void TestHashtagsProcessing()
        {
            Message tweet = new Message();
            tweet.Header = "T123123123";
            tweet.Body = "@john That is so funny LOL #apalla";
            DataManagerSingleton.Instance.MainViewModel.test = true;
            DataManagerSingleton.Instance.MainViewModel.SelectedMessage = tweet;

            int count = 1;
            string hashtag = "#apalla " + count;
            List<string> expectedList = new List<string>();
            expectedList.Add(hashtag);

            Assert.AreEqual(expectedList[0], DataManagerSingleton.Instance.MainViewModel.TrendList[0]);
        }

        [TestMethod]
        public void TestURLsProcessing()
        {
            Message sir = new Message();
            sir.Header = "E123123123";
            sir.Body = "jkon@gmail.com SIR 12/02/2012 Sport Centre Code: 12-123-12 Nature of Incident: Theft Robbed us twitter.com/home";
            DataManagerSingleton.Instance.MainViewModel.test = true;
            DataManagerSingleton.Instance.MainViewModel.SelectedMessage = sir;

            string url = "twitter.com/home";
            List<string> expectedList = new List<string>();
            expectedList.Add(url);

            Assert.AreEqual(expectedList[0], DataManagerSingleton.Instance.MainViewModel.QuarantinedURLs[0]);
        }

        [TestMethod]
        public void TestIncidentsProcessing()
        {
            Message sir = new Message();
            sir.Header = "E123123123";
            sir.Body = "jkon@gmail.com SIR 12/02/2012 Sport Centre Code: 12-123-12 Nature of Incident: Theft Robbed us twitter.com/home";
            DataManagerSingleton.Instance.MainViewModel.test = true;
            DataManagerSingleton.Instance.MainViewModel.SelectedMessage = sir;

            string sirType = "12-123-12 12/02/2012 Theft";
            List<string> expectedList = new List<string>();
            expectedList.Add(sirType);

            Assert.AreEqual(expectedList[0], DataManagerSingleton.Instance.SIRDict[1]);
        }

        [TestMethod]
        public void TestMentionsProcessing()
        {
            Message tweet = new Message();
            tweet.Header = "T123123123";
            tweet.Body = "@john That is so funny LOL @tata #apalla";
            DataManagerSingleton.Instance.MainViewModel.test = true;
            DataManagerSingleton.Instance.MainViewModel.SelectedMessage = tweet;

            string mention = "@tata";
            List<string> expectedList = new List<string>();
            expectedList.Add(mention);

            Assert.AreEqual(expectedList[0], DataManagerSingleton.Instance.MainViewModel.MentionList[0]);
        }
    }
}
