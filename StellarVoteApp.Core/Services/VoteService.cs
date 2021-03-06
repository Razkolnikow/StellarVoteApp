﻿using stellar_dotnet_sdk;
using stellar_dotnet_sdk.requests;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.page;
using StellarVoteApp.Core.Models;
using StellarVoteApp.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace StellarVoteApp.Core.Services
{
    public class VoteService : IVoteService
    {
        private HttpClient http;
        private IJsonConverter jsonConverter;
        private IConfiguration config;
        private readonly string VoteToken;

        public VoteService(HttpClient http, IJsonConverter jsonConverter, IConfiguration config)
        {
            this.http = http;
            this.jsonConverter = jsonConverter;
            this.config = config;
            this.VoteToken = this.config.GetSection("VoteToken").Value;
        }

        public async Task<bool> ChangeTrustVoteToken(string pubKey, string secretKey)
        {
            //Set network and server
            Network.UseTestNetwork();
            Server server = new Server("https://horizon-testnet.stellar.org");

            var nonNativeAsset = Asset.Create(null, this.VoteToken, IssueAccount.PublicKey);

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
                return response.IsSuccess();
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

            Asset asset = new AssetTypeCreditAlphaNum12(this.VoteToken, IssueAccount.PublicKey);

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
                return response.IsSuccess();
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public async Task<string> SendVoteToken(string pubKey, string secretKey, string memo)
        {
            //Set network and server
            Network.UseTestNetwork();
            Server server = new Server("https://horizon-testnet.stellar.org");

            var source = KeyPair.FromSecretSeed(secretKey);

            AccountResponse sourceAccountResponse = await server.Accounts.Account(source.AccountId);

            //Create source account object
            Account sourceAccount = new Account(source.AccountId, sourceAccountResponse.SequenceNumber);

            Asset asset = new AssetTypeCreditAlphaNum12(this.VoteToken, IssueAccount.PublicKey);

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
                var hash = response.Hash;
                return hash;
            }
            catch (Exception exception)
            {
                return string.Empty;
            }
        }

        public StellarAccount CreateUserAccount()
        {
            Network.UseTestNetwork();
            KeyPair keypair = KeyPair.Random();
            var stellarAccount = new StellarAccount(keypair.AccountId, keypair.SecretSeed);
            return stellarAccount;
        }

        public async Task<bool> ActivateUserAccount(string accountId, string secretSeed)
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

            var operationStartSponsor = new BeginSponsoringFutureReservesOperation.Builder(destinationKeyPair)
                .SetSourceAccount(sourceKeyPair)
                .Build();            

            //Create payment operation
            CreateAccountOperation operation = new CreateAccountOperation.Builder(destinationKeyPair, "0").SetSourceAccount(sourceAccount.KeyPair).Build();

            var sponsoredSource = KeyPair.FromSecretSeed(secretSeed);
            var operationEndSponsoring = new EndSponsoringFutureReservesOperation.Builder()
                .SetSourceAccount(sponsoredSource)
                .Build();

            Transaction transaction = new TransactionBuilder(sourceAccount)
                .SetFee(100)
                .AddOperation(operationStartSponsor)
                .AddOperation(operation)
                .AddOperation(operationEndSponsoring)
                .Build();

            //Sign Transaction            
            transaction.Sign(sourceKeyPair);
            transaction.Sign(sponsoredSource);

            try
            {
                var response = await server.SubmitTransaction(transaction);
                return response.IsSuccess();
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

        public async Task<Dictionary<string, int>> GetElectionResults()
        {
            Network.UseTestNetwork();
            Server server = new Server("https://horizon-testnet.stellar.org");
            Dictionary<string, int> candidateVotes = new Dictionary<string, int>();

            var transactionsRequestBuilder = new TransactionsRequestBuilder(new Uri("https://horizon-testnet.stellar.org"), this.http);
            var res = await transactionsRequestBuilder.ForAccount(DistributionAccount.PublicKey).Execute();
            var records = res.Records;

            while (records.Count > 0)
            {
                var result = await this.http.GetAsync(res.Links.Next.Uri);
                var json = await result.Content.ReadAsStringAsync();
                res = this.jsonConverter.DeserializeJson<Page<TransactionResponse>>(json);
                records = res.Records;

                foreach (var rec in records)
                {
                    var memo = rec.MemoValue;
                    if (!string.IsNullOrWhiteSpace(memo))
                    {
                        if (this.CheckStellarVIsValidVote(rec))
                        {
                            if (!candidateVotes.ContainsKey(memo))
                            {
                                candidateVotes.Add(memo, 1);
                            }
                            else
                            {
                                candidateVotes[memo]++;
                            }
                        }
                    }
                }
            }

            return candidateVotes;
        }

        public async Task<UserAccountInformation> GetUserAccountInformation(string userAccountId)
        {
            Network.UseTestNetwork();

            var transactionsRequestBuilder = new TransactionsRequestBuilder(new Uri("https://horizon-testnet.stellar.org"), this.http);
            var res = await transactionsRequestBuilder.ForAccount(userAccountId).Execute();
            var record = res.Records.FirstOrDefault(x => !string.IsNullOrEmpty(x.MemoValue));
            if (record != null)
            {
                return new UserAccountInformation(record.Hash, record.MemoValue, userAccountId);
            }

            return new UserAccountInformation(userAccountId);
        }

        private bool CheckStellarVIsValidVote(TransactionResponse rec)
        {
            try
            {
                var bytes = rec.EnvelopeXdr.ToCharArray();
                var txEnvelope = stellar_dotnet_sdk.xdr.TransactionEnvelope
                    .Decode(new stellar_dotnet_sdk.xdr.XdrDataInputStream(Convert.FromBase64CharArray(bytes, 0, bytes.Length)));
                var feeBumpTx = FeeBumpTransaction.FromEnvelopeXdr(txEnvelope);
                var ops = feeBumpTx.InnerTransaction.Operations;
                foreach (PaymentOperation op in ops)
                {
                    var asset = op.Asset as AssetTypeCreditAlphaNum12;
                    var code = asset?.Code;
                    if (code == this.VoteToken)
                        return true;
                }
            }
            catch (Exception ex)
            {
                return false;               
            }            

            return false;
        }
    }
}
