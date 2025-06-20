using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeworkAssignment1.Models
{
    public class Vehicle
    {
        public string vehicleID { get; set; } = Guid.NewGuid().ToString();
        public string vehicleImage {  get; set; }
        public string vehicleType { get; set; }
        public string vehicleRegistration {  get; set; }
        public string vehicleServiceType { get; set; }
        public bool isFromLocalStorage { get; set; }
    }
}