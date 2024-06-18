using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Helpers
{
    public static class OptionsHelper
    {
        public static int Choose(string message, string[] options)
        {
            int selectedIndex = 0;
            ConsoleKey key;

            Console.Clear();
            Console.WriteLine(message);

            do
            {
                DisplayOptions(options, selectedIndex);

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

        private static void DisplayOptions(string[] options, int selectedIndex)
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
    }
}
