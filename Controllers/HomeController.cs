using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StellarVoteApp.Core.Services.Contracts;
using StellarVoteApp.Data.Services.Contracts;
using StellarVoteApp.Models;

namespace StellarVoteApp.Controllers
{
    public class HomeController : Controller
    {
        private IUserService userService;
        private IVoteService voteService;
        public HomeController(IUserService userService, IVoteService voteService)
        {
            this.userService = userService;
            this.voteService = voteService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var hasStellarAccount = await this.userService.HasUserVotingAccount(userId);
            if (hasStellarAccount)
            {
                var stellarAccount = await this.userService.GetAccountDetails(userId);
                var userAccountInfo = await this.voteService.GetUserAccountInformation(stellarAccount.AccountId);
                ViewBag.HasVoted = userAccountInfo.HasVoted;
            }
            else
            {
                ViewBag.HasVoted = false;
            }
            
            return View(hasStellarAccount);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
