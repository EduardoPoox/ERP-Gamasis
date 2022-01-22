using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Revision
    {
        public Revision() { }
        public int id { set; get; }
        public string comment { set; get; }
        public string date { set; get; }
        public int type { set; get; } //ENUM(INCIDENT,REQUIREMENT)
        public int idrel { set; get; }
        public string name { set; get; }
        public int accountid { set; get; }
        public string accountname { set; get; }
    }
}