using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Model
{
    public class SMS : Message
    {
        private string pattern = @"^((\+\d{1,3}(-| )?\(?\d\)?(-| )?\d{1,5})|(\(?\d{2,6}\)?))(-| )?(\d{3,4})(-| )?(\d{4})(( x| ext)\d{1,5}){0,1}$";
        private string sender;
        private string message;

        public string Sender 
        {
            get => sender;
            set
            {
                if (Regex.IsMatch(value, pattern))
                {
                    sender = value;
                }
                else
                {
                    throw new ArgumentException("Please provide a valid international phone number");
                }
            }
        }
        public string Message
        {
            get => message;
            set
            {
                if (value.Length > 0 && value.Length <= 140)
                {
                    message = value;
                }
                else
                {
                    throw new ArgumentException("SMS message body must be between 1 and 140 characters.");
                }
            }
        }

    }
}
