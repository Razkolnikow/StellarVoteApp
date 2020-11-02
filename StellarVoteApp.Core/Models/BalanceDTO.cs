using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Models
{
    public class BalanceDTO
    {
        public BalanceDTO(string asset, string balanceString)
        {
            this.Asset = asset;
            this.BalanceString = balanceString;
        }

        public string Asset { get; set; }

        public string BalanceString { get; set; }
    }
}
