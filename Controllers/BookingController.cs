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

        public ActionResult ConfirmBooking() 
        {
            return View();
        }

        public ActionResult RideHistory()
        {
            return View();
        }

        public ActionResult ManagePage()
        {
            return View();
        }
    }
}