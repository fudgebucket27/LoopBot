using LoopBot.Models;
using Nethereum.JsonRpc.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Services
{
    public class LoopExchangeApiService : IDisposable
    {
        const string _baseUrl = "https://api.loopexchange.art";
        readonly RestClient _client;

        public LoopExchangeApiService()
        {
            _client = new RestClient(_baseUrl);
            _client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36 Edg/126.0.0.0");
        }

        public async Task<BearerToken> LoginAsync(int accountId, string address, string signature)
        {
            var request = new RestRequest("/web-v1/account/login");
            request.AddJsonBody(new
            {
                address,
                signature,
                accountId,
                isCounterfactual = false,
                chainID = 1
            });
            try
            {
                var response = await _client.PostAsync<BearerToken>(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<TakerListingDetails> GetTakerListingDetailsAsync(string id)
        {
            var request = new RestRequest($"/web-v1/taker/{id}");

            try
            {
                var response = await _client.GetAsync<TakerListingDetails>(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<TakerFee> GetTakerFeesAsync(string id, int accountId, string nftTokenAddress, int maxFeeBips, string sellTokenAmount)
        {
            var request = new RestRequest($"/web-v1/taker/{id}/calculatefees?chainId=1&accountId={accountId}&nftTokenAddress={nftTokenAddress}&feeTokenId=1&maxFeeBips={maxFeeBips}&sellTokenAmount={sellTokenAmount}");

            try
            {
                var response = await _client.GetAsync<TakerFee>(request);
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
