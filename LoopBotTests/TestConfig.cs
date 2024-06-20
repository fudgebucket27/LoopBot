using LoopBot;
using LoopBot.Models;
using LoopBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace LoopBotTests
{
    public static class TestConfig
    {
        public static Settings settings;
        public static ServiceManager serviceManager;

        static TestConfig()
        {
            IConfiguration config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables()
                   .Build();
            settings = config.GetRequiredSection("Settings").Get<Settings>();
            serviceManager = ServiceManager.Instance;
            serviceManager.Initialize(settings.LoopringApiKey, settings.LoopringAccountId, settings.LoopringAddress, settings.L1PrivateKey);
        }
    }
}
