using LoopBot.Models;
using Nethereum.JsonRpc.Client;
using Nethereum.Signer;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Services
{
    public class LoopExchangeWebApiService : IDisposable
    {
        const string _baseUrl = "https://web-api.loopexchange.art";
        readonly RestClient _client;

        public LoopExchangeWebApiService(string token)
        {
            _client = new RestClient(_baseUrl);
            _client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36 Edg/126.0.0.0");
            _client.AddDefaultHeader("authorization", $"Bearer {token}");
        }

        public void UpdateAuthorizationHeader(string newToken)
        {
            var authParameter = _client.DefaultParameters.FirstOrDefault(p => p.Name == "authorization");
            if (authParameter != null)
            {
                _client.DefaultParameters.RemoveParameter(authParameter);
            }
            _client.AddDefaultHeader("authorization", $"Bearer {newToken}");
        }

        public async Task<NftDetails> GetNftDetailsAsync(string nftUrlId)
        {
            var request = new RestRequest($"/nft/{nftUrlId}");
            try
            {
                var response = await _client.GetAsync<NftDetails>(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<NftCollectionInfo> GetCollectionInfo(string url)
        {
            Uri uri = new Uri(url);
            string[] segments = uri.Segments;
            var collection = segments[segments.Length - 1].TrimEnd('/');
            var request = new RestRequest($"/collection/{collection}");
            try
            {
                var response = await _client.GetAsync<NftCollectionInfo>(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<NftCollectionListing> GetCollectionListings(int collectionId)
        {
            var request = new RestRequest($"/collection/{collectionId}/items?limit=20&offset=0&sort=price&sortDescending=false&traits=");
            try
            {
                var response = await _client.GetAsync<NftCollectionListing>(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<ListingDetails> GetNftListingDetailsAsync(string nftUrlId)
        {
            var request = new RestRequest($"/listing/featured-for-nft/{nftUrlId}");
            try
            {
                var response = await _client.GetAsync<ListingDetails>(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<object> SubmitTradeAsync(int accountId, string listingId, NftOrder order, string takerOrderEddsaSignature, string signature)
        {
            var request = new RestRequest($"/taker/take-listing/{listingId}");

            var chainID = 1;
            var exchange = order.exchange;
            var storageId = order.storageId;
            var sellTokenId = 1;
            var sellTokenAmount = order.sellToken.amount;
            var buyTokenId = order.buyToken.tokenId;
            var nftData = order.buyToken.nftData;
            var buyTokenAmount = "1";
            var allOrNone = order.allOrNone;
            var fillAmountBOrS = order.fillAmountBOrS;
            var validUntil = order.validUntil;
            var maxFeeBips = order.maxFeeBips;

            var takerOrder = new
            {
                exchange,
                accountId,
                storageId,
                sellToken = new
                {
                    tokenId = sellTokenId,
                    amount = sellTokenAmount
                },
                buyToken = new
                {
                    tokenId = buyTokenId,
                    nftData,
                    amount = buyTokenAmount
                },
                allOrNone,
                fillAmountBOrS,
                validUntil,
                maxFeeBips
            };

            var body = new
            {
                chainID,
                takerOrder = JsonConvert.SerializeObject(takerOrder),
                takerOrderEddsaSignature,
                signature
            };

            request.AddJsonBody(body);
            try
            {
                var response = await _client.PostAsync<object>(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
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
