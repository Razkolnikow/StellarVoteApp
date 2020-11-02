using StellarVoteApp.Data.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Data.Models
{
    public class StellarVoteDatabaseSettings : IStellarVoteDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string IDCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string SimpleConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string SimpleCloudConnectionString { get; set; }
    }
}
