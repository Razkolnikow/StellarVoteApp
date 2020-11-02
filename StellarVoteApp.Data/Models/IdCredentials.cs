using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Data.Models
{
    [BsonIgnoreExtraElements]
    public class IdCredentials
    {
        public IdCredentials(string nationalIdNumber, string numberOfIdCard)
        {
            this.NationalIDNumber = nationalIdNumber;
            this.NumberOfIDCard = numberOfIdCard;
        }

        public string NationalIDNumber { get; set; }

        public string NumberOfIDCard { get; set; }
    }
}
