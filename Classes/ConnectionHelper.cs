using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Classes
{
    public class ConnectionHelper
    {
        public static string getConnString(string name = "maindb")
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}