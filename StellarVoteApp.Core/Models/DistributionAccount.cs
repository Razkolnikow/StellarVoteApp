using System;
using System.Collections.Generic;
using System.Text;

namespace StellarVoteApp.Core.Models
{
    public static class DistributionAccount
    {
        // pub key: GBRK5SAEVIACUW25HABMJ7CBQRAM3YLLJJSSAGS47TYOPUSDEBF2KQYO
        // secret key: SAQ5E5BPVYJR4LEZBAVIWJLZ7QOOICMLTDKGCYSPKV2Z5FKIY5KACMLC

        public static string PublicKey 
        { 
            get 
            {
                return "GBRK5SAEVIACUW25HABMJ7CBQRAM3YLLJJSSAGS47TYOPUSDEBF2KQYO";
            } 
        }

        public static string PrivateKey { get; set; }
    }
}
