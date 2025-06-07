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
            ViewBag.Service = Services.serviceTypes;
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
        public ActionResult CreateBooking(Booking booking)
        {

            TempData["Booking"] = booking;
            return RedirectToAction("ConfirmBooking");
        }

        public ActionResult ConfirmBooking() 
        {
            return View();
        }

        public ActionResult RideHistory()
        {
            return View();
        }

        //Manage
        // Update the ManagePage action to handle search
        public ActionResult ManagePage(string searchDriverName, string searchServiceType)
        {
            var drivers = Repository.GetDrivers() ?? new List<Driver>();
            var vehicles = Repository.GetVehicles() ?? new List<Vehicle>();

            // Debugging output
            Debug.WriteLine($"Total Drivers: {drivers.Count}");

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(searchDriverName))
            {
                Debug.WriteLine($"Search Term: {searchDriverName}");
                drivers = drivers.Where(d =>
                    (d.driverFirstName != null && d.driverFirstName.Equals(searchDriverName, StringComparison.OrdinalIgnoreCase)) ||
                    (d.driverLastName != null && d.driverLastName.Equals(searchDriverName, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchServiceType))
            {
                drivers = drivers.Where(d =>
                    d.driverServiceType != null && d.driverServiceType.Equals(searchServiceType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                vehicles = vehicles.Where(v =>
                    v.vehicleServiceType != null && v.vehicleServiceType.Equals(searchServiceType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var model = new Manage
            {
                Drivers = drivers,
                Vehicles = vehicles,
                searchDriverName = searchDriverName,
                searchServiceType = searchServiceType
            };

            return View(model);
        }





        [HttpPost]
        public ActionResult AddDriver(Driver driver)
        {
            driver.driverID = Guid.NewGuid().ToString();
            Repository.AddDriver(driver);
            return RedirectToAction("ManagePage");
        }

        [HttpPost]
        public ActionResult UpdateDriver(Driver driver)
        {
            Repository.UpdateDriver(driver);
            return RedirectToAction("ManagePage");
        }

        public ActionResult DeleteDriver(string id)
        {
            Repository.DeleteDriver(id);
            return RedirectToAction("ManagePage");
        }

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