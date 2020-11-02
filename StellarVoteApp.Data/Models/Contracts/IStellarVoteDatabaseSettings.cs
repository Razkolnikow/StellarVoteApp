using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Data.Models.Contracts
{
    public interface IStellarVoteDatabaseSettings
    {
        string UsersCollectionName { get; set; }
        string IDCollectionName { get; set; }
        string ConnectionString { get; set; }
        string SimpleConnectionString { get; set; }
        string SimpleCloudConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
