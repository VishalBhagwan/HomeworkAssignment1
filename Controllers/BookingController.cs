using HomeworkAssignment1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeworkAssignment1.Controllers
{
    public class BookingController : Controller
    {
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult SelectService()
        {
            return View();
        }

        //Booking form
        public ActionResult BookingForm(string serviceType)
        {
            //Booking form of specific service type (all booking forms looks the same)
            if (!Services.serviceTypes.Contains(serviceType))
            {
                return RedirectToAction("SelectService");
            }

            ViewBag.ServiceType = serviceType;
            return View(new DriverVehicleModel { ServiceType = serviceType });
        }

        [HttpPost]
        public ActionResult CreateBooking()
        {
            //Gets data from form
            var booking = new Booking
            {
                serviceType = Request.Form["serviceType"],
                bookingFullName = Request.Form["bookingFullName"],
                bookingPhoneNumber = Request.Form["bookingPhoneNumber"],
                bookingPickUp = DateTime.Parse(Request.Form["bookingPickUp"]),
                bookingReason = Request.Form["bookingReason"],
                bookingPickupAddress = Request.Form["bookingPickupAddress"],
                bookingDate = DateTime.Now,
                isEmergency = false,
                driverID = Request.Form["driverID"],
                vehicleID = Request.Form["vehicleID"]
            };

            //Store booking in localStorage
            var bookingData = $"{booking.serviceType}|{booking.bookingFullName}|{booking.bookingPhoneNumber}|" +
                            $"{booking.bookingPickUp}|{booking.bookingReason}|{booking.bookingPickupAddress}|" +
                            $"{booking.bookingDate}|{booking.isEmergency}|{booking.driverID}|{booking.vehicleID}";

            TempData["NewBooking"] = $"{booking.bookingID}|{bookingData}";
            TempData["BookingAdded"] = true;

            return RedirectToAction("ConfirmBooking");
        }

        public ActionResult ConfirmBooking(string bookingId)
        {
            //Returns the confirmed booking page of the booking with the relevant ID
            ViewBag.BookingId = bookingId;
            return View();
        }

        public ActionResult ViewBooking(string id)
        {
            //Bookings are stored in localStorage, so just pass the ID
            TempData["BookingId"] = id;
            return RedirectToAction("ConfirmBooking");
        }

        [HttpPost]
        public ActionResult EmergencyBooking(string serviceType)
        {
            var allDrivers = new List<Driver>();
            var allVehicles = new List<Vehicle>();

            var matchingDrivers = new List<Driver>();
            var matchingVehicles = new List<Vehicle>();         

            //Select random driver and vehicle
            var random = new Random();
            var selectedDriver = matchingDrivers[random.Next(matchingDrivers.Count)];
            var selectedVehicle = matchingVehicles[random.Next(matchingVehicles.Count)];

            //Create the emergency booking
            var booking = new Booking
            {
                serviceType = serviceType,
                bookingID = Guid.NewGuid().ToString(),
                bookingFullName = "EMERGENCY PATIENT",
                bookingPhoneNumber = "000-000-0000",
                bookingPickUp = DateTime.Now.AddMinutes(15),
                bookingReason = "Emergency",
                bookingPickupAddress = "Emergency Location",
                bookingDate = DateTime.Now,
                isEmergency = true,
                driverID = selectedDriver.driverID,
                vehicleID = selectedVehicle.vehicleID
            };

            //Store booking in localStorage
            var bookingData = $"{booking.serviceType}|{booking.bookingFullName}|{booking.bookingPhoneNumber}|" +
                            $"{booking.bookingPickUp}|{booking.bookingReason}|{booking.bookingPickupAddress}|" +
                            $"{booking.bookingDate}|{booking.isEmergency}|{booking.driverID}|{booking.vehicleID}";

            TempData["NewBooking"] = $"{booking.bookingID}|{bookingData}";
            TempData["BookingAdded"] = true;

            return Content(
                $"DriverSelected={selectedDriver.driverID}&VehicleSelected={selectedVehicle.vehicleID}",
                "text/plain"
            );
        }

        //Ride History
        public ActionResult RideHistory()
        {
            return View();
        }

        //Manage Page
        public ActionResult ManagePage(string searchDriverName, string searchServiceType)
        {
            return View(new Manage
            {
                searchDriverName = searchDriverName,
                searchServiceType = searchServiceType
            });
        }

        //Driver CRUDs
        [HttpGet]
        public ActionResult AddDriver()
        {
            return View(new Driver());
        }

        //Gets the driver object from the form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDriver(Driver driver)
        {
            //Check if the data passed all validation rules
            if (ModelState.IsValid)
            {
                //Generate ID if missing
                if (string.IsNullOrEmpty(driver.driverID))
                {
                    driver.driverID = Guid.NewGuid().ToString();
                }

                //Prepares how the data is saved in localStorage
                var driverData = $"{driver.driverFirstName}|{driver.driverLastName}|" + $"{driver.driverPhoneNumber}|{driver.driverServiceType}";

                //Image path
                if (!string.IsNullOrEmpty(driver.driverImage))
                {
                    driverData += $"|{driver.driverImage}";
                }

                //JavaScript in the view will read the TempData and then save the data in localStorage
                TempData["NewDriver"] = $"{driver.driverID}|{driverData}";
                TempData["DriverAdded"] = true;

                return RedirectToAction("ManagePage");
            }
            //If validation fails it redisplays the form
            return View(driver);
        }

        [HttpGet]
        public ActionResult EditDriver(string id, bool fromLocal = false)
        {
            //Return to manage page if ID cannot be found
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("ManagePage");
            }
                
            //Prepares the data to be edited
            var driver = new Driver
            {
                driverID = id,
                isFromLocalStorage = fromLocal
            };

            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDriver(Driver driver, bool isFromLocalStorage)
        {
            //Check if the data passed all validation rules
            if (ModelState.IsValid)
            {
                if (isFromLocalStorage)
                {
                    //Formats the driver data and how it will be saved/stored
                    var driverData = $"{driver.driverFirstName}|{driver.driverLastName}|" +
                                   $"{driver.driverPhoneNumber}|{driver.driverServiceType}";

                    if (!string.IsNullOrEmpty(driver.driverImage))
                    {
                        driverData += $"|{driver.driverImage}";
                    }

                    TempData["DriverUpdated"] = true;
                    TempData["UpdatedDriver"] = $"{driver.driverID}|{driverData}";
                }
                //Go back to the manage page if eveyrthing was successful
                return RedirectToAction("ManagePage");
            }
            //If validation fails it redisplays the form
            return View("EditDriver", driver);
        }

        [HttpPost]
        public ActionResult DeleteDriver(string id, bool isFromLocalStorage)
        {
            //Gets the ID so that it can be removed from localStorage
            if (isFromLocalStorage)
            {
                TempData["DriverDeleted"] = id;
            }
            return RedirectToAction("ManagePage");
        }

        //Vehicle CRUDs
        [HttpGet]
        public ActionResult AddVehicle()
        {
            return View(new Vehicle());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVehicle(Vehicle vehicle)
        {
            //Check if the data passed all validation rules
            if (ModelState.IsValid)
            {
                //Generate ID if missing
                if (string.IsNullOrEmpty(vehicle.vehicleID))
                {
                    vehicle.vehicleID = Guid.NewGuid().ToString();
                }

                //Prepares how the data is saved in localStorage
                var vehicleData = $"{vehicle.vehicleType}|{vehicle.vehicleRegistration}|{vehicle.vehicleServiceType}";

                //Image path
                if (!string.IsNullOrEmpty(vehicle.vehicleImage))
                {
                    vehicleData += $"|{vehicle.vehicleImage}";
                }

                //JavaScript in the view will read the TempData and then save the data in localStorage
                TempData["NewVehicle"] = $"{vehicle.vehicleID}|{vehicleData}";
                TempData["VehicleAdded"] = true;

                return RedirectToAction("ManagePage");
            }
            //If validation fails it redisplays the form
            return View(vehicle);
        }

        [HttpGet]
        public ActionResult EditVehicle(string id, bool fromLocal = false)
        {
            //Return to manage page if ID cannot be found
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("ManagePage");
            }

            //Prepares the data to be edited
            return View(new Vehicle
            {
                vehicleID = id,
                isFromLocalStorage = fromLocal
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateVehicle(Vehicle vehicle, bool isFromLocalStorage)
        {
            //Check if the data passed all validation rules
            if (ModelState.IsValid)
            {
                if (isFromLocalStorage)
                {
                    //Formats the driver data and how it will be saved/stored
                    var vehicleData = $"{vehicle.vehicleType}|{vehicle.vehicleRegistration}|{vehicle.vehicleServiceType}";

                    if (!string.IsNullOrEmpty(vehicle.vehicleImage))
                    {
                        vehicleData += $"|{vehicle.vehicleImage}";
                    }

                    TempData["VehicleUpdated"] = true;
                    TempData["UpdatedVehicle"] = $"{vehicle.vehicleID}|{vehicleData}";
                }
                //Go back to the manage page if eveyrthing was successful
                return RedirectToAction("ManagePage");
            }
            //If validation fails it redisplays the form
            return View("EditVehicle", vehicle);
        }

        [HttpPost]
        public ActionResult DeleteVehicle(string id, bool isFromLocalStorage)
        {
            //Gets the ID so that it can be removed from localStorage
            if (isFromLocalStorage)
            {
                TempData["VehicleDeleted"] = id;
            }
            return RedirectToAction("ManagePage");
        }
    }
}