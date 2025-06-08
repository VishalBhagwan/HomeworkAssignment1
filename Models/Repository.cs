using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeworkAssignment1.Models
{
    public class Repository
    {
        public static List<Driver> drivers = new List<Driver>
        {

            new Driver{driverID = "1", driverImage = "", driverFirstName = "A", driverLastName = "B", driverPhoneNumber = "1212121212", driverServiceType = "ALS (Advances Life Support)" },
            new Driver{driverID = "2", driverImage = "", driverFirstName = "C", driverLastName = "D", driverPhoneNumber = "3434343434", driverServiceType = "BLS" },
            new Driver{driverID = "3", driverImage = "", driverFirstName = "E", driverLastName = "F", driverPhoneNumber = "5656565656", driverServiceType = "AS" }

        };

        public static List<Vehicle> vehicles = new List<Vehicle>
        {

            new Vehicle{vehicleID = "1", vehicleImage = "", vehicleType = "A", vehicleRegistration = "B", vehicleServiceType = "ALS (Advances Life Support)" },
            new Vehicle{vehicleID = "2", vehicleImage = "", vehicleType = "C", vehicleRegistration = "D", vehicleServiceType = "BLS" },

        };

        public static List<Booking> bookings = new List<Booking>();
        public static List<Booking> GetBookings() 
        { 
            return bookings; 
        }
        public static void AddBooking(Booking booking)
        {
            bookings.Add(booking);
        }
        public static Booking GetBookingByID(string bID)
        {
            return bookings.FirstOrDefault(b => b.bookingID == bID);
        }

        //Driver and Vehicle CRUD
        //Driver
        public static List<Driver> GetDrivers() => drivers;
        public static Driver GetDriver(string dID) => drivers.FirstOrDefault(d => d.driverID == dID);
        public static void AddDriver(Driver driver) => drivers.Add(driver);
        public static void UpdateDriver(Driver updateDriver)
        {
            var index = drivers.FindIndex(d => d.driverID == updateDriver.driverID);
            if (index >= 0)
            {
                drivers[index] = updateDriver;
            }
        }
        public static void DeleteDriver(string dID) => drivers.RemoveAll(d => d.driverID == dID);

        //Vehicle
        public static List<Vehicle> GetVehicles() => vehicles;
        public static Vehicle GetVehicle(string vID) => vehicles.FirstOrDefault(v => v.vehicleID == vID);
        public static void AddVehicle(Vehicle vehicle) => vehicles.Add(vehicle);
        public static void UpdateVehicle(Vehicle updateVehicle)
        {
            var index = vehicles.FindIndex(v => v.vehicleID == updateVehicle.vehicleID);
            if (index >= 0)
            {
                vehicles[index] = updateVehicle;
            }
        }
        public static void DeleteVehicle(string vID) => vehicles.RemoveAll(v => v.vehicleID == vID);
    }
}