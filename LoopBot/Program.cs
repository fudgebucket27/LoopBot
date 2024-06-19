
using JsonFlatten;
using LoopBot.Helpers;
using LoopBot.Models;
using Microsoft.Extensions.Configuration;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Nethereum.Signer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Tls;
using PoseidonSharp;
using System.Numerics;
using LoopBot.Services;
using System.Text;
using Nethereum.Contracts.QueryHandlers.MultiCall;

class Program
{
    static async Task Main(string[] args)
    {
        var settings = SettingsHelper.GetSettings();
        var serviceManager = ServiceManager.Instance;
        var loginStatus = serviceManager.Initialize(settings.LoopringApiKey, settings.LoopringAccountId, settings.LoopringAddress, settings.L1PrivateKey);

        if (loginStatus == false)
        {
            Console.WriteLine("Login to LoopExchange unsuccessful...");
            Console.WriteLine("Terminating program in 5 seconds...");
            Console.WriteLine("Try again later...");
            await Task.Delay(5000);
            System.Environment.Exit(0);
        }

        bool exit = false;

        while (!exit)
        {
            var selectedMode = OptionsHelper.ChooseMainMenuOptions("Welcome to LoopBot! Choose an option below to begin. Use arrow keys then press enter to select the mode.",
                                                    new string[] { "Monitor collection", "Monitor listing", "Exit" });
            if (selectedMode == 0)
            {
                // Add code for "Monitor collection"
            }
            else if (selectedMode == 1)
            {
                var nftFullId = OptionsHelper.ChooseNftListingOption();
                var priceToBuyDecimal = OptionsHelper.ChoosePriceToBuyOption();
                var delayInSeconds = OptionsHelper.ChooseDelayInSeconds();

                Console.WriteLine($"Monitoring listings for the NFT...");

                // Initialize the stopwatch for the 5-minute interval
                var tokenRefreshStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var nftIsBought = false;
                do
                {
                    if (tokenRefreshStopwatch.Elapsed >= TimeSpan.FromMinutes(5))
                    {
                        // Refresh the token every 5 minutes
                        var newToken = await serviceManager.LoopExchangeApiService.LoginAsync(settings.LoopringAccountId, settings.LoopringAddress, settings.L1PrivateKey);
                        serviceManager.LoopExchangeWebApiService.UpdateAuthorizationHeader(newToken.AccessToken);

                        // Reset the stopwatch
                        tokenRefreshStopwatch.Restart();
                    }
                    // Measure time taken by ListingMode
                    var listingModeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    nftIsBought = await ListingMode(serviceManager, settings, nftFullId, priceToBuyDecimal);
                    listingModeStopwatch.Stop();
                    Console.WriteLine("Restarting");

                    // Calculate the remaining time to wait to ensure a 10-second interval
                    var delay = TimeSpan.FromSeconds(delayInSeconds) - listingModeStopwatch.Elapsed;
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay);
                    }
                } while (nftIsBought == false);
            }
            else if (selectedMode == 2)
            {
                exit = true;
                Console.WriteLine("Exiting LoopBot in 5 seconds. Goodbye!");
                await Task.Delay(TimeSpan.FromSeconds(5));
                System.Environment.Exit(0);
            }
        }
    }

    static async Task<bool> ListingMode(ServiceManager serviceManager, Settings settings, string nftFullId, decimal priceToBuyDecimal)
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
                Console.WriteLine("Valid Listing found for NFT...Attempting to buy.");
                //Get  fees
                var orderFee = await loopringApiService.GetOrderFee(settings.LoopringAccountId, nftDetails.TokenAddress, nftTakerListingDetails.Erc20TokenAmount);
                var takerOrderFee = await loopExchangeApiService.GetTakerFeesAsync(nftListingDetails.Id, settings.LoopringAccountId, nftDetails.TokenAddress, orderFee.FeeRate.Rate, nftTakerListingDetails.Erc20TokenAmount);
                var storageId = await loopringApiService.GetNextStorageId(settings.LoopringAccountId, 1);

                //Sign the order
                (NftOrder nftTakerOrder, string takerEddsaSignature, string message, string signedMessage) = await Utils.CreateAndSignNftTakerOrderAsync(settings, nftDetails, nftTakerListingDetails, nftListingDetails, orderFee, storageId, takerOrderFee);
                
                //Submit the trade
                var submitTrade = await loopExchangeWebApiService.SubmitTradeAsync(settings.LoopringAccountId, nftListingDetails.Id, nftTakerOrder, takerEddsaSignature, signedMessage);
                if (submitTrade != null && submitTrade.ToString() == "{}")
                {
                    Console.WriteLine("Bought NFT successfully! Press any key to go back to the options!");
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
            return false;
        }
        return false;
    }
}



