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
        // GET: Booking
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult SelectService()
        {
            return View();
        }

        public ActionResult BookingForm(string serviceType)
        {
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

            // Store booking in localStorage via TempData
            var bookingData = $"{booking.serviceType}|{booking.bookingFullName}|{booking.bookingPhoneNumber}|" +
                            $"{booking.bookingPickUp}|{booking.bookingReason}|{booking.bookingPickupAddress}|" +
                            $"{booking.bookingDate}|{booking.isEmergency}|{booking.driverID}|{booking.vehicleID}";

            TempData["NewBooking"] = $"{booking.bookingID}|{bookingData}";
            TempData["BookingAdded"] = true;

            return RedirectToAction("ConfirmBooking");
        }

        public ActionResult ConfirmBooking(string bookingId)
        {
            ViewBag.BookingId = bookingId;
            return View();
        }

        public ActionResult ViewBooking(string id)
        {
            // Bookings are stored in localStorage, so we just pass the ID
            TempData["BookingId"] = id;
            return RedirectToAction("ConfirmBooking");
        }

        [HttpPost]
        public ActionResult EmergencyBooking(string serviceType)
        {
            // Get all available drivers and vehicles from localStorage simulation
            var allDrivers = new List<Driver>();
            var allVehicles = new List<Vehicle>();

            // In a real app, this would come from localStorage via JavaScript
            // For server-side simulation, we'll check if there are matching entries first
            var matchingDrivers = new List<Driver>();
            var matchingVehicles = new List<Vehicle>();

            // Check if we have matching drivers/vehicles in the repository
            //matchingDrivers = Repository.GetDrivers().Where(d => d.driverServiceType == serviceType).ToList();
            //matchingVehicles = Repository.GetVehicles().Where(v => v.vehicleServiceType == serviceType).ToList();

            // If no matches in repository, create emergency entries
            if (matchingDrivers.Count == 0)
            {
                matchingDrivers.Add(new Driver
                {
                    driverID = "EMG-DRIVER-" + Guid.NewGuid().ToString().Substring(0, 4),
                    driverFirstName = "Emergency",
                    driverLastName = "Driver",
                    driverPhoneNumber = "111-111-1111",
                    driverServiceType = serviceType
                });
            }

            if (matchingVehicles.Count == 0)
            {
                matchingVehicles.Add(new Vehicle
                {
                    vehicleID = "EMG-VEHICLE-" + Guid.NewGuid().ToString().Substring(0, 4),
                    vehicleType = "Emergency " + serviceType + " Vehicle",
                    vehicleRegistration = "EMG" + new Random().Next(100, 999),
                    vehicleServiceType = serviceType
                });
            }

            // Select random driver and vehicle
            var random = new Random();
            var selectedDriver = matchingDrivers[random.Next(matchingDrivers.Count)];
            var selectedVehicle = matchingVehicles[random.Next(matchingVehicles.Count)];

            // Create emergency booking
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

            // Store booking in localStorage via TempData
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

        public ActionResult RideHistory()
        {
            // Ride history is handled client-side with localStorage
            return View();
        }

        // Manage Page - No repository data needed
        public ActionResult ManagePage(string searchDriverName, string searchServiceType)
        {
            return View(new Manage
            {
                searchDriverName = searchDriverName,
                searchServiceType = searchServiceType
            });
        }

        // Driver CRUD Operations
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
                // Generate ID if not set
                if (string.IsNullOrEmpty(driver.driverID))
                {
                    driver.driverID = Guid.NewGuid().ToString();
                }

                // Prepare data for localStorage
                var driverData = $"{driver.driverFirstName}|{driver.driverLastName}|" +
                               $"{driver.driverPhoneNumber}|{driver.driverServiceType}";

                if (!string.IsNullOrEmpty(driver.driverImage))
                {
                    driverData += $"|{driver.driverImage}";
                }

                // Store in TempData for client-side storage
                TempData["NewDriver"] = $"{driver.driverID}|{driverData}";
                TempData["DriverAdded"] = true;

                return RedirectToAction("ManagePage");
            }
            return View(driver);
        }

        [HttpGet]
        public ActionResult EditDriver(string id, bool fromLocal = false)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("ManagePage");

            // Create a driver model with the ID and fromLocal flag
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
            if (ModelState.IsValid)
            {
                if (isFromLocalStorage)
                {
                    // Prepare data for localStorage update
                    var driverData = $"{driver.driverFirstName}|{driver.driverLastName}|" +
                                   $"{driver.driverPhoneNumber}|{driver.driverServiceType}";

                    if (!string.IsNullOrEmpty(driver.driverImage))
                    {
                        driverData += $"|{driver.driverImage}";
                    }

                    TempData["DriverUpdated"] = true;
                    TempData["UpdatedDriver"] = $"{driver.driverID}|{driverData}";
                }

                return RedirectToAction("ManagePage");
            }
            return View("EditDriver", driver);
        }

        [HttpPost]
        public ActionResult DeleteDriver(string id, bool isFromLocalStorage)
        {
            if (isFromLocalStorage)
            {
                TempData["DriverDeleted"] = id;
            }
            return RedirectToAction("ManagePage");
        }

        // Vehicle CRUD Operations
        [HttpGet]
        public ActionResult AddVehicle()
        {
            return View(new Vehicle());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVehicle(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                // Generate ID if not set
                if (string.IsNullOrEmpty(vehicle.vehicleID))
                {
                    vehicle.vehicleID = Guid.NewGuid().ToString();
                }

                // Prepare data for localStorage
                var vehicleData = $"{vehicle.vehicleType}|{vehicle.vehicleRegistration}|{vehicle.vehicleServiceType}";

                if (!string.IsNullOrEmpty(vehicle.vehicleImage))
                {
                    vehicleData += $"|{vehicle.vehicleImage}";
                }

                // Store in TempData for client-side storage
                TempData["NewVehicle"] = $"{vehicle.vehicleID}|{vehicleData}";
                TempData["VehicleAdded"] = true;

                return RedirectToAction("ManagePage");
            }
            return View(vehicle);
        }

        [HttpGet]
        public ActionResult EditVehicle(string id, bool fromLocal = false)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("ManagePage");

            // For localStorage vehicles, we just pass the ID
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
            if (ModelState.IsValid)
            {
                if (isFromLocalStorage)
                {
                    // Prepare data for localStorage update
                    var vehicleData = $"{vehicle.vehicleType}|{vehicle.vehicleRegistration}|{vehicle.vehicleServiceType}";

                    if (!string.IsNullOrEmpty(vehicle.vehicleImage))
                    {
                        vehicleData += $"|{vehicle.vehicleImage}";
                    }

                    TempData["VehicleUpdated"] = true;
                    TempData["UpdatedVehicle"] = $"{vehicle.vehicleID}|{vehicleData}";
                }

                return RedirectToAction("ManagePage");
            }
            return View("EditVehicle", vehicle);
        }

        [HttpPost]
        public ActionResult DeleteVehicle(string id, bool isFromLocalStorage)
        {
            if (isFromLocalStorage)
            {
                TempData["VehicleDeleted"] = id;
            }
            return RedirectToAction("ManagePage");
        }
    }
}