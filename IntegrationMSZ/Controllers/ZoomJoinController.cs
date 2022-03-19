using IntegrationMSZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntegrationMSZ.Controllers
{
    public class ZoomJoinController : Controller
    {
        // GET: ZoomJoin
        public ActionResult Index(ApplicationModel model)
        {
            model.type = 1;
            return View(model);
        }
    }
}