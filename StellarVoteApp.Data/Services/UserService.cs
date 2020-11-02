using MongoDB.Driver;
using StellarVoteApp.Core.Models;
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

        public async Task<StellarAccount> GetAccountDetails(string userId)
        {
            var asyncUser = await this._users.Find(user => user.Id == userId).FirstOrDefaultAsync();

            if (asyncUser == null)
            {
                return new StellarAccount(string.Empty, string.Empty);
            }

            return new StellarAccount(asyncUser.AccountId, asyncUser.SecretSeed);
        }        

        public async Task<bool> SaveAccountDetails(string userId, string pubKey, string secretKey)
        {
            var asyncUser = await this._users.FindAsync(u => u.Id == userId);
            var user = asyncUser.FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            var filter1 = Builders<StellarVoteUser>.Filter.Eq(s => s.Id, userId);
            var update1 = new UpdateDefinitionBuilder<StellarVoteUser>()
                .Set<string>(u => u.AccountId, pubKey);
            this._users.UpdateOne(filter1, update1);

            var filter2 = Builders<StellarVoteUser>.Filter.Eq(s => s.Id, userId);
            var update2 = new UpdateDefinitionBuilder<StellarVoteUser>()
                .Set<string>(u => u.SecretSeed, secretKey);
            this._users.UpdateOne(filter2, update2);

            return true;
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

        public async Task<bool> SetVotingAccountTrue(string userId)
        {
            var asyncUser = await this._users.FindAsync(u => u.Id == userId);
            var user = asyncUser.FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            var filter1 = Builders<StellarVoteUser>.Filter.Eq(s => s.Id, userId);
            var update1 = new UpdateDefinitionBuilder<StellarVoteUser>()
                .Set<bool>(u => u.HasStellarVotingAccount, true);
            this._users.UpdateOne(filter1, update1);

            return true;
        }
    }
}
