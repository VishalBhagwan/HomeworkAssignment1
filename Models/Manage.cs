using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeworkAssignment1.Models
{
    public class Manage
    {
        public List<Driver> Drivers { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public string searchDriverName { get; set; }
        public string searchServiceType { get; set; }
    }
}