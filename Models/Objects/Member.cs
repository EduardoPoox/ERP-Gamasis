using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Member
    {
        public int id { set; get; }
        public int idmember { set; get; }
        public int idrel { set; get; }
        public string name { set; get; }
        public string mail { set; get; }
        public string comesfrom { set; get; }
        public string cellphone { set; get; }
    }
}