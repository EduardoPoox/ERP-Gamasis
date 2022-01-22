using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Log
    {
        public int id { set; get; }
        public int idrel { set; get; }
        public int type { set; get; } //enum('Access','Status','Incident','Requirement')
        public int subtype { set; get; } //Added,Updated
        public int typestring { set; get; }
        public string ip { set; get; }
        public int accountid { set; get; }
        public string accountname { set; get; }
        public string navigator { set; get; }
        public string date { set; get; }
        public string val { set; get; }
        public int platform { set; get; }
        public string platformstring { set; get; }
        public string comments { set; get; }
       
    }
}