using HomeworkAssignment1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeworkAssignment1.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult SelectService() 
        {
            //ViewBag.Service = Services.serviceTypes;
            return View();
        }

        public ActionResult BookingForm(string serviceType)
        {
            var viewModel = new DriverVehicleModel
            {
                Drivers = Repository.GetDrivers().Where(d => d.driverServiceType == serviceType).ToList(),
                Vehicles = Repository.GetVehicles().Where(v => v.vehicleServiceType == serviceType).ToList(),
            };


            if (!Services.serviceTypes.Contains(serviceType))
            {
                return RedirectToAction("SelectService");
            }

            ViewBag.ServiceType = serviceType;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateBooking()
        {
            var booking = new Booking
            {
                serviceType = Request.Form["serviceType"],
                bookingFullName = Request.Form["bookingFullName"],
                // ... other properties ...
                driver = Repository.GetDriver(Request.Form["driverID"]),
                vehicle = Repository.GetVehicle(Request.Form["vehicleID"])
            };
            Repository.AddBooking(booking); // Add this line
                                            // Add the booking to local storage
            TempData["Booking"] = booking;
            return RedirectToAction("ConfirmBooking");
        }

        public ActionResult ConfirmBooking()
        {
            return View();
        }

        public ActionResult ViewBooking(string id)
        {
            var booking = Repository.GetBookingByID(id);
            if (booking == null)
            {
                return HttpNotFound();
            }

            TempData["Booking"] = booking;
            return RedirectToAction("ConfirmBooking");
        }

        [HttpPost]
        public ActionResult EmergencyBooking(string serviceType, string driverId, string vehicleId)
        {
            var driver = Repository.GetDriver(driverId);
            var vehicle = Repository.GetVehicle(vehicleId);

            var booking = new Booking
            {
                serviceType = serviceType,
                bookingID = "EMG-" + Guid.NewGuid().ToString().Substring(0, 8),
                bookingFullName = "EMERGENCY PATIENT",
                bookingPhoneNumber = "000-000-0000",
                bookingPickUp = DateTime.Now.AddMinutes(15),
                bookingReason = "Emergency",
                bookingPickupAddress = "Emergency Location",
                bookingDate = DateTime.Now,
                isEmergency = true,
                driver = driver,
                vehicle = vehicle
            };

            Repository.AddBooking(booking);

            return Json(new { success = true });
        }

        public ActionResult RideHistory()
        {
            var bookings = Repository.GetBookings().OrderByDescending(b => b.bookingDate).ToList();

            return View(bookings);
        }

        //Manage
        // Update the ManagePage action to handle search
        public ActionResult ManagePage(string searchDriverName, string searchServiceType)
        {
            var model = new Manage
            {
                Drivers = Repository.GetDrivers()
                    .Where(d => string.IsNullOrEmpty(searchDriverName) ||
                           (d.driverFirstName + " " + d.driverLastName).Contains(searchDriverName))
                    .ToList(),
                Vehicles = Repository.GetVehicles()
                    .Where(v => string.IsNullOrEmpty(searchServiceType) ||
                           v.vehicleServiceType.Contains(searchServiceType))
                    .ToList(),
                searchDriverName = searchDriverName,
                searchServiceType = searchServiceType
            };

            return View(model);
        }





        // In BookingController.cs

        // Add Driver
        [HttpGet]
        public ActionResult AddDriver()
        {
            return View(new Driver());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDriver(Driver driver)
        {
            if (ModelState.IsValid)
            {
                // Add to repository
                Repository.AddDriver(driver);

                // Set flag to indicate successful addition
                TempData["DriverAdded"] = true;

                // Store in TempData for client-side storage
                TempData["NewDriver"] = $"{driver.driverID}|{driver.driverFirstName}|{driver.driverLastName}|{driver.driverPhoneNumber}|{driver.driverServiceType}";

                return RedirectToAction("ManagePage");
            }
            return View(driver);
        }

        // Edit Driver
        // In your BookingController.cs

        [HttpGet]
        public ActionResult EditDriver(string id)
        {
            // Get driver from repository
            var driver = Repository.GetDriver(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDriver(Driver driver)
        {
            if (ModelState.IsValid)
            {
                // Update the repository
                Repository.UpdateDriver(driver);

                // Store in TempData for client-side storage
                TempData["DriverUpdated"] = true;
                TempData["UpdatedDriver"] = $"{driver.driverID}|{driver.driverFirstName}|{driver.driverLastName}|{driver.driverPhoneNumber}|{driver.driverServiceType}";

                return RedirectToAction("ManagePage");
            }
            return View("EditDriver", driver);
        }

        // Delete Driver
        [HttpPost]
        public ActionResult DeleteDriver(string id)
        {
            Repository.DeleteDriver(id);
            return RedirectToAction("ManagePage");
        }

        // Similar actions for Vehicle (AddVehicle, EditVehicle, DeleteVehicle)

        [HttpPost]
        public ActionResult AddVehicle(Vehicle vehicle)
        {
            vehicle.vehicleID = Guid.NewGuid().ToString();
            Repository.AddVehicle(vehicle);
            return RedirectToAction("ManagePage");
        }

        [HttpPost]
        public ActionResult UpdateVehicle(Vehicle vehicle)
        {
            Repository.UpdateVehicle(vehicle);
            return RedirectToAction("ManagePage");
        }

        public ActionResult DeleteVehicle(string id)
        {
            Repository.DeleteVehicle(id);
            return RedirectToAction("ManagePage");
        }
    }
}