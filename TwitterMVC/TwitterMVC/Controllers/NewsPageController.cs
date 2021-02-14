using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterMVC.Services;
using TwitterMVC.ViewModels;

namespace TwitterMVC.Controllers
{
    [Authorize]
    public class NewsPageController : Controller
    {
        private ITwittsService _twittsService;

        public NewsPageController(ITwittsService twittsService)
        {
            _twittsService = twittsService;
        }

        public async Task<IActionResult> Index()
        {
            NewsPageModel model = new NewsPageModel();
            model.Twitts = await _twittsService.getCurrentUserNews();

            return View(model);
        }
    }
}
