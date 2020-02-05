using System;

namespace EustonLeisureMessaging.Model
{
    /// <summary>
    /// Abstract Class for all the message inputs
    /// </summary>
    public class Message
    {
        private string header;
        private string body;

        public string Header 
        { 
            get => header; 
            set 
            { 
                if (value.Length == 10)
                {
                    if (value.StartsWith("S") || value.StartsWith("T") || value.StartsWith("E"))
                    {
                        if (int.TryParse(value.Substring(1, 9), out int code))
                        {
                            header = value.ToUpper();
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("The header should be in the correct format. Starting with S, T, or E and followed by 9 numbers.");
                }
            } 
        }

        public string Body
        {
            get => body;
            set
            {
                if (value.Length > 0)
                {
                    body = value;
                }
                else
                {
                    throw new ArgumentException("Message body cannot be empty.");
                }
            }
        }
    }
}
