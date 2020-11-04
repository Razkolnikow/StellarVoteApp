using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using StellarVoteApp.Core.Models;
using StellarVoteApp.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StellarVoteApp.Core.Services
{
    public class VoteService : IVoteService
    {

        // TODO: add validation for id credentials that were already used for creation of stellar account before sending the data to the national DB!!!

        public async Task<bool> ChangeTrustVoteToken(string pubKey, string secretKey)
        {
            //Set network and server
            Network.UseTestNetwork();
            Server server = new Server("https://horizon-testnet.stellar.org");

            var nonNativeAsset = Asset.Create(null, "StellarV", IssueAccount.PublicKey);

            var source = KeyPair.FromSecretSeed(secretKey);

            var limit = "100";

            AccountResponse accountResponse = await server.Accounts.Account(source.AccountId);

            Account sourceAccount = new Account(source.AccountId, accountResponse.SequenceNumber);

            var sourceSponsor = KeyPair.FromSecretSeed(DistributionAccount.SecretSeed);

            var sponsored = KeyPair.FromAccountId(pubKey);

            var operationStartSponsor = new BeginSponsoringFutureReservesOperation.Builder(sponsored)
                .SetSourceAccount(sourceSponsor)
                .Build();

            var operation = new ChangeTrustOperation.Builder(nonNativeAsset, limit)
                .SetSourceAccount(source)
                .Build();

            var operationEndSponsoring = new EndSponsoringFutureReservesOperation.Builder()
                .SetSourceAccount(source)
                .Build();

            Transaction innerTransaction = new TransactionBuilder(sourceAccount)
                .SetFee(100)
                .AddOperation(operationStartSponsor)
                .AddOperation(operation)
                .AddOperation(operationEndSponsoring)
                .Build();

            //Sign Transaction            
            innerTransaction.Sign(sourceSponsor);
            innerTransaction.Sign(source);

            var feeSource = sourceSponsor;
            var finalTx = TransactionBuilder.BuildFeeBumpTransaction(feeSource, innerTransaction, 100);
            finalTx.Sign(feeSource);
            //Try to send the transaction
            try
            {
                var response = await this.SubmitTransaction(finalTx.ToEnvelopeXdrBase64());
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public async Task<bool> SendVoteTokenToUser(string destinationAddress)
        {
            //Set network and server
            Network.UseTestNetwork();
            Server server = new Server("https://horizon-testnet.stellar.org");

            //Sender
            var senderSource = KeyPair.FromSecretSeed(DistributionAccount.SecretSeed);

            //Destination keypair from the account id
            KeyPair destinationKeyPair = KeyPair.FromAccountId(destinationAddress);

            AccountResponse sourceAccountResponse = await server.Accounts.Account(senderSource.AccountId);

            //Create source account object
            Account sourceAccount = new Account(senderSource.AccountId, sourceAccountResponse.SequenceNumber);

            Asset asset = new AssetTypeCreditAlphaNum12("StellarV", IssueAccount.PublicKey);

            //Create payment operation
            PaymentOperation operation = new PaymentOperation.Builder(destinationKeyPair, asset, "1").SetSourceAccount(sourceAccount.KeyPair).Build();

            //Create transaction and add the payment operation we created
            Transaction transaction = new TransactionBuilder(sourceAccount).AddOperation(operation).Build();

            //Sign Transaction
            transaction.Sign(senderSource);

            //Try to send the transaction
            try
            {
                var response = await server.SubmitTransaction(transaction);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public async Task<bool> SendVoteToken(string pubKey, string secretKey, string memo)
        {
            //Set network and server
            Network.UseTestNetwork();
            Server server = new Server("https://horizon-testnet.stellar.org");

            var source = KeyPair.FromSecretSeed(secretKey);

            AccountResponse sourceAccountResponse = await server.Accounts.Account(source.AccountId);

            //Create source account object
            Account sourceAccount = new Account(source.AccountId, sourceAccountResponse.SequenceNumber);

            Asset asset = new AssetTypeCreditAlphaNum12("StellarV", IssueAccount.PublicKey);

            //Create payment operation
            PaymentOperation operation = new PaymentOperation.Builder(KeyPair.FromAccountId(DistributionAccount.PublicKey), asset, "1").SetSourceAccount(sourceAccount.KeyPair).Build();

            Transaction innerTransaction = new TransactionBuilder(sourceAccount)
                .SetFee(100)
                .AddOperation(operation)
                .AddMemo(new MemoText(memo))
                .Build();

            innerTransaction.Sign(source);

            var feeSource = KeyPair.FromSecretSeed(DistributionAccount.SecretSeed);
            var finalTx = TransactionBuilder.BuildFeeBumpTransaction(feeSource, innerTransaction, 100);
            finalTx.Sign(feeSource);
            //Try to send the transaction
            try
            {
                var response = await this.SubmitTransaction(finalTx.ToEnvelopeXdrBase64());
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public StellarAccount CreateUserAccount()
        {
            Network.UseTestNetwork();
            KeyPair keypair = KeyPair.Random();
            var stellarAccount = new StellarAccount(keypair.AccountId, keypair.SecretSeed);
            return stellarAccount;
        }

        public async Task<bool> ActivateUserAccount(string accountId)
        {
            //Set network and server
            Network.UseTestNetwork();
            Server server = new Server("https://horizon-testnet.stellar.org");

            var sourceKeyPair = KeyPair.FromSecretSeed(DistributionAccount.SecretSeed);

            //Destination keypair from the account id
            KeyPair destinationKeyPair = KeyPair.FromAccountId(accountId);

            AccountResponse sourceAccountResponse = await server.Accounts.Account(DistributionAccount.PublicKey);

            //Create source account object
            Account sourceAccount = new Account(DistributionAccount.PublicKey, sourceAccountResponse.SequenceNumber);

            Asset asset = new AssetTypeNative();

            //Create payment operation
            CreateAccountOperation operation = new CreateAccountOperation.Builder(destinationKeyPair, "1").SetSourceAccount(sourceAccount.KeyPair).Build();

            Transaction transaction = new TransactionBuilder(sourceAccount)
                .AddOperation(operation)
                .Build();

            transaction.Sign(sourceKeyPair);

            try
            {
                var response = await server.SubmitTransaction(transaction);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        private async Task<SubmitTransactionResponse> SubmitTransaction(string transactionEnvelopeBase64)
        {
            var transactionUri = new UriBuilder("https://horizon-testnet.stellar.org").SetPath("/transactions").Uri;

            var paramsPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("tx", transactionEnvelopeBase64)
            };
            var _httpClient = new HttpClient();
            var response = await _httpClient.PostAsync(transactionUri, new FormUrlEncodedContent(paramsPairs.ToArray()));
            if (response.Content != null)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var submitTransactionResponse = JsonSingleton.GetInstance<SubmitTransactionResponse>(responseString);
                return submitTransactionResponse;
            }

            return null;
        }

        public async Task<BalanceDTO[]> GetBalances(string accountId)
        {
            Network.UseTestNetwork();
            Server server = new Server("https://horizon-testnet.stellar.org");

            AccountResponse sourceAccountResponse = await server.Accounts.Account(accountId);

            var balances = sourceAccountResponse.Balances;

            var balancesList = new List<BalanceDTO>();
            foreach (var balance in balances)
            {
                balancesList.Add(new BalanceDTO(balance.AssetType, balance.BalanceString));
            }

            return balancesList.ToArray();
        }
    }
}
