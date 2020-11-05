using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Models
{
    public class UserAccountInformation
    {
        public UserAccountInformation(string accountId)
        {
            this.AccountId = accountId;
        }

        public UserAccountInformation(string txHash, string candidateName, string accountId) : this(accountId)
        {
            this.TxHash = txHash;
            this.CandidateName = candidateName;
            this.HasVoted = true;
        }

        public string AccountId { get; set; }

        public string TxHash { get; set; }

        public string CandidateName { get; set; }

        public bool HasVoted { get; set; }
    }
}
