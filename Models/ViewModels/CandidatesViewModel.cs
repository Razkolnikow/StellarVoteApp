using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StellarVoteApp.Models.ViewModels
{
    public class CandidatesViewModel
    {
        public CandidatesViewModel(List<SelectListItem> candidates)
        {
            this.Candidates = candidates;
        }

        public List<SelectListItem> Candidates { get; set; }

        public string Value { get; set; }
    }
}
