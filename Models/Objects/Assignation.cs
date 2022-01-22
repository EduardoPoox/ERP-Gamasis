using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Assignation
    {
        public string value { set; get; }
        public int type { set; get; } //Module, Programmer
        public string typestring { set; get; } //
        public int accountid { set; get; }
        public string accountname { set; get; }
        public int id { set; get; }
    }
}