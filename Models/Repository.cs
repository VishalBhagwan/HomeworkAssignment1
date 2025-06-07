using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeworkAssignment1.Models
{
    public class Repository
    {
        public static List<Driver> GetDrivers()
        {
            return new List<Driver>
            {
                new Driver{driverID = "1", driverImage = "", driverFirstName = "A", driverLastName = "B", driverPhoneNumber = "1212121212", driverServiceType = "ALS (Advances Life Support)" },
                new Driver{driverID = "2", driverImage = "", driverFirstName = "C", driverLastName = "D", driverPhoneNumber = "3434343434", driverServiceType = "BLS" },
                new Driver{driverID = "3", driverImage = "", driverFirstName = "E", driverLastName = "F", driverPhoneNumber = "5656565656", driverServiceType = "AS" }
            };
        }

        public static List<Vehicle> GetVehicles()
        {
            return new List<Vehicle>
            {
                new Vehicle{vehicleID = "1", vehicleImage = "", vehicleType = "A", vehicleRegistration = "B", vehicleServiceType = "ALS (Advances Life Support)" },
                new Vehicle{vehicleID = "2", vehicleImage = "", vehicleType = "C", vehicleRegistration = "D", vehicleServiceType = "BLS" },
            };
        }
    }
}