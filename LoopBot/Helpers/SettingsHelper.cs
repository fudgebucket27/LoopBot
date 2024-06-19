using LoopBot.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Helpers
{
    public static class SettingsHelper
    {
        public static Settings GetSettings()
        {
            if (!File.Exists("appsettings.json"))
            {
                Console.WriteLine("Can not find your appsettings.json file.\nPlease create and set it up in this directory!");
                Console.WriteLine("Terminating program in 5 seconds...");
                Thread.Sleep(5000);
                System.Environment.Exit(0);
            }

            //Settings file
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

            //Check is settings file is populated
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
                    Console.WriteLine($"{field.Key} is null or empty");
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
                Task.Delay(5000);
                System.Environment.Exit(0);
            }

            return settings;
        }
    }
}
