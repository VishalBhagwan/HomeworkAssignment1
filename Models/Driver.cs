using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeworkAssignment1.Models
{
    public class Driver
    {
        //Variables
        public string driverID {  get; set; } = Guid.NewGuid().ToString();
        public string driverImage { get; set; }
        public string driverFirstName { get; set; }
        public string driverLastName { get; set; }
        public string driverPhoneNumber { get; set; }
        public string driverServiceType { get; set; }
        public bool isFromLocalStorage { get; set; }
    }
}