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

        Task<string> GetAccountDetails(string userId);

        Task<bool> HasUserVotingAccount(string userId);
    }
}
