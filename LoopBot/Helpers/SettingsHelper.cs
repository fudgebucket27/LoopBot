using LoopBot.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoopBot.Helpers
{
    public static class SettingsHelper
    {
        public static Settings GetSettings()
        {
            if (!File.Exists("appsettings.json"))
            {
                Console.WriteLine("Cannot find your appsettings.json file.");
                CreateAppSettingsFile();
            }

            //Settings file
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

            //Check if settings file is populated
            ValidateSettings(settings);

            return settings;
        }

        private static void CreateAppSettingsFile()
        {
            Console.WriteLine("Creating a new appsettings.json file...Follow the prompts below...");

            var settings = new Settings();

            settings.LoopringApiKey = PromptForNonEmptyString("Enter your Loopring Api Key: ");
            settings.LoopringPrivateKey = PromptForNonEmptyString("Enter your Loopring L2 Private Key: ");
            settings.L1PrivateKey = PromptForNonEmptyString("Enter your L1 PrivateKey: ");
            settings.LoopringAddress = PromptForNonEmptyString("Enter your Loopring Address in 0x format: ");
            settings.LoopringAccountId = PromptForNonZeroInt("Enter your Loopring Account Id: ");
            settings.Exchange = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4";

            var settingsObject = new
            {
                Settings = settings
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(settingsObject, jsonOptions);

            File.WriteAllText("appsettings.json", jsonString);

            Console.WriteLine("appsettings.json file created successfully.");
        }

        private static string PromptForNonEmptyString(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input.Trim()))
                {
                    Console.WriteLine("This field cannot be empty. Please enter a value.");
                }
            } while (string.IsNullOrEmpty(input));

            return input;
        }

        private static int PromptForNonZeroInt(string prompt)
        {
            int input;
            do
            {
                Console.Write(prompt);
                if (!int.TryParse(Console.ReadLine(), out input) || input == 0)
                {
                    Console.WriteLine("This field must be a non-zero integer. Please enter a valid value.");
                }
            } while (input == 0);

            return input;
        }

        private static void ValidateSettings(Settings settings)
        {
            var settingsDictionary = new Dictionary<string, object>
            {
                { "LoopringApiKey", settings.LoopringApiKey },
                { "LoopringPrivateKey", settings.LoopringPrivateKey },
                { "LoopringAddress", settings.LoopringAddress },
                { "LoopringAccountId", settings.LoopringAccountId },
                { "L1PrivateKey", settings.L1PrivateKey },
                { "Exchange", settings.Exchange }
            };

            int invalidSettingsCount = 0;
            foreach (var field in settingsDictionary)
            {
                if (field.Value is string strValue && string.IsNullOrEmpty(strValue))
                {
                    Console.WriteLine($"{field.Key} is null or empty.");
                    invalidSettingsCount++;
                }
                else if (field.Value is int intValue && intValue == 0)
                {
                    Console.WriteLine($"{field.Key} is null or empty.");
                    invalidSettingsCount++;
                }
                else if (field.Value == null)
                {
                    Console.WriteLine($"{field.Key} is null.");
                    invalidSettingsCount++;
                }
            }

            if (invalidSettingsCount > 0)
            {
                Console.WriteLine("You are missing some settings in your appsettings.json file. Please add them.");
                Console.WriteLine("Terminating program in 5 seconds...");
                Task.Delay(5000).Wait();
                System.Environment.Exit(0);
            }
        }
    }
}
