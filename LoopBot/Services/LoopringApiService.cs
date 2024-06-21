using JsonFlatten;
using LoopBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Services
{
    public class LoopringApiService : IDisposable
    {
        const string _baseUrl = "https://api3.loopring.io";

        readonly RestClient _client;

        public LoopringApiService(string apiKey)
        {
            _client = new RestClient(_baseUrl);
            _client.AddDefaultHeader("x-api-key", apiKey);
        }

        public async Task<StorageId?> GetNextStorageId(int accountId, int sellTokenId)
        {
            var request = new RestRequest("api/v3/storageId");
            request.AddParameter("accountId", accountId);
            request.AddParameter("sellTokenId", sellTokenId);

            var response = await _client.ExecuteGetAsync<StorageId>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting storage id from Loopring, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<NftBalance?> GetNftBalancePage(int accountId, int offset)
        {
            var request = new RestRequest("api/v3/user/nft/balances");
            request.AddParameter("accountId", accountId);
            request.AddParameter("limit", 25);
            request.AddParameter("offset", offset);
            request.AddParameter("metadata", "true");
            var response = await _client.ExecuteGetAsync<NftBalance>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting nft balance from Loopring, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<NftOrderFee?> GetOrderFee(int accountId, string tokenAddress, string quoteAmount)
        {
            var request = new RestRequest($"/api/v3/user/nft/orderFee?accountId={accountId}&nftTokenAddress={tokenAddress}&quoteToken=1&quoteAmount={quoteAmount}");

            var response = await _client.ExecuteGetAsync<NftOrderFee>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting order fee from Loopring, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<string?> SubmitNftTradeValidateOrder(NftTakerOrder nftOrder, string eddsaSignature)
        {
            var request = new RestRequest("api/v3/nft/validateOrder");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("exchange", nftOrder.exchange);
            request.AddParameter("accountId", nftOrder.accountId);
            request.AddParameter("storageId", nftOrder.storageId);
            request.AddParameter("sellToken.tokenId", nftOrder.sellToken.tokenId);
            request.AddParameter("sellToken.amount", nftOrder.sellToken.amount);
            request.AddParameter("buyToken.tokenId", nftOrder.buyToken.tokenId);
            request.AddParameter("buyToken.amount", nftOrder.buyToken.amount);
            request.AddParameter("buyToken.nftData", nftOrder.buyToken.nftData);
            request.AddParameter("allOrNone", "false");
            request.AddParameter("fillAmountBOrS", "true");
            request.AddParameter("validUntil", nftOrder.validUntil);
            request.AddParameter("maxFeeBips", nftOrder.maxFeeBips);
            request.AddParameter("eddsaSignature", eddsaSignature);
            var response = await _client.ExecutePostAsync<string>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error submitting NFT trade validation to Loopring, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<string?> SubmitNftTrade(NftTrade nftTrade, string apiSig)
        {
            var request = new RestRequest("/api/v3/nft/trade", Method.Post);
            request.AddHeader("x-api-sig", apiSig);
            request.AddHeader("Accept", "application/json");
            var jObject = JObject.Parse(JsonConvert.SerializeObject(nftTrade));
            var jObjectFlattened = jObject.Flatten();
            var jObjectFlattenedString = JsonConvert.SerializeObject(jObjectFlattened);
            request.AddParameter("application/json", jObjectFlattenedString, ParameterType.RequestBody);


            var response = await _client.ExecuteGetAsync<string>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error submitting NFT trade to Loopring, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }
        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
