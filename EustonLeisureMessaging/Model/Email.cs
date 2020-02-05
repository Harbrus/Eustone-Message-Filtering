using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Model
{
    public class Email : Message
    {
        private string sender;
        private string subject;
        private string message;

        public string Sender
        {
            get => sender;
            set
            {
                if (IsEmailValid(value))
                {
                    sender = value;
                }
                else
                {
                    throw new ArgumentException("Please provide a valid email address.");
                }
            }
        }

        public string Subject
        {
            get => subject;
            set
            {
                if (value.Length > 0 && value.Length <= 20)
                {
                    subject = value;
                }
                else
                {
                    throw new ArgumentException("Please insert a subject between 1 and 20 characters.");
                }
            }
        }

        public string Message
        {
            get => message;
            set
            {
                if (value.Length <= 1028 && value.Length > 0)
                {
                    message = value;
                }
                else
                {
                    throw new ArgumentException("Tweet message must  be between 1 and 140 characters.");
                }
            }
        }

        private bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
