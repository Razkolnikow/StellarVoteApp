using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Models
{
    public class StellarAccount
    {
        public StellarAccount(string accountId, string secretSeed)
        {
            this.AccountId = accountId;
            this.SecredSeed = secretSeed;
        }

        public string AccountId { get; set; }

        public string SecredSeed { get; set; }
    }
}
