using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace MicrosoftEvangelists.Models
{
    public class Profile
    {
        public string id { get; set; }
        public string name { get; set; }
        public string slogan { get; set; }
        public string bio { get; set; }
        public string picture { get; set; }
        public string website { get; set; }
        public string blog { get; set; }
        public string twitter { get; set; }
        public string linkedin { get; set; }
        public string skype { get; set; }
        public string github { get; set; }
        public string stackoverflow { get; set; }
        public string email { get; set; }
        public string youtube { get; set; }
        public string vimeo { get; set; }
        public List<string> cities { get; set; }
        public List<string> states { get; set; }
        public string country { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
    }
}
