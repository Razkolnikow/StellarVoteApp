using AspNetCore.Identity.Mongo.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Data.Models
{
    public class StellarVoteUser : MongoUser
    {
        /// <summary>
        /// When the user enters his identity information the form should be sent to the central database and check
        /// if the information there exists. If that is the case a Stellar account should be generated and this property will be set to "true". 
        /// No connection between the user in this application and the identity information in the central database will stay in existence.
        /// </summary>
        public bool HasStellarVotingAccount { get; set; }

        public string AccountId { get; set; }

        public string SecretSeed { get; set; }
    }
}
