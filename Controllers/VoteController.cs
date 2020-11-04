using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using StellarVoteApp.Core.Services.Contracts;
using StellarVoteApp.Data.Services.Contracts;
using StellarVoteApp.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StellarVoteApp.Controllers
{
    [Authorize]
    public class VoteController : Controller
    {
        private IVoteService voteService;
        private IUserService userService;
        private IUserNationalIDInformationService idService;
        private IJsonConverter jsonConverter;
        private IConfiguration config;

        public VoteController(
            IVoteService voteService, 
            IUserService userService, 
            IUserNationalIDInformationService idService, 
            IJsonConverter jsonConverter,
            IConfiguration config)
        {
            this.voteService = voteService;
            this.userService = userService;
            this.idService = idService;
            this.jsonConverter = jsonConverter;
            this.config = config;
        }

        public IActionResult Index()
        {
            var candidates = this.config.GetSection("Candidates").Get<List<SelectListItem>>();
            return View(new CandidatesViewModel(candidates));
        }

        [HttpPost]
        public async Task<IActionResult> SendVote(string value)
        {
            // TODO: Send to vote to the stellar network
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var stellarAccount = await this.userService.GetAccountDetails(userId);
            // TODO get the transaction hash!!!
            var isVoteSent = await this.voteService.SendVoteToken(stellarAccount.AccountId, stellarAccount.SecredSeed, value);

            return View("SuccessfulVote", value);
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<StellarAccountViewModel> CreateAccount([FromBody] JObject json)
        {
            var idCredentials = this.jsonConverter.DeserializeJson<IdCredentialsViewModel>(json.ToString());
            var idCredentialsExist = await this.userService.CheckIfIdCredentialsExist(idCredentials.IdNumber, idCredentials.CardNumber);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var hasStellarAccount = await this.userService.HasUserVotingAccount(userId);
            if (hasStellarAccount)
            {
                return null;
            }

            var isValidID = this.idService.CheckUserID(idCredentials.IdNumber, idCredentials.CardNumber);

            if (!isValidID)
            {
                throw new Exception("Not valid ID Credentials!");
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
            var isTrustCreated = await this.voteService.ChangeTrustVoteToken(stellarAccount.AccountId, stellarAccount.SecredSeed);

            if (isTrustCreated)
            {
                var receivedVoteToken = await this.voteService.SendVoteTokenToUser(stellarAccount.AccountId);
            }

            await this.userService.SaveUserIdCredentials(idCredentials.IdNumber, idCredentials.CardNumber);

            var balances = await this.voteService.GetBalances(stellarAccount.AccountId);
            model.AccountId = stellarAccount.AccountId;
            foreach (var b in balances)
            {
                if (b.Asset.ToLower().Contains("native"))
                {
                    model.XLMBalance = b.BalanceString;
                }
                else
                {
                    model.VoteTokenBalance = b.BalanceString;
                }
            }

            return model;
        }
    }
}
