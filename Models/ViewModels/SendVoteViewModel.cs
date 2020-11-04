using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StellarVoteApp.Models.ViewModels
{
    public class SendVoteViewModel
    {
        public SendVoteViewModel(string txHash, string chosenCandidateName)
        {
            this.TxHash = txHash;
            this.ChosenCandidateName = chosenCandidateName;
        }

        public string TxHash { get; set; }

        public string ChosenCandidateName { get; set; }
    }
}
