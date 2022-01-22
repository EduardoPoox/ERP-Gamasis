using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Programmer
    {
        public int id { set; get; }
        public int idpro { set; get; }
        public int idinc { set; get; }
        public string fullname { set; get; }
        public string email { set; get; }
        public string cellphone { set; get; }
    }
}