using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using TwitterMVC.ViewModels;
using System.Web;
using Microsoft.AspNetCore.Http;
using TwitterMVC.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Linq;
using TwitterMVC.Services;

namespace TwitterMVC.Controllers
{
    [Authorize]
    public class HomePageController : Controller
    {
        private ITwittsService _twittsService;

        public  HomePageController(ITwittsService twittsService)
        {
            _twittsService = twittsService;
        }

        public async Task<IActionResult> Index()
        {
            HomePageModel model = new HomePageModel();
            model.Twitts = await _twittsService.getUserTwitts();

            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewTwitt(HomePageModel model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                _twittsService.addTwitt(file, model.Text);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult RemoveTwitt(string id)
        {
            _twittsService.deleteTwitt(id);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> EditTwitt(string id)
        {
            HomePageModel model = new HomePageModel();

            model.Twitts = await _twittsService.getUserTwitts();

            TwittModel twitt = model.Twitts.Find(t => t.RowKey == id);

            model.EditedTwittId = twitt.RowKey;
            model.ImgUrl = twitt.ImageUrl;
            model.Text = twitt.Text;

            return View("Index", model);
        }

        public ActionResult SaveTwitt(HomePageModel model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
              
                _twittsService.saveTwitt(file, model.Text, model.ImgUrl, model.EditedTwittId);

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
