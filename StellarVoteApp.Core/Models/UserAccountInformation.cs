using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Models
{
    public class UserAccountInformation
    {
        public UserAccountInformation()
        {

        }

        public UserAccountInformation(string txHash, string candidateName)
        {
            this.TxHash = txHash;
            this.CandidateName = candidateName;
        }

        public string TxHash { get; set; }

        public string CandidateName { get; set; }
    }
}
