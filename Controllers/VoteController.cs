using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StellarVoteApp.Controllers
{
    [Authorize]
    public class VoteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
