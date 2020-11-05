﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarVoteApp.Core.Services.Contracts;
using StellarVoteApp.Data.Services.Contracts;

namespace StellarVoteApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private IVoteService voteService;
        private IUserService userService;

        public UserController(IVoteService voteService, IUserService userService)
        {
            this.voteService = voteService;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var stellarAccount = await this.userService.GetAccountDetails(userId);
            var userAccountInformation = await this.voteService.GetUserAccountInformation(stellarAccount.AccountId);
            return View(userAccountInformation);
        }
    }
}
