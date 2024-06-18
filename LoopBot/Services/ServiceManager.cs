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

    public void Initialize(string loopringApiKey, int loopringAccountId, string loopringAddress, string l1PrivateKey)
    {
        if (LoopExchangeApiService != null || LoopExchangeWebApiService != null || LoopringApiService != null)
        {
            throw new InvalidOperationException("ServiceManager is already initialized.");
        }

        var loopexchangeLoginMessage = "Welcome to LoopExchange!\n\nClick to sign in and agree to LoopExchange Terms of Service and Privacy policy.\n\nThis request will not trigger a blockchain transaction or cost any gas fees.";
        var l1Key = new EthECKey(l1PrivateKey);
        var signer = new EthereumMessageSigner();
        var loginSignedMessage = signer.EncodeUTF8AndSign(loopexchangeLoginMessage, l1Key);

        LoopExchangeApiService = new LoopExchangeApiService();
        var loopExchangeToken = LoopExchangeApiService.LoginAsync(loopringAccountId, loopringAddress, loginSignedMessage).GetAwaiter().GetResult();
        if (loopExchangeToken != null && loopExchangeToken.AccessToken.Length > 0)
        {
            LoopExchangeWebApiService = new LoopExchangeWebApiService(loopExchangeToken.AccessToken);
            LoopringApiService = new LoopringApiService(loopringApiKey);
        }
        else
        {
            Console.WriteLine("Login to LoopExchange unsuccessful...Terminating bot...");
            System.Environment.Exit(0);
        }
    }
}
