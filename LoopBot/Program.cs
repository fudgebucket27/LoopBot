
using JsonFlatten;
using LoopBot.Helpers;
using LoopBot.Models;
using Microsoft.Extensions.Configuration;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RLP;
using Nethereum.Signer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Tls;
using PoseidonSharp;
using System.Numerics;
using LoopBot.Services;
using System.Text;

var settings = SettingsHelper.GetSettings();

var selectedMode = OptionsHelper.Choose("Welcome to LoopBot! Choose an option below to begin. Use arrows then press enter to select.", 
                                        new string[] { "Monitor collection", "Monitor listing" });

//The NFT to buy
var nftFullId = ""; //test with 0x16e0eae0799de387be4917d05e8eb00e0a1ccb43-0-0xde2404647c15e8bfb6656e3000bdb4b54cc5a3fa-0xb128327dd0a36ebc1494ffb3b0ea7ea8cfecb01cc6b422ce25330b6dd19f486b-10;
while(string.IsNullOrEmpty(nftFullId) || nftFullId.Split('-').Length != 5)
{
    Console.WriteLine("Enter the full nft id to buy: ");
    nftFullId = Console.ReadLine().Trim();
    if(nftFullId.Split('-').Length != 5)
    {
        Console.WriteLine("Not a valid full nft id. Try again...");
    }
}

//Setup
var loopexchangeLoginMessage = "Welcome to LoopExchange!\n\nClick to sign in and agree to LoopExchange Terms of Service and Privacy policy.\n\nThis request will not trigger a blockchain transaction or cost any gas fees.";
var l1PrivateKey = new EthECKey(settings.L1PrivateKey);
var signer = new EthereumMessageSigner();
var loginSignedMessage = signer.EncodeUTF8AndSign(loopexchangeLoginMessage, l1PrivateKey);
var loopExchangeApiService = new LoopExchangeApiService();
Console.WriteLine("Logging in to LoopExchange...");
var loopExchangeToken = await loopExchangeApiService.LoginAsync(settings.LoopringAccountId, settings.LoopringAddress, loginSignedMessage);
if(loopExchangeToken != null && loopExchangeToken.AccessToken.Length > 0)
{
    Console.WriteLine("Login to LoopExchange successful...");
}
else
{
    Console.WriteLine("Login to LoopExchange unsuccessful...Terminating bot...");
    System.Environment.Exit(0);
}
var loopExchangeWebApiService = new LoopExchangeWebApiService(loopExchangeToken.AccessToken);
var loopringApiService = new LoopringApiService();

//Getting details from LoopExchange
Console.WriteLine($"Getting NFT details from LoopExchange...");
var nftDetails = await loopExchangeWebApiService.GetNftDetailsAsync(nftFullId);
var nftListingDetails = await loopExchangeWebApiService.GetNftListingDetailsAsync(nftFullId);
if(nftListingDetails.Id == null)
{
    Console.WriteLine("Listing not found...Terminating bot...");
    System.Environment.Exit(0);
}
var nftTakerListingDetails = await loopExchangeApiService.GetTakerListingDetailsAsync(nftListingDetails.Id);
var orderFee = await loopringApiService.GetOrderFee(settings.LoopringApiKey, settings.LoopringAccountId, nftDetails.TokenAddress, nftTakerListingDetails.Erc20TokenAmount);
var takerOrderFee = await loopExchangeApiService.GetTakerFeesAsync(nftListingDetails.Id, settings.LoopringAccountId, nftDetails.TokenAddress, orderFee.FeeRate.Rate, nftTakerListingDetails.Erc20TokenAmount);
int nftTokenId = nftTakerListingDetails.NftTokenId;
string nftData = nftTakerListingDetails.NftTokenData;

//Getting the storage id
var storageId = await loopringApiService.GetNextStorageId(settings.LoopringApiKey, settings.LoopringAccountId, 1);

//Creating the nft taker order
NftOrder nftTakerOrder = new NftOrder()
{
    exchange = settings.Exchange,
    accountId = settings.LoopringAccountId,
    storageId = storageId.orderId,
    sellToken = new SellToken
    {
        tokenId = 1,
        amount = nftTakerListingDetails.Erc20TokenAmount
    },
    buyToken = new BuyToken
    {
        tokenId = nftTokenId,
        nftData = nftData,
        amount = "1"
    },
    allOrNone = false,
    fillAmountBOrS = true,
    validUntil = DateTimeOffset.Now.AddDays(30).ToUnixTimeSeconds(),
    maxFeeBips = orderFee.FeeRate.Rate
};
int fillAmountBOrSValue2 = 0;
if (nftTakerOrder.fillAmountBOrS == true)
{
    fillAmountBOrSValue2 = 1;
}

BigInteger[] poseidonTakerOrderInputs =
{
    Utils.ParseHexUnsigned(settings.Exchange),
    (BigInteger) nftTakerOrder.storageId,
    (BigInteger) nftTakerOrder.accountId,
    (BigInteger) nftTakerOrder.sellToken.tokenId,
    !String.IsNullOrEmpty(nftTakerOrder.buyToken.nftData) ? Utils.ParseHexUnsigned(nftTakerOrder.buyToken.nftData) : (BigInteger) nftTakerOrder.buyToken.tokenId ,
    !String.IsNullOrEmpty(nftTakerOrder.sellToken.amount) ? BigInteger.Parse(nftTakerOrder.sellToken.amount) : (BigInteger) 0,
    !String.IsNullOrEmpty(nftTakerOrder.buyToken.amount) ? BigInteger.Parse(nftTakerOrder.buyToken.amount) : (BigInteger) 0,
    (BigInteger) nftTakerOrder.validUntil,
    (BigInteger) nftTakerOrder.maxFeeBips,
    (BigInteger) fillAmountBOrSValue2,
    Utils.ParseHexUnsigned("0x0000000000000000000000000000000000000000")
};

//Generate the poseidon hash
Poseidon poseidon2 = new Poseidon(12, 6, 53, "poseidon", 5, _securityTarget: 128);
BigInteger takerOrderPoseidonHash = poseidon2.CalculatePoseidonHash(poseidonTakerOrderInputs);

//Generate the signaures
Eddsa eddsa2 = new Eddsa(takerOrderPoseidonHash, settings.LoopringPrivateKey);
string takerEddsaSignature = eddsa2.Sign();
nftTakerOrder.eddsaSignature = takerEddsaSignature;
var nftTakerTradeValidateResponse = await loopringApiService.SubmitNftTradeValidateOrder(settings.LoopringApiKey, nftTakerOrder, takerEddsaSignature);
var message = $"Sign this message to complete transaction\n\n{nftDetails.Name}\nQuantity: 1\nPrice: {takerOrderFee.Eth} LRC\nMaximum fees: {takerOrderFee.Fee} LRC\nSold by: {nftListingDetails.Maker}\nNFT data: {nftData}";
var signedMessage = signer.EncodeUTF8AndSign(message, l1PrivateKey);

//Buy from LoopExchange
Console.WriteLine("Buying NFT from LoopExchange...");
var submitTrade = await loopExchangeWebApiService.SubmitTradeAsync(settings.LoopringAccountId, nftListingDetails.Id, nftTakerOrder, takerEddsaSignature, signedMessage);
if(submitTrade != null && submitTrade.ToString() == "{}")
{
    Console.WriteLine("Bought NFT successfully...");
}
else
{
    Console.WriteLine("Something went wrong...");
}

