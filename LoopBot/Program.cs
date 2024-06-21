
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
        var settings = SettingsModeHelper.GetSettings();
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
                                                   new string[] { "Monitor collection", "Monitor listing", "Create listings","Modify appsettings", "Exit" });

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
                    await Utils.RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch);

                    var collectionModeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    nftIsBought = await MonitorCollectionModeHelper.Run(serviceManager, settings, collectionInfo.Id, priceToBuyDecimal);
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
                    await Utils.RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch);

                    var listingModeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    nftIsBought = await MonitorListingModeHelper.Run(serviceManager, settings, nftFullId, priceToBuyDecimal);
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
                await Utils.RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch);
                await CreateListingsModeHelper.Run(serviceManager, settings, tokenRefreshStopwatch);
            }
            else if (selectedMode == 3)
            {
                settings = SettingsModeHelper.ModifyAppSettingsFile();
                await Utils.RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch, true);
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
}



