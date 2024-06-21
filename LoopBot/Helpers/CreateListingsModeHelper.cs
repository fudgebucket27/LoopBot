using LoopBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Helpers
{
    public static class CreateListingsModeHelper
    {
        private static void DisplayTopSection()
        {
            Console.WriteLine("Use arrow keys to navigate and press Enter to select an NFT.");
        }

        private static void DisplayNavigationOptions()
        {
            Console.WriteLine("[P]revious Page, [N]ext Page, [M]ark NFT, [L]ist marked NFTs, [E]xit");
        }


        public static async Task Run(ServiceManager serviceManager, Settings settings, System.Diagnostics.Stopwatch tokenRefreshStopwatch)
        {
            int offset = 0;
            bool exitPagination = false;
            int selectedIndex = 0;
            int previousIndex = -1;
            ConsoleKey input = ConsoleKey.NoName;
            var markedNfts = new HashSet<Datum>();

            while (!exitPagination)
            {
                Console.Clear();
                DisplayTopSection();
                DisplayNavigationOptions();

                Console.WriteLine("Getting NFTs...");
                var nftBalance = await serviceManager.LoopringApiService.GetNftBalancePage(settings.LoopringAccountId, offset);

                if (nftBalance == null || nftBalance.TotalNum == 0)
                {
                    Console.WriteLine("No NFTs found.");
                    return;
                }

                Console.Clear();
                DisplayTopSection();
                DisplayNftList(nftBalance, offset, selectedIndex, markedNfts.ToList());
                DisplayNavigationOptions();

                bool optionSelected = false;

                while (!optionSelected)
                {
                    input = Console.ReadKey(true).Key;

                    switch (input)
                    {
                        case ConsoleKey.UpArrow:
                            previousIndex = selectedIndex;
                            selectedIndex = (selectedIndex == 0) ? nftBalance.Data.Count - 1 : selectedIndex - 1;
                            UpdateNftSelection(nftBalance, offset, selectedIndex, previousIndex, markedNfts.ToList());
                            break;
                        case ConsoleKey.DownArrow:
                            previousIndex = selectedIndex;
                            selectedIndex = (selectedIndex == nftBalance.Data.Count - 1) ? 0 : selectedIndex + 1;
                            UpdateNftSelection(nftBalance, offset, selectedIndex, previousIndex, markedNfts.ToList());
                            break;
                        case ConsoleKey.Enter:
                            optionSelected = true;
                            break;
                        case ConsoleKey.N:
                            if (offset + 25 < nftBalance.TotalNum)
                            {
                                offset += 25;
                                selectedIndex = 0;
                                optionSelected = true; // Exit inner loop to fetch new data
                            }
                            else
                            {
                                await DisplayErrorMessage("You are on the last page.");
                            }
                            break;
                        case ConsoleKey.P:
                            if (offset - 25 >= 0)
                            {
                                offset -= 25;
                                selectedIndex = 0;
                                optionSelected = true; // Exit inner loop to fetch new data
                            }
                            else
                            {
                                await DisplayErrorMessage("You are on the first page.");
                            }
                            break;
                        case ConsoleKey.E:
                            exitPagination = true;
                            optionSelected = true;
                            break;
                        case ConsoleKey.M:
                            var selectedNft = nftBalance.Data[selectedIndex];
                            if (!markedNfts.Add(selectedNft))
                            {
                                markedNfts.Remove(selectedNft);
                            }
                            UpdateNftSelection(nftBalance, offset, selectedIndex, previousIndex, markedNfts.ToList());
                            break;
                        case ConsoleKey.L:
                            if (markedNfts.Count > 0)
                            {
                                Console.Clear();
                                await Utils.RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch);
                                await ShowNftMarkedOptions(markedNfts.ToList(), serviceManager, settings);
                                markedNfts.Clear();
                                // Ensure redisplay of the main options after marking process
                                Console.Clear();
                                DisplayTopSection();
                                DisplayNftList(nftBalance, offset, selectedIndex, markedNfts.ToList());
                                DisplayNavigationOptions();
                            }
                            else
                            {
                                await DisplayErrorMessage("No NFTs marked for listing.");
                            }
                            break;
                        default:
                            await DisplayErrorMessage("Invalid input. Please try again.");
                            break;
                    }
                }

                if (input == ConsoleKey.Enter && !exitPagination && selectedIndex < nftBalance.Data.Count)
                {
                    Console.Clear();
                    var selectedNft = nftBalance.Data[selectedIndex];
                    Console.WriteLine($"You selected: {selectedNft.Metadata.Base.Name} - Amount: {selectedNft.Total}");
                    await Utils.RefreshTokenIfNeeded(serviceManager, settings, tokenRefreshStopwatch);
                    await ShowNftOptions(selectedNft, serviceManager, settings);
                }
            }
        }

        private static async Task ShowNftMarkedOptions(List<Datum> markedNfts, ServiceManager serviceManager, Settings settings)
        {
            Console.WriteLine("You have marked the following NFTs for listing:");
            foreach (var nft in markedNfts)
            {
                Console.WriteLine(nft.Metadata.Base.Name);
            }
            var amountToSell = 1;
            var priceToSell = OptionsHelper.ChoosePriceToSellOption();
            var expirationInSeconds = ChooseExpirationOption();
            var innerValidUntil = 0;
            foreach (var nft in markedNfts)
            {
                try
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(expirationInSeconds);
                    DateTimeOffset newDateTimeOffset = dateTimeOffset.AddDays(15).AddSeconds(-1).AddSeconds(innerValidUntil);
                    long orderValidUntil = newDateTimeOffset.ToUnixTimeSeconds();
                    innerValidUntil++;
                    Console.WriteLine($"Submitting listing for: {nft.Metadata.Base.Name}...");
                    var storageId = await serviceManager.LoopringApiService.GetNextStorageId(settings.LoopringAccountId, nft.TokenId);

                    List<(NftMakerOrder order, string eddsaSignature)> makerOrders = new List<(NftMakerOrder, string)>();
                    for (int i = 0; i < amountToSell; i++)
                    {
                        var currentStorageID = storageId.orderId * 2;
                        var platformFee = 2;
                        var maxFeeBips = (nft.RoyaltyPercentage.Value + platformFee) * 100;
                        (NftMakerOrder makerOrder, string signature) = await Utils.CreateAndSignNftMakerOrderAsync(settings, nft.TokenId, nft.NftData, Utils.ConvertDecimalToStringRepresentation(priceToSell), currentStorageID, maxFeeBips, settings.LoopringAddress, orderValidUntil);
                        makerOrders.Add((makerOrder, signature));
                        var loopringResponse = await serviceManager.LoopringApiService.SubmitNftTradeValidateMakerOrder(makerOrder, signature);
                    }
                    var response = await serviceManager.LoopExchangeWebApiService.SubmitMakerTradeAsync(makerOrders, expirationInSeconds);
                    if (response != null && response.Ids.Count > 0)
                    {
                        Console.WriteLine("Listing successful! Here is your listing link: ");
                        var listingLink = response.Ids.First();
                        Console.WriteLine($"https://loopexchange.art/b/{listingLink}\n");

                    }
                    else
                    {
                        Console.WriteLine("Something went wrong! Try again...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something went wrong! Try again...{ex.Message}");
                }
            }

            Console.WriteLine("Listing complete! Press 'q' to continue...");
            while (Console.ReadKey(true).Key != ConsoleKey.Q)
            {
            }
            Console.Clear();
        }
        private static void DisplayNftList(NftBalance nftBalance, int offset, int selectedIndex, List<Datum> markedNfts)
        {
            for (int i = 0; i < nftBalance.Data.Count; i++)
            {
                var nft = nftBalance.Data[i];
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"> {i + 1 + offset}. {nft.Metadata.Base.Name} {(markedNfts.Any(m => m.Id == nft.Id) ? "[MARKED]" : "")}".PadRight(100));
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"{i + 1 + offset}. {nft.Metadata.Base.Name} {(markedNfts.Any(m => m.Id == nft.Id) ? "[MARKED]" : "")}".PadRight(100));
                }
            }
        }

        private static void UpdateNftSelection(NftBalance nftBalance, int offset, int selectedIndex, int previousIndex, List<Datum> markedNfts)
        {
            if (previousIndex != -1)
            {
                Console.SetCursorPosition(0, previousIndex + 1); // Adjust 1 to match the number of lines before the NFT list
                var previousNft = nftBalance.Data[previousIndex];
                Console.WriteLine($"{previousIndex + 1 + offset}. {previousNft.Metadata.Base.Name} {(markedNfts.Any(m => m.Id == previousNft.Id) ? "[MARKED]" : "")}".PadRight(100)); // Extra spaces to overwrite previous text
            }

            Console.SetCursorPosition(0, selectedIndex + 1); // Adjust 1 to match the number of lines before the NFT list
            var selectedNft = nftBalance.Data[selectedIndex];
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"> {selectedIndex + 1 + offset}. {selectedNft.Metadata.Base.Name} {(markedNfts.Any(m => m.Id == selectedNft.Id) ? "[MARKED]" : "")}".PadRight(100)); // Extra spaces to overwrite previous text
            Console.ResetColor();
        }


        private static async Task DisplayErrorMessage(string message)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.WriteLine(message.PadRight(Console.WindowWidth - 1));
            await Task.Delay(500);
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.WriteLine(new string(' ', Console.WindowWidth - 1)); // Clear the message
        }

        public static long ChooseExpirationOption()
        {
            string[] options = { "in an hour", "in a day", "in 7 days", "in 14 days", "in a month" };
            int selectedIndex = 0;
            Console.WriteLine("Please choose the expiry date for the listing. Use the up and down arrows, Enter to select:");
            int topCursorPos = Console.CursorTop; // Store the top cursor position

            while (true)
            {
                // Display the options without clearing the entire console
                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(0, topCursorPos + i); // Move cursor to the correct line

                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"> {options[i]}"); // Clear the line
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}"); // Clear the line
                    }
                }

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow)
                {
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
                }
                else if (key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            int expirationInSeconds = options[selectedIndex] switch
            {
                "in an hour" => 3600,
                "in a day" => 86400,
                "in 7 days" => 604800,
                "in 14 days" => 1209600,
                "in a month" => 2592000,
                _ => throw new ArgumentOutOfRangeException()
            };

            DateTimeOffset now = DateTimeOffset.UtcNow;
            long expirationTimestamp = now.ToUnixTimeSeconds() + expirationInSeconds;
            return expirationTimestamp;
        }

        public static async Task ShowNftOptions(Datum nft, ServiceManager serviceManager, Settings settings)
        {
            var amountToSell = OptionsHelper.ChooseAmountToSellOption(Int32.Parse(nft.Total));
            var priceToSell = OptionsHelper.ChoosePriceToSellOption();
            var expirationInSeconds = ChooseExpirationOption();
            try
            {
                Console.WriteLine("Submitting listing...");
                var storageId = await serviceManager.LoopringApiService.GetNextStorageId(settings.LoopringAccountId, nft.TokenId);
                List<(NftMakerOrder order, string eddsaSignature)> makerOrders = new List<(NftMakerOrder, string)>();
                var innerValidUntil = 0;
                for (int i = 0; i < amountToSell; i++)
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(expirationInSeconds);
                    DateTimeOffset newDateTimeOffset = dateTimeOffset.AddDays(15).AddSeconds(-1).AddSeconds(innerValidUntil);
                    long orderValidUntil = newDateTimeOffset.ToUnixTimeSeconds();

                    var currentStorageID = storageId.orderId * 2;
                    var platformFee = 2;
                    var maxFeeBips = (nft.RoyaltyPercentage.Value + platformFee) * 100;
                    (NftMakerOrder makerOrder, string signature) = await Utils.CreateAndSignNftMakerOrderAsync(settings, nft.TokenId, nft.NftData, Utils.ConvertDecimalToStringRepresentation(priceToSell), currentStorageID, maxFeeBips, settings.LoopringAddress, orderValidUntil);
                    makerOrders.Add((makerOrder, signature));
                    innerValidUntil++;
                    var loopringResponse = await serviceManager.LoopringApiService.SubmitNftTradeValidateMakerOrder(makerOrder, signature);
                }
                var loopexResponse = await serviceManager.LoopExchangeWebApiService.SubmitMakerTradeAsync(makerOrders, expirationInSeconds);
                if (loopexResponse != null && loopexResponse.Ids.Count > 0)
                {
                    Console.WriteLine("Listing successful! Here is your listing link: ");
                    var listingLink = loopexResponse.Ids.First();
                    Console.WriteLine($"https://loopexchange.art/b/{listingLink}");
                    Console.WriteLine("\nListing complete! Press 'q' to continue...");
                    while (Console.ReadKey(true).Key != ConsoleKey.Q)
                    {
                    }
                }
                else
                {
                    Console.WriteLine("Something went wrong! Try again...");
                    Console.WriteLine("\nPress 'q' to continue...");
                    while (Console.ReadKey(true).Key != ConsoleKey.Q)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong! Try again...{ex.Message}");
                Console.WriteLine("\nPress 'q' to continue...");
                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {
                }
            }

        }
    }
}
