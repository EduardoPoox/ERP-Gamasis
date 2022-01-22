using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class File
    {
        public int id { set; get;  }
        public int idrel { set; get; }
        public int type { set; get; }
        public string name { set; get; }
        public byte[] data { set; get; }
        public string mime { set; get; }
        public string extension { set; get; }

        public File()
        {
            id = 0;
            idrel = 0;
            type = 0;
            name = "";
            data = null;
            mime = "";
            extension = "";
        }
    }
}