using LoopBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Helpers
{
    public static class MonitorListingModeHelper
    {
        public static async Task<bool> Run(ServiceManager serviceManager, Settings settings, string nftFullId, decimal priceToBuyDecimal)
        {
            //Setup local vars from service class - tiny bit faster
            var loopExchangeApiService = serviceManager.LoopExchangeApiService;
            var loopExchangeWebApiService = serviceManager.LoopExchangeWebApiService;
            var loopringApiService = serviceManager.LoopringApiService;

            try
            {
                //Get the NFT details
                var nftDetails = await loopExchangeWebApiService.GetNftDetailsAsync(nftFullId);
                var nftListingDetails = await loopExchangeWebApiService.GetNftListingDetailsAsync(nftFullId);
                var nftTakerListingDetails = await loopExchangeApiService.GetTakerListingDetailsAsync(nftListingDetails.Id);
                var listingPriceDecimal = Utils.ConvertStringToDecimal(nftTakerListingDetails.Erc20TokenAmount);

                if (listingPriceDecimal <= priceToBuyDecimal) //NFT has to be under or equal to the price limit
                {
                    Console.WriteLine($"Valid listing found for NFT: '{nftDetails.Name}',at price: {listingPriceDecimal} LRC...Attempting to buy...");
                    //Get  fees
                    var orderFee = await loopringApiService.GetOrderFee(settings.LoopringAccountId, nftDetails.TokenAddress, nftTakerListingDetails.Erc20TokenAmount);
                    var takerOrderFee = await loopExchangeApiService.GetTakerFeesAsync(nftListingDetails.Id, settings.LoopringAccountId, nftDetails.TokenAddress, orderFee.FeeRate.Rate, nftTakerListingDetails.Erc20TokenAmount);
                    var storageId = await loopringApiService.GetNextStorageId(settings.LoopringAccountId, 1);

                    //Sign the order
                    (NftTakerOrder nftTakerOrder, string takerEddsaSignature, string message, string signedMessage) = await Utils.CreateAndSignNftTakerOrderAsync(settings, nftDetails, nftTakerListingDetails, nftListingDetails, orderFee, storageId, takerOrderFee);

                    //Submit the trade
                    var submitTrade = await loopExchangeWebApiService.SubmitTakerTradeAsync(settings.LoopringAccountId, nftListingDetails.Id, nftTakerOrder, takerEddsaSignature, signedMessage);
                    if (submitTrade != null && submitTrade.ToString() == "{}")
                    {
                        Console.WriteLine($"Bought NFT: '{nftDetails.Name}',at price: {listingPriceDecimal} LRC, successfully! Press any key to go back to the options!");
                        Console.ReadKey();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Something went wrong, someone was probably quicker!");
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Something went wrong: {ex.Message}");
                return false;
            }
            return false;
        }
    }
}
