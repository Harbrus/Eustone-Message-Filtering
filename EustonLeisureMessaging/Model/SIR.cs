using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EustonLeisureMessaging.Model
{
    public class SIR : Email
    {
        private string pattern = @"^([0-9][0-9][\-][0-9][0-9][0-9][\-][0-9][0-9])$";
        private string sirSubject;
        private string sortCode;
        private string natureOfIncident;
        private List<string> incidentList; 

        public SIR()
        {
            incidentList = new List<string>();
            incidentList.Add("THEFT");
            incidentList.Add("STAFFATTACK");
            incidentList.Add("DEVICEDAMAGE");
            incidentList.Add("RAID");
            incidentList.Add("CUSTOMERATTACK");
            incidentList.Add("STAFFABUSE");
            incidentList.Add("BOMBTHREAT");
            incidentList.Add("TERRORISM");
            incidentList.Add("SUSPICIOUSINCIDENT");
            incidentList.Add("SPORTINJURY");
            incidentList.Add("PERSONALINFOLEAK");
        }

        public string SortCode
        {
            get => sortCode;
            set
            {
                if (value.Substring(value.IndexOf(value.Split(' ')[0]), value.IndexOf(value.Split(' ')[3])) == "Sport Centre Code: " && Regex.IsMatch(value.Split(' ')[3], pattern))
                {
                    sortCode = value;
                }
                else
                {
                    throw new ArgumentException("Please insert a valid numeric sortcode. E.g. Sport Centre Code: 12-123-12");
                }
            }
        }

        public string SIRSubject 
        { 
            get => this.sirSubject;
            set
            {
                if (value.Split(' ')[0] == "SIR" && DateTime.TryParse(value.Split(' ')[1], out DateTime date))
                {
                    this.sirSubject = value;
                }
                else
                {
                    throw new ArgumentException("Please insert a valid subject: SIR dd/mm/yyyy");
                }
            }
        }

        public List<string> IncidentList { get => incidentList;}
        public string NatureOfIncident 
        { 
            get => natureOfIncident;
            set
            {
                if (value.Substring(value.IndexOf(value.Split(' ')[0]), value.IndexOf(value.Split(' ')[3])) == "Nature of Incident: " && 
                    IncidentList.Contains(value.Split(' ')[3].ToUpper()))
                {
                    natureOfIncident = value;
                }
                else
                {
                    throw new Exception("Please insert a valid nature of incident. E.g. Nature of Incident: Theft. The incident type must also have no spaces e.g StaffAbuse");
                }
            }
        }
    }
}
