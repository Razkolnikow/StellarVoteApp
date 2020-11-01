using StellarVoteApp.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Services
{
    public class VoteService : IVoteService
    {
        public bool ChangeTrustVoteToken(string pubKey, string secretKey)
        {
            throw new NotImplementedException();
        }

        public bool IssueVoteTokenTo(string destinationAddress)
        {
            throw new NotImplementedException();
        }

        public bool SendVoteToken(string pubKey, string secretKey, string memo)
        {
            throw new NotImplementedException();
        }
    }
}
