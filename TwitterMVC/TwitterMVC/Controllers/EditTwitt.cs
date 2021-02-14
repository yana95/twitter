using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterMVC.Controllers
{
    public class EditTwitt : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
