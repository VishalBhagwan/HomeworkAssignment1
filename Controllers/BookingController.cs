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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectService() 
        {
            ViewBag.Service = Services.serviceTypes;
            return View();
        }
    }
}