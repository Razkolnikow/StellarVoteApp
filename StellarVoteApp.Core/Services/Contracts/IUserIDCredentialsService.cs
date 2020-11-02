using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Services.Contracts
{
    /// <summary>
    /// This service should validate the user's ID Info and check in the central DB that the user has used or not the right to create stellar account and tokenize the right to vote.
    /// Every user can have only one stellar account. The stellar account will not be linked to the user's ID in the central DB. It will only be noted with "True" that user
    /// with this ID has tokenized the right to vote.
    /// </summary>
    public interface IUserNationalIDInformationService
    {
        /// <summary>
        /// This method should validate the user's ID Info and check in the central DB that the user has used or not the right to create stellar account and tokenize the right to vote.
        /// Every user can have only one stellar account. The stellar account will not be linked to the user's ID in the central DB. It will only be noted with "True" that user
        /// with this ID has tokenized the right to vote.
        /// </summary>
        /// <param name="nationalIDNumber"></param>
        /// <param name="numberOfIDCard"></param>
        /// <returns></returns>
        bool CheckUserID(string nationalIDNumber, string numberOfIDCard);
    }
}
