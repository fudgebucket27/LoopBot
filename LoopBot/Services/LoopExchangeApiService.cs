using LoopBot.Models;
using Nethereum.JsonRpc.Client;
using Nethereum.Signer;
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

        public async Task<BearerToken?> LoginAsync(int accountId, string address, string l1PrivateKey)
        {
            var loopexchangeLoginMessage = "Welcome to LoopExchange!\n\nClick to sign in and agree to LoopExchange Terms of Service and Privacy policy.\n\nThis request will not trigger a blockchain transaction or cost any gas fees.";
            var l1Key = new EthECKey(l1PrivateKey);
            var signer = new EthereumMessageSigner();
            var signature = signer.EncodeUTF8AndSign(loopexchangeLoginMessage, l1Key);
            var request = new RestRequest("/web-v1/account/login");
            request.AddJsonBody(new
            {
                address,
                signature,
                accountId,
                isCounterfactual = false,
                chainID = 1
            });

            var response = await _client.ExecutePostAsync<BearerToken>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error logging into LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<TakerListingDetails?> GetTakerListingDetailsAsync(string id)
        {
            var request = new RestRequest($"/web-v1/taker/{id}");
            var response = await _client.ExecuteGetAsync<TakerListingDetails>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting Taker Listing Details from LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public async Task<TakerFee?> GetTakerFeesAsync(string id, int accountId, string nftTokenAddress, int maxFeeBips, string sellTokenAmount)
        {
            var request = new RestRequest($"/web-v1/taker/{id}/calculatefees?chainId=1&accountId={accountId}&nftTokenAddress={nftTokenAddress}&feeTokenId=1&maxFeeBips={maxFeeBips}&sellTokenAmount={sellTokenAmount}");
            var response = await _client.ExecuteGetAsync<TakerFee>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }
            else
            {
                throw new Exception($"Error getting Taker Fees from LoopExchange, HTTP Status Code:{response.StatusCode}, Content:{response.Content}");
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
