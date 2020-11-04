using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StellarVoteApp.Models.ViewModels
{
    public class CandidateResultViewModel
    {
        public CandidateResultViewModel(string name, int votes)
        {
            this.Name = name;
            this.Votes = votes;
        }

        public string Name { get; set; }

        public int Votes { get; set; }
    }
}
