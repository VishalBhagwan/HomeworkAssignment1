using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeworkAssignment1.Models
{
    public class DriverVehicleModel
    {
        //Variables
        public List<Driver> Drivers { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public string ServiceType { get; set; }
    }
}