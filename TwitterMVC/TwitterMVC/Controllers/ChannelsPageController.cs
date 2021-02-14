using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TwitterMVC.Models;
using TwitterMVC.Services;
using TwitterMVC.ViewModels;

namespace TwitterMVC.Controllers
{
    [Authorize]
    public class ChannelsPageController : Controller
    {
        private IUsersService _usersService;
        public ChannelsPageController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<ActionResult> Index()
        {
            ChannelsPageModel model = new ChannelsPageModel();
            model.Channels = await _usersService.getUsersExceptCurrrent();
            model.CurrentUser = await _usersService.getCurrentUser();

            return View(model);
        }

        public async Task<ActionResult> Follow(string id)
        {
            await _usersService.followTo(id);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Unfollow(string id)
        {
            await _usersService.unfollow(id);

            return RedirectToAction("Index");
        }
    }
}
