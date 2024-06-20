﻿using ADRaffy.ENSNormalize;
using LoopBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Helpers
{
    public static class OptionsHelper
    {
        public static int ChooseMainMenuOptions(string message, string[] options)
        {
            int selectedIndex = 0;
            ConsoleKey key;

            Console.Clear();
            Console.WriteLine(message);

            do
            {
                DisplayMainMenuOptions(options, selectedIndex);

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedIndex > 0) selectedIndex--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (selectedIndex < options.Length - 1) selectedIndex++;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        Console.WriteLine($"You selected: {options[selectedIndex]}");
                        break;
                }

            } while (key != ConsoleKey.Enter);
            return selectedIndex;
        }

        private static void DisplayMainMenuOptions(string[] options, int selectedIndex)
        {
            Console.SetCursorPosition(0, 1);

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.WriteLine(options[i]);
            }
            Console.ResetColor();
        }

        public static string ChooseNftListingOption()
        {
            string nftFullId = ""; //test with 0x16e0eae0799de387be4917d05e8eb00e0a1ccb43-0-0xde2404647c15e8bfb6656e3000bdb4b54cc5a3fa-0xb128327dd0a36ebc1494ffb3b0ea7ea8cfecb01cc6b422ce25330b6dd19f486b-10;
            while (string.IsNullOrEmpty(nftFullId) || nftFullId.Split('-').Length != 5)
            {
                Console.WriteLine("Enter the full nft id to buy:");
                nftFullId = Console.ReadLine().Trim();
                if (nftFullId.Split('-').Length != 5)
                {
                    Console.WriteLine("Not a valid full nft id. Try again...");
                }
            }
            return nftFullId;
        }

        public static string ChooseUrlOption()
        {
            string url;
            do
            {
                Console.WriteLine("Enter the LoopExchange URL for the collection(e.g https://loopexchange.art/collection/loopheads):");
                url = Console.ReadLine().Trim();
            } while (!Uri.IsWellFormedUriString(url, UriKind.Absolute));
            return url;
        }

        public static decimal ChoosePriceToBuyOption()
        {
            var priceToBuyDecimal = 0m;
            while (true)
            {
                Console.WriteLine("Please enter a price to buy at in LRC:");
                string input = Console.ReadLine();
                if (decimal.TryParse(input, out priceToBuyDecimal))
                {
                    if (priceToBuyDecimal != 0)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("The price cannot be zero. Please enter a valid price.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid price. Please enter a valid price");
                }
            }
            return priceToBuyDecimal;
        }

        public static int ChooseDelayInSeconds()
        {
            int delay;
            string input;
            do
            {
                Console.WriteLine("Enter how often to check listings in seconds (must be greater than or equal to 1)");
                input = Console.ReadLine();
            } while (!int.TryParse(input, out delay) || delay < 1);
            return delay;
        }

        public static async Task DisplayNftBalanceWithPagination(ServiceManager serviceManager, int accountId)
        {
            int offset = 0;
            bool exitPagination = false;
            int selectedIndex = 0;

            while (!exitPagination)
            {
                Console.WriteLine("Getting NFTs...");
                var nftBalance = await serviceManager.LoopringApiService.GetNftBalancePage(accountId, offset);
                Console.Clear();

                if (nftBalance == null || nftBalance.TotalNum == 0)
                {
                    Console.WriteLine("No NFTs found.");
                    return;
                }

                bool optionSelected = false;

                while (!optionSelected)
                {
                    DisplayTopSection();

                    // Display NFTs with the selected index
                    DisplayNftList(nftBalance, offset, selectedIndex);

                    // Display navigation options
                    DisplayNavigationOptions();

                    var input = Console.ReadKey(true).Key;

                    switch (input)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex = (selectedIndex == 0) ? nftBalance.Data.Count - 1 : selectedIndex - 1;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex = (selectedIndex == nftBalance.Data.Count - 1) ? 0 : selectedIndex + 1;
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
                                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                                Console.WriteLine("You are on the last page.       ");
                                await Task.Delay(1000);
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
                                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                                Console.WriteLine("You are on the first page.      ");
                                await Task.Delay(1000);
                            }
                            break;
                        case ConsoleKey.E:
                            exitPagination = true;
                            optionSelected = true;
                            break;
                        default:
                            Console.SetCursorPosition(0, Console.WindowHeight - 1);
                            Console.WriteLine("Invalid input. Please try again.");
                            await Task.Delay(1000);
                            break;
                    }
                }

                if (!exitPagination && selectedIndex < nftBalance.Data.Count)
                {
                    Console.Clear();
                    var selectedNft = nftBalance.Data[selectedIndex];
                    Console.WriteLine($"You selected: {selectedNft.Metadata.Base.Name} - Amount: {selectedNft.Total}");
                    // Here you can add the logic for the selected NFT (e.g., showing more options or details)
                    await ShowNftOptions(selectedNft);
                }
            }
            Console.WriteLine("Exiting NFT selection...");
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        private static void DisplayTopSection()
        {
            Console.Clear();
            Console.WriteLine("Use arrow keys to navigate and press Enter to select an NFT.");
        }

        private static void DisplayNftList(NftBalance nftBalance, int offset, int selectedIndex)
        {
            int displayCount = Math.Min(nftBalance.Data.Count, Console.WindowHeight - 3);

            for (int i = 0; i < displayCount; i++)
            {
                var nft = nftBalance.Data[i];
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"> {i + 1 + offset}. {nft.Metadata.Base.Name}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"{i + 1 + offset}. {nft.Metadata.Base.Name}");
                }
            }

            // Clear remaining lines if any
            int currentLineCursor = displayCount + 1;
            while (currentLineCursor < Console.WindowHeight - 3)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
                currentLineCursor++;
            }
        }

        private static void DisplayNavigationOptions()
        {
            Console.WriteLine("\n[N]ext Page, [P]revious Page, [E]xit");
        }




        public static async Task ShowNftOptions(Datum nft)
        {
            // Placeholder method for showing more options or details for the selected NFT
            Console.WriteLine($"Showing options for NFT: {nft.Metadata.Base.Name}");
            await Task.Delay(2000);
        }

    }
}
