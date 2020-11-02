using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MongoDB.Driver;
using StellarVoteApp.Core.Models;
using StellarVoteApp.Data.Models;
using StellarVoteApp.Data.Models.Contracts;
using StellarVoteApp.Data.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StellarVoteApp.Data.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<StellarVoteUser> _users;
        private readonly IMongoCollection<IdCredentials> _idCredentials;

        public UserService(IStellarVoteDatabaseSettings settings)
        {
            var client = new MongoClient(settings.SimpleConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<StellarVoteUser>(settings.UsersCollectionName);
            _idCredentials = database.GetCollection<IdCredentials>(settings.IDCollectionName);
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

        public async Task<bool> SaveUserIdCredentials(string nationalIdNumber, string numberOfIdCard)
        {
            // TODO: Hash the values!!!
            var hashedIdNumber = this.HashIdInfo(nationalIdNumber);
            var hashedNumberOfIdCard = this.HashIdInfo(numberOfIdCard);
            await this._idCredentials.InsertOneAsync(new IdCredentials(hashedIdNumber, hashedNumberOfIdCard));

            return true;
        }

        public async Task<bool> CheckIfIdCredentialsExist(string nationalIdNumber, string numberOfIdCard)
        {
            try
            {
                var hashedIdNumber = this.HashIdInfo(nationalIdNumber);
                var hashedNumberOfCard = this.HashIdInfo(numberOfIdCard);
                var idCredentials = await this._idCredentials.FindAsync(i => i.NationalIDNumber == hashedIdNumber);
                var asyncIdCredentials = idCredentials.FirstOrDefault();

                if (asyncIdCredentials == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        private string HashIdInfo(string source)
        {
            using (var md5Hash = MD5.Create())
            {
                var sourceBytes = Encoding.UTF8.GetBytes(source);

                var hashBytes = md5Hash.ComputeHash(sourceBytes);

                var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                return hash;
            }
        }
    }
}
