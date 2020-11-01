using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Services.Contracts
{
    public interface IVoteService
    {
        bool IssueVoteTokenTo(string destinationAddress);

        bool ChangeTrustVoteToken(string pubKey, string secretKey);

        bool SendVoteToken(string pubKey, string secretKey, string memo);
    }
}
