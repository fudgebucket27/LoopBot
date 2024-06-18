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

        public async Task<object> SubmitTradeAsync(string listingId, NftOrder order, string takerOrderEddsaSignature, string signature)
        {
            var request = new RestRequest($"/taker/take-listing/{listingId}");

            var chainID = 1;
            var exchange = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4";
            var accountId = 40940;
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
