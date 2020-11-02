using StellarVoteApp.Core.Models;
using StellarVoteApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StellarVoteApp.Data.Services.Contracts
{
    public interface IUserService
    {
        Task<StellarVoteUser> GetUserByIdAsync(string id);

        Task<bool> SaveAccountDetails(string userId, string pubKey, string secretKey);

        Task<StellarAccount> GetAccountDetails(string userId);

        Task<bool> HasUserVotingAccount(string userId);

        Task<bool> SetVotingAccountTrue(string userId);

        /// <summary>
        /// Saving the hash of the Id Credentials. No readable private data will be saved.
        /// </summary>
        /// <param name="nationalIdNumber"></param>
        /// <param name="numberOfIdCard"></param>
        /// <returns></returns>
        Task<bool> SaveUserIdCredentials(string nationalIdNumber, string numberOfIdCard);

        /// <summary>
        /// Checking the hashed id credentials.
        /// </summary>
        /// <param name="nationalIdNumber"></param>
        /// <param name="numberOfIdCard"></param>
        /// <returns></returns>
        Task<bool> CheckIfIdCredentialsExist(string nationalIdNumber, string numberOfIdCard);
    }
}
