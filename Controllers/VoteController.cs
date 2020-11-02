using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StellarVoteApp.Core.Services.Contracts;
using StellarVoteApp.Data.Services.Contracts;
using StellarVoteApp.Models.ViewModels;

namespace StellarVoteApp.Controllers
{
    [Authorize]
    public class VoteController : Controller
    {
        private IVoteService voteService;
        private IUserService userService;
        private IUserNationalIDInformationService idService;

        public VoteController(IVoteService voteService, IUserService userService, IUserNationalIDInformationService idService)
        {
            this.voteService = voteService;
            this.userService = userService;
            this.idService = idService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<StellarAccountViewModel> CreateAccount([FromBody] JObject json)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var hasStellarAccount = await this.userService.HasUserVotingAccount(userId);
            if (hasStellarAccount)
            {
                return null;
            }

            var isValidID = this.idService.CheckUserID("", "");

            if (!isValidID)
            {
                // TODO
                throw new Exception();
            }

            StellarAccountViewModel model = new StellarAccountViewModel();
            var stellarAccount = this.voteService.CreateUserAccount();
            await this.userService.SaveAccountDetails(userId, stellarAccount.AccountId, stellarAccount.SecredSeed);
            await this.userService.SetVotingAccountTrue(userId);
            var isActivated = await this.voteService.ActivateUserAccount(stellarAccount.AccountId);
            if (!isActivated)
            {
                // TODO
                throw new Exception();
            }

            // TODO change trust etc

            var balances = await this.voteService.GetBalances(stellarAccount.AccountId);
            model.AccountId = stellarAccount.AccountId;
            model.XLMBalance = balances[0].BalanceString;
            model.VoteTokenBalance = balances.Length > 1 ? balances[1].BalanceString : "0";

            return model;
        }
    }
}
