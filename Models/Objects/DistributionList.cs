using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class DistributionList
    {
        public DistributionList()
        {
            members = new List<Member>();
        }
        public int id { set; get; }
        public string name { set; get; }
        public string description { set; get; }
        public List<Member> members { set; get; }

    }
}