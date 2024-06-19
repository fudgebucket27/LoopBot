using LoopBot.Services;
using Nethereum.Signer;
using System;

public class ServiceManager
{
    private static readonly Lazy<ServiceManager> instance = new Lazy<ServiceManager>(() => new ServiceManager());

    public LoopExchangeApiService LoopExchangeApiService { get; private set; }
    public LoopExchangeWebApiService LoopExchangeWebApiService { get; private set; }
    public LoopringApiService LoopringApiService { get; private set; }

    private ServiceManager() { }

    public static ServiceManager Instance { get { return instance.Value; } }

    public bool Initialize(string loopringApiKey, int loopringAccountId, string loopringAddress, string l1PrivateKey)
    {
        if (LoopExchangeApiService != null || LoopExchangeWebApiService != null || LoopringApiService != null)
        {
            throw new InvalidOperationException("ServiceManager is already initialized.");
        }

        LoopExchangeApiService = new LoopExchangeApiService();
        var loopExchangeToken = LoopExchangeApiService.LoginAsync(loopringAccountId, loopringAddress, l1PrivateKey).GetAwaiter().GetResult();
        if (loopExchangeToken != null && loopExchangeToken.AccessToken.Length > 0)
        {
            LoopExchangeWebApiService = new LoopExchangeWebApiService(loopExchangeToken.AccessToken);
            LoopringApiService = new LoopringApiService(loopringApiKey);
            return true;
        }
        else
        {
            return false;
        }
    }
}
