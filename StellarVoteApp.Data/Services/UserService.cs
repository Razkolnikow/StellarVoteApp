using MongoDB.Driver;
using StellarVoteApp.Data.Models;
using StellarVoteApp.Data.Models.Contracts;
using StellarVoteApp.Data.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StellarVoteApp.Data.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<StellarVoteUser> _users;

        public UserService(IStellarVoteDatabaseSettings settings)
        {
            var client = new MongoClient(settings.SimpleConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<StellarVoteUser>(settings.UsersCollectionName);
        }

        public async Task<StellarVoteUser> GetUserByIdAsync(string id)
        {
            return await this._users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<string> GetAccountDetails(string userId)
        {
            throw new NotImplementedException();
        }        

        public async Task<bool> SaveAccountDetails(string userId, string pubKey, string secretKey)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasUserVotingAccount(string id)
        {
            if (id == null)
            {
                return false;
            }

            var userToUse = await this._users.Find(user => user.Id == id).FirstOrDefaultAsync();
            return userToUse.HasStellarVotingAccount;
        }
    }
}
