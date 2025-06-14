﻿using HomeworkAssignment1.Models;
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
                Drivers = Repository.GetDrivers()
                    .Where(d => d.driverServiceType == serviceType)
                    .ToList(),
                Vehicles = Repository.GetVehicles()
                    .Where(v => v.vehicleServiceType == serviceType)
                    .ToList()
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
        public ActionResult EmergencyBooking(string serviceType)
        {
            // Get all available drivers (from both repository and Request.Cookies)
            var allDrivers = new List<Driver>();

            // 1. Get drivers from repository
            var repoDrivers = Repository.GetDrivers()
                .Where(d => d.driverServiceType == serviceType)
                .ToList();
            allDrivers.AddRange(repoDrivers);

            // 2. Get drivers from Request.Cookies (simulating localStorage access)
            for (int i = 0; i < Request.Cookies.Count; i++)
            {
                var key = Request.Cookies.Keys[i];
                if (key != null && key.StartsWith("driver_") && !key.Contains("|"))
                {
                    var driverData = Request.Cookies[key]?.Value?.Split('|');
                    if (driverData != null && driverData.Length == 4 && driverData[3] == serviceType)
                    {
                        allDrivers.Add(new Driver
                        {
                            driverID = key.Replace("driver_", ""),
                            driverFirstName = driverData[0],
                            driverLastName = driverData[1],
                            driverPhoneNumber = driverData[2],
                            driverServiceType = driverData[3]
                        });
                    }
                }
            }

            // Get all available vehicles (from both repository and Request.Cookies)
            var allVehicles = new List<Vehicle>();

            // 1. Get vehicles from repository
            var repoVehicles = Repository.GetVehicles()
                .Where(v => v.vehicleServiceType == serviceType)
                .ToList();
            allVehicles.AddRange(repoVehicles);

            // 2. Get vehicles from Request.Cookies (simulating localStorage access)
            for (int i = 0; i < Request.Cookies.Count; i++)
            {
                var key = Request.Cookies.Keys[i];
                if (key != null && key.StartsWith("vehicle_") && !key.Contains("|"))
                {
                    var vehicleData = Request.Cookies[key]?.Value?.Split('|');
                    if (vehicleData != null && vehicleData.Length == 3 && vehicleData[2] == serviceType)
                    {
                        allVehicles.Add(new Vehicle
                        {
                            vehicleID = key.Replace("vehicle_", ""),
                            vehicleType = vehicleData[0],
                            vehicleRegistration = vehicleData[1],
                            vehicleServiceType = vehicleData[2]
                        });
                    }
                }
            }

            // Ensure we always have at least one driver and one vehicle
            if (allDrivers.Count == 0)
            {
                // Create emergency driver if none available
                allDrivers.Add(new Driver
                {
                    driverID = "EMG-DRIVER-" + Guid.NewGuid().ToString().Substring(0, 4),
                    driverFirstName = "Emergency",
                    driverLastName = "Driver",
                    driverPhoneNumber = "111-111-1111",
                    driverServiceType = serviceType
                });
            }

            if (allVehicles.Count == 0)
            {
                // Create emergency vehicle if none available
                allVehicles.Add(new Vehicle
                {
                    vehicleID = "EMG-VEHICLE-" + Guid.NewGuid().ToString().Substring(0, 4),
                    vehicleType = "Emergency " + serviceType + " Vehicle",
                    vehicleRegistration = "EMG" + new Random().Next(100, 999),
                    vehicleServiceType = serviceType
                });
            }

            // Select random driver and vehicle
            var random = new Random();
            var randomDriver = allDrivers[random.Next(allDrivers.Count)];
            var randomVehicle = allVehicles[random.Next(allVehicles.Count)];

            // Create emergency booking
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
                driver = randomDriver,
                vehicle = randomVehicle
            };

            Repository.AddBooking(booking);

            // Return success with selected IDs
            return Content(
                $"DriverSelected={randomDriver.driverID}&VehicleSelected={randomVehicle.vehicleID}",
                "text/plain"
            );
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