using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Services.Description;

namespace HomeworkAssignment1.Models
{
    public class Booking
    {
        //Variables
        public string serviceType { get; set; }
        public string bookingID {  get; set; } = Guid.NewGuid().ToString();
        public string bookingFullName { get; set; }
        public string bookingPhoneNumber { get; set; }
        public DateTime bookingDate { get; set; } = DateTime.Now;
        public DateTime bookingPickUp {  get; set; }
        public string bookingReason { get; set; }
        public string vehicleID { get; set; }
        public string driverID { get; set; }
        public string bookingPickupAddress { get; set; }
        public bool isEmergency { get; set; } = false;

        public Driver driver { get; set; }
        public Vehicle vehicle { get; set; }
    }
}