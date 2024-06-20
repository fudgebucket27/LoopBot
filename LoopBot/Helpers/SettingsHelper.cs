using LoopBot.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
                return CreateAppSettingsFile();
            }
            else
            {
                string encryptedJson = File.ReadAllText("appsettings.json");
                var encryptedData = JsonSerializer.Deserialize<EncryptedData>(encryptedJson);

                string decryptedJson = null;
                while (decryptedJson == null)
                {
                    Console.Write("Enter your password: ");
                    string encryptionKey = PromptForPassword();

                    try
                    {
                        decryptedJson = DecryptString(encryptedData.EncryptedSettings, encryptionKey, Convert.FromBase64String(encryptedData.IV));
                    }
                    catch (CryptographicException)
                    {
                        Console.WriteLine("Invalid password. Please try again.");
                    }
                }

                // Settings file
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(decryptedJson)))
                    .AddEnvironmentVariables()
                    .Build();
                Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

                // Check if settings file is populated
                ValidateSettings(settings);

                return settings;
            }
        }

        public static Settings ModifyAppSettingsFile()
        {
            Console.WriteLine("Modifying your appsettings.json file...Follow the prompts below...");

            var settings = new Settings();

            settings.LoopringApiKey = PromptForNonEmptyString("Enter your Loopring Api Key: ");
            settings.LoopringPrivateKey = PromptForNonEmptyString("Enter your Loopring L2 Private Key: ");
            settings.L1PrivateKey = PromptForNonEmptyString("Enter your L1 Private Key: ");
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

            Console.Write("Enter a password to secure your settings: ");
            string encryptionKey = PromptForPassword();
            string encryptedJson = EncryptString(jsonString, encryptionKey, out byte[] iv);

            var encryptedData = new EncryptedData
            {
                EncryptedSettings = encryptedJson,
                IV = Convert.ToBase64String(iv)
            };

            string finalJson = JsonSerializer.Serialize(encryptedData, jsonOptions);

            File.WriteAllText("appsettings.json", finalJson);

            Console.WriteLine("appsettings.json file modified successfully.");
            return settings;
        }

        private static Settings CreateAppSettingsFile()
        {
            Console.WriteLine("Creating a new appsettings.json file...Follow the prompts below...");

            var settings = new Settings();

            settings.LoopringApiKey = PromptForNonEmptyString("Enter your Loopring Api Key: ");
            settings.LoopringPrivateKey = PromptForNonEmptyString("Enter your Loopring L2 Private Key: ");
            settings.L1PrivateKey = PromptForNonEmptyString("Enter your L1 Private Key: ");
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

            Console.Write("Enter a password to secure your settings: ");
            string encryptionKey = PromptForPassword();
            string encryptedJson = EncryptString(jsonString, encryptionKey, out byte[] iv);

            var encryptedData = new EncryptedData
            {
                EncryptedSettings = encryptedJson,
                IV = Convert.ToBase64String(iv)
            };

            string finalJson = JsonSerializer.Serialize(encryptedData, jsonOptions);

            File.WriteAllText("appsettings.json", finalJson);

            Console.WriteLine("appsettings.json file created successfully.");
            return settings;
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

        private static string PromptForPassword()
        {
            string input;
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input.Trim()) || input.Length < 8)
                {
                    Console.Write("The password must be at least 8 characters long.\n\nPlease enter a valid password: ");
                    input = null;
                }
            } while (input == null);

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

        private static string EncryptString(string plainText, string key, out byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // Ensure the key is 32 bytes
                aes.GenerateIV();
                iv = aes.IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        private static string DecryptString(string cipherText, string key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // Ensure the key is 32 bytes
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        private class EncryptedData
        {
            public string EncryptedSettings { get; set; }
            public string IV { get; set; }
        }
    }
}
