using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C_WebApp_.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string type)
        {
            if(string.IsNullOrEmpty(type))
                return Content(string.Format(@"Record type not provided, Please provide the record type as query string parameter.
                                    Example : {0}?type=Red", Url.Action("Index", "Home", null, Request.Url.Scheme)));
            return View();
        }

    }
}