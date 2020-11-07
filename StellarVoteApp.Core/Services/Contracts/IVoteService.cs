using StellarVoteApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StellarVoteApp.Core.Services.Contracts
{
    public interface IVoteService
    {
        /// <summary>
        /// Distribution account sends VoteToken to current user
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <returns></returns>
        Task<bool> SendVoteTokenToUser(string destinationAddress);

        /// <summary>
        /// User account creates trustline for VoteToken
        /// </summary>
        /// <param name="pubKey"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        Task<bool> ChangeTrustVoteToken(string pubKey, string secretKey);

        /// <summary>
        /// User account votes by sending the VoteToken with the memo field holding the voting choice of the user to the distribution account
        /// </summary>
        /// <param name="pubKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        Task<string> SendVoteToken(string pubKey, string secretKey, string memo);

        /// <summary>
        /// Activates user account with Create Account Operation 
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<bool> ActivateUserAccount(string accountId, string secretSeed);

        /// <summary>
        /// Generates random KeyPair
        /// </summary>
        /// <returns></returns>
        StellarAccount CreateUserAccount();
        /// <summary>
        /// Get Balances of an account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<BalanceDTO[]> GetBalances(string accountId);

        /// <summary>
        /// Gets the results from the election
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, int>> GetElectionResults();

        Task<UserAccountInformation> GetUserAccountInformation(string userAccountId);
    }
}
