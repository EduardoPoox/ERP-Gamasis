using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Setting
    {
        public int id { set; get; }
        public int idrel { set; get; }
        public int type { set; get; }
        public string value { set; get; }
        public string setting { set; get; }
    }
}