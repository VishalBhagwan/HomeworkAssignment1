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
            if (!Services.serviceTypes.Contains(serviceType))
            {
                return RedirectToAction("SelectService");
            }

            ViewBag.Services = serviceType;
            return View();
        }

        public ActionResult ConfirmedBooking() 
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