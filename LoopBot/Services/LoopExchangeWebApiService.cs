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

        public async Task<NftDetails?> GetNftDetailsAsync(string nftUrlId)
        {
            var request = new RestRequest($"/nft/{nftUrlId}");
            var response = await _client.ExecuteGetAsync<NftDetails>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting NFT details from LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }

        }

        public async Task<NftCollectionInfo?> GetCollectionInfo(string url)
        {
            Uri uri = new Uri(url);
            string[] segments = uri.Segments;
            var collection = segments[segments.Length - 1].TrimEnd('/');
            var request = new RestRequest($"/collection/{collection}");
            var response = await _client.ExecuteGetAsync<NftCollectionInfo>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting NFT Collection Info from LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<NftCollectionListing?> GetCollectionListings(int collectionId)
        {
            var request = new RestRequest($"/collection/{collectionId}/items?limit=20&offset=0&sort=price&sortDescending=false&traits=");

            var response = await _client.ExecuteGetAsync<NftCollectionListing>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting NFT Collection Listings from LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<ListingDetails?> GetNftListingDetailsAsync(string nftUrlId)
        {
            var request = new RestRequest($"/listing/featured-for-nft/{nftUrlId}");

            var response = await _client.ExecuteGetAsync<ListingDetails>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting NFT Listing Details from LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<ListingItems?> GetNftListingsAsync(string nftUrlId)
        {
            var request = new RestRequest($"/listing/search-for-nft/{nftUrlId}");

            var response = await _client.ExecuteGetAsync<ListingItems>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting NFT Listings from LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }


        public async Task<object?> SubmitTakerTradeAsync(int accountId, string listingId, NftTakerOrder order, string takerOrderEddsaSignature, string signature)
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

            var response = await _client.ExecutePostAsync<object>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error submitting NFT taker trade to LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<NftListingResponse?> SubmitMakerTradeAsync(List<(NftMakerOrder makerOrder, string makerOrderEddsaSignature)> makerOrders, long validUntil)
        {
            var request = new RestRequest($"/my-listing");

            // Create a new structure to hold the correctly formatted makerOrders
            var makerOrdersList = new List<Dictionary<string, string>>();
            foreach (var order in makerOrders)
            {
                var dict = new Dictionary<string, string>
            {
                { "makerOrder", JsonConvert.SerializeObject(order.makerOrder) },
                { "makerOrderEddsaSignature", order.makerOrderEddsaSignature }
            };
                makerOrdersList.Add(dict);
            }

            var body = new
            {
                validUntil,
                makerOrders = makerOrdersList
            };

            request.AddJsonBody(body);

            var response = await _client.ExecutePostAsync<NftListingResponse>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error submitting NFT maker trade to LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
