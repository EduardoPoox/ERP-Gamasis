using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Account
    {
        public int id { set; get; }
        public string ip { set; get; }
        public string username { set; get; }
        public string password { set; get; }
        public int rol { set; get; }
        public string rolstring { set; get; }
        public Rol rolObject { set; get; }
        public byte[] avatar { set; get; }
        public bool isfromanotherdb { set; get; }
        public bool isactive { set; get; }
        public bool companyStatus { set; get; }
        public List<string> privileges { set; get; }
        public List<Assignation> assignations { set; get; }
        public Alfas data { set; get; }
        public string sessionStartedAt { set; get; }
        public bool hasPrivilege(string role)
        {
            if (role == "")
                return false;

            if (this.privileges.Any(r => role.Contains(r)))
                return true;
            else
                return false;
        }

        public bool hasAccessToModule(string module)
        {
            if (module == "")
                return false;
            foreach (Assignation ass in this.assignations)
            {
                if (ass.type == 1)
                {
                    if (ass.value == module)
                        return true;
                }
            }
            return false;
        }

        public Account()
        {
            rolstring = "";
            data = new Alfas();
            rolObject = new Rol();
            privileges = new List<string>();
            assignations = new List<Assignation>();
        }
    }
}