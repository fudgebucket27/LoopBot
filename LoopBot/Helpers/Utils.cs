using LoopBot.Models;
using LoopBot.Services;
using Nethereum.Signer;
using PoseidonSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace LoopBot.Helpers
{
    public static class Utils
    {
        public static BigInteger ParseHexUnsigned(string toParse)
        {
            toParse = toParse.Replace("0x", "");
            var parsResult = BigInteger.Parse(toParse, System.Globalization.NumberStyles.HexNumber);
            if (parsResult < 0)
                parsResult = BigInteger.Parse("0" + toParse, System.Globalization.NumberStyles.HexNumber);
            return parsResult;
        }

        public static string UrlEncodeUpperCase(string stringToEncode)
        {
            var reg = new Regex(@"%[a-f0-9]{2}");
            stringToEncode = HttpUtility.UrlEncode(stringToEncode);
            return reg.Replace(stringToEncode, m => m.Value.ToUpperInvariant());
        }

        public static string ConvertDecimalToStringRepresentation(decimal value)
        {
            // Multiply by 10^18 LRC token decimals
            decimal scaledValue = value * 1000000000000000000m;
            // Convert to string without scientific notation
            return scaledValue.ToString("0");
        }

        public static decimal ConvertStringToDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0m;
            }
            try
            {
                decimal parsedValue = decimal.Parse(value);
                // Divide by 10^18 to get the original decimal value
                decimal originalValue = parsedValue / 1000000000000000000m;
                return originalValue;
            }
            catch (FormatException)
            {
                return 0m;
            }
        }

        public static async Task<(NftOrder, string, string, string)> CreateAndSignNftTakerOrderAsync(Settings settings, NftDetails nftDetails, TakerListingDetails nftTakerListingDetails, ListingDetails nftListingDetails, NftOrderFee orderFee, StorageId storageId, TakerFee takerOrderFee)
        {
            int nftTokenId = nftTakerListingDetails.NftTokenId;
            string nftData = nftTakerListingDetails.NftTokenData;

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

            int fillAmountBOrSValue2 = nftTakerOrder.fillAmountBOrS ? 1 : 0;

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

            //Generate the signatures
            Eddsa eddsa2 = new Eddsa(takerOrderPoseidonHash, settings.LoopringPrivateKey);
            string takerEddsaSignature = eddsa2.Sign();
            nftTakerOrder.eddsaSignature = takerEddsaSignature;
            //var nftTakerTradeValidateResponse = await loopringApiService.SubmitNftTradeValidateOrder(nftTakerOrder, takerEddsaSignature);
            var message = $"Sign this message to complete transaction\n\n{nftDetails.Name}\nQuantity: 1\nPrice: {takerOrderFee.Eth} LRC\nMaximum fees: {takerOrderFee.Fee} LRC\nSold by: {nftListingDetails.Maker}\nNFT data: {nftData}";
            var l1Key = new EthECKey(settings.L1PrivateKey);
            var signer = new EthereumMessageSigner();
            var signedMessage = signer.EncodeUTF8AndSign(message, l1Key);

            return (nftTakerOrder, takerEddsaSignature, message, signedMessage);
        }
    }
}
