using JsonFlatten;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class LoopringApiService : IDisposable
    {
        const string _baseUrl = "https://api3.loopring.io";

        readonly RestClient _client;

        public LoopringApiService()
        {
            _client = new RestClient(_baseUrl);
        }

        public async Task<StorageId> GetNextStorageId(string apiKey, int accountId, int sellTokenId)
        {
            var request = new RestRequest("api/v3/storageId");
            request.AddHeader("x-api-key", apiKey);
            request.AddParameter("accountId", accountId);
            request.AddParameter("sellTokenId", sellTokenId);
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<StorageId>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Console.WriteLine($"Error getting storage id: {httpException.Message}");
                return null;
            }
        }

        public async Task<NftOrderFee> GetOrderFee(string apiKey, int accountId, string tokenAddress, string quoteAmount)
        {
            var request = new RestRequest($"/api/v3/user/nft/orderFee?accountId={accountId}&nftTokenAddress={tokenAddress}&quoteToken=1&quoteAmount={quoteAmount}");
            request.AddHeader("x-api-key", apiKey);
            var response = await _client.GetAsync<NftOrderFee>(request);
            return response;
        }

        public async Task<string> SubmitNftTradeValidateOrder(string apiKey,NftOrder nftOrder,string eddsaSignature)
        {
            var request = new RestRequest("api/v3/nft/validateOrder");
            request.AddHeader("x-api-key", apiKey);
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

            try
            {
                var response = await _client.ExecutePostAsync(request);
                var data = response.Content;
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Console.WriteLine($"Error validating nft order!: {httpException.Message}");
                return null;
            }
        }

        public async Task<string> SubmitNftTrade(string apiKey, NftTrade nftTrade, string apiSig)
        {
            var request = new RestRequest("/api/v3/nft/trade", Method.Post);
            request.AddHeader("x-api-key", apiKey);
            request.AddHeader("x-api-sig", apiSig);
            request.AddHeader("Accept", "application/json");
            var jObject = JObject.Parse(JsonConvert.SerializeObject(nftTrade));
            var jObjectFlattened = jObject.Flatten();
            var jObjectFlattenedString = JsonConvert.SerializeObject(jObjectFlattened);
            request.AddParameter("application/json", jObjectFlattenedString, ParameterType.RequestBody);

            try
            {
                var response = await _client.ExecuteAsync(request);
                var data = response.Content;
                Console.WriteLine($"NFT Trade Response: {response.Content}");
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Console.WriteLine($"Error with nft trade!: {httpException.Message}");
                return null;
            }
        }
        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
