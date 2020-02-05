using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Model
{
    public class Tweet : Message
    {
        private string sender;
        private string message;

        public string Sender
        {
            get => sender;
            set
            {
                if (value.StartsWith("@") && value.Length <= 15 && value.Length > 0)
                {
                    sender = value;
                }
                else
                {
                    throw new ArgumentException("Tweet sender must start with @ and be between 1 and 15 characters.");
                }
            }
        }
        public string Message
        {
            get => message;
            set
            {
                if (value.Length <= 140 && value.Length > 0)
                {
                    message = value;
                }
                else
                {
                    throw new ArgumentException("Tweet message must  be between 1 and 140 characters.");
                }
            }
        }
    }
}
