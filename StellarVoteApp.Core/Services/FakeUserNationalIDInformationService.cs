using StellarVoteApp.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Services
{
    public class FakeUserNationalIDInformationService : IUserNationalIDInformationService
    {
        public bool CheckUserID(string nationalIDNumber, string numberOfIDCard)
        {
            return true;
        }
    }
}
