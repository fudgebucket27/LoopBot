
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
using System.Reflection;

class Program
{
    static async Task Main(string[] args)
    {
        //Welcome screen
        Assembly assembly = Assembly.GetExecutingAssembly();
        Version version = assembly.GetName().Version;
        Console.WriteLine(".____                       __________        __   \r\n|    |    ____   ____ ______\\______   \\ _____/  |_ \r\n|    |   /  _ \\ /  _ \\\\____ \\|    |  _//  _ \\   __\\\r\n|    |__(  <_> |  <_> )  |_> >    |   (  <_> )  |  \r\n|_______ \\____/ \\____/|   __/|______  /\\____/|__|  \r\n        \\/            |__|          \\/             "+ $"v{version.ToString().Remove(version.ToString().Length - 2,2)}");

        //General Setup
        var settings = SettingsHelper.GetSettings();
        var serviceManager = ServiceManager.Instance;
        var loginStatus = serviceManager.Initialize(settings.LoopringApiKey, settings.LoopringAccountId, settings.LoopringAddress, settings.L1PrivateKey);

        if (!loginStatus)
        {
            Console.WriteLine("Login to LoopExchange unsuccessful...");
            Console.WriteLine("Terminating program in 5 seconds...");
            Console.WriteLine("Try again later...");
            await Task.Delay(5000);
            Environment.Exit(0);
        }

        bool exit = false;
        var tokenRefreshStopwatch = System.Diagnostics.Stopwatch.StartNew();
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // Prevent the process from terminating.
            cts.Cancel();
        };

        //Main option loop
        while (!exit)
        {
            if (cts.Token.IsCancellationRequested)
            {
                cts.Dispose();
                cts = new CancellationTokenSource(); // Reset the cancellation token source
            }

            var selectedMode = OptionsHelper.ChooseMainMenuOptions("Welcome to LoopBot! Choose an option below to begin. Use arrow keys then press enter to select the mode.",
                                                   new string[] { "Monitor collection", "Monitor listing", "Mass listing","Modify appsettings", "Exit" });

            if (selectedMode == 0 && !cts.Token.IsCancellationRequested)
            {
                var url = OptionsHelper.ChooseUrlOption();
                var priceToBuyDecimal = OptionsHelper.ChoosePriceToBuyOption();
                var delayInSeconds = OptionsHelper.ChooseDelayInSeconds();

                Console.WriteLine($"Monitoring listings for the collection...");

                var collectionInfo = await serviceManager.LoopExchangeWebApiService.GetCollectionInfo(url);

                var nftIsBought = false;

                do
                {
                    await RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch);

                    var collectionModeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    nftIsBought = await CollectionMode(serviceManager, settings, collectionInfo.Id, priceToBuyDecimal);
                    collectionModeStopwatch.Stop();

                    var delay = TimeSpan.FromSeconds(delayInSeconds) - collectionModeStopwatch.Elapsed;
                    if (delay > TimeSpan.Zero)
                    {
                        try
                        {
                            await Task.Delay(delay, cts.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            // Handle the task cancellation
                            break;
                        }
                    }
                } while (!nftIsBought && !cts.Token.IsCancellationRequested);
            }
            else if (selectedMode == 1 && !cts.Token.IsCancellationRequested)
            {
                var nftFullId = OptionsHelper.ChooseNftListingOption();
                var priceToBuyDecimal = OptionsHelper.ChoosePriceToBuyOption();
                var delayInSeconds = OptionsHelper.ChooseDelayInSeconds();

                Console.WriteLine($"Monitoring listings for the NFT...");

                var nftIsBought = false;

                do
                {
                    await RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch);

                    var listingModeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    nftIsBought = await ListingMode(serviceManager, settings, nftFullId, priceToBuyDecimal);
                    listingModeStopwatch.Stop();

                    var delay = TimeSpan.FromSeconds(delayInSeconds) - listingModeStopwatch.Elapsed;
                    if (delay > TimeSpan.Zero)
                    {
                        try
                        {
                            await Task.Delay(delay, cts.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            // Handle the task cancellation
                            break;
                        }
                    }
                } while (!nftIsBought && !cts.Token.IsCancellationRequested);
            }
            else if(selectedMode == 2 && !cts.Token.IsCancellationRequested)
            {
                await RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch);
                await OptionsHelper.DisplayNftBalanceWithPagination(serviceManager, settings.LoopringAccountId);
            }
            else if (selectedMode == 3)
            {
                settings = SettingsHelper.ModifyAppSettingsFile();
                await RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch, true);
            }
            else if (selectedMode == 4)
            {
                exit = true;

            }
        }
        cts.Dispose();
        Console.WriteLine("Exiting LoopBot in a few seconds...Goodbye!");
        await Task.Delay(TimeSpan.FromSeconds(2));
    }


    static async Task RefreshTokenIfNeeded(ServiceManager serviceManager, Settings settings, System.Diagnostics.Stopwatch tokenRefreshStopwatch, bool appsettingsChanged = false)
    {
        if (tokenRefreshStopwatch.Elapsed >= TimeSpan.FromMinutes(5) || appsettingsChanged == true)
        {
            // Refresh the token every 5 minutes
            try
            {
                var newToken = await serviceManager.LoopExchangeApiService.LoginAsync(settings.LoopringAccountId, settings.LoopringAddress, settings.L1PrivateKey);
                serviceManager.LoopExchangeWebApiService.UpdateAuthorizationHeader(newToken.AccessToken);
            }
            catch (Exception ex)
            {

            }
            // Reset the stopwatch
            tokenRefreshStopwatch.Restart();
        }
    }


    static async Task<bool> CollectionMode(ServiceManager serviceManager, Settings settings, int collectionId, decimal priceToBuyDecimal)
    {
        //Setup local vars from service class - tiny bit faster
        var loopExchangeApiService = serviceManager.LoopExchangeApiService;
        var loopExchangeWebApiService = serviceManager.LoopExchangeWebApiService;
        var loopringApiService = serviceManager.LoopringApiService;

        try
        {
            //Get the NFT details
            var collectionListings = await loopExchangeWebApiService.GetCollectionListings(collectionId);
            var nftItem = collectionListings.Items
                .Where(item => item.Token1PriceDecimal > 0 && item.Token1PriceDecimal <= priceToBuyDecimal)
                .FirstOrDefault();
            var nftFullId = nftItem != null ? nftItem.NftUrlId : "";
            if (!string.IsNullOrEmpty(nftFullId))
            {
                Console.WriteLine($"Valid listing found for collection, NFT: '{nftItem.Name}',at price: {nftItem.Token1PriceDecimal} LRC...Attempting to buy...");
                var nftDetails = await loopExchangeWebApiService.GetNftDetailsAsync(nftFullId);
                var nftListingDetails = await loopExchangeWebApiService.GetNftListingDetailsAsync(nftFullId);
                var nftTakerListingDetails = await loopExchangeApiService.GetTakerListingDetailsAsync(nftListingDetails.Id);
                var listingPriceDecimal = Utils.ConvertStringToDecimal(nftTakerListingDetails.Erc20TokenAmount);

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
                    Console.WriteLine($"Bought NFT: '{nftDetails.Name}',at price: {nftItem.Token1PriceDecimal} LRC, successfully! Press any key to go back to the options!");
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
                Console.WriteLine($"Valid listing found for NFT: '{nftDetails.Name}',at price: {listingPriceDecimal} LRC...Attempting to buy...");
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



