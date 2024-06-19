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
    }
}
