using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Alfas
    {
        public Alfas()
        {
            tcn = "";
            name = "";
            phone = "";
            email = "";
            cellphone = "";
            comesfrom = "";
            lastname = "";
            name = "";
            rolstring = "";

        }
        public string tcn { set; get; }
        public int iddata { set; get; }
        public int idfromproject { set; get; }
        public string name { set; get; }
        public string lastname { set; get; }
        public string email { set; get; }
        public string cellphone { set; get; }
        public string phone { set; get; }
        public string rolstring { set; get; }
        public bool activate { set; get; }
        public int rol { set; get; }
        public string comesfrom { set; get; }
    }
}