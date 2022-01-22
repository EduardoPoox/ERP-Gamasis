using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Comment
    {
        public Comment() { }
        public int accountid { set; get; }
        public string accountname { set; get; }
        public string comment { set; get; }
        public string date { set; get; }
        public int id { set; get; }
        public int idrel { set; get; }
        public int type { set; get; }
        public List<dynamic> replies { set; get; }
    }
}