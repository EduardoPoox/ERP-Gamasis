using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects.Views
{
    public class Dashboard
    {
        public int reportedincindences { set; get; }
        public int concludedincindences { set; get; }
        public int requestedmodules { set; get; }
        public int concludedmodules { set; get; }
        public int concludedincindentpercent { set; get; }
        public int concludedmodulepercent { set; get; }
        public List<Incident> inclist { set; get; } //By id
        public List<Requirement> reqlist { set; get; } //By id

        public Dashboard()
        {
            reportedincindences = 0;
            requestedmodules = 0;
            concludedmodules = 0;
            concludedincindences = 0;
        }
    }
}