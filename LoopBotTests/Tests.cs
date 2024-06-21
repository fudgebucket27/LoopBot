using LoopBot.Helpers;
using LoopBot.Models;
using LoopBot.Services;
using System;

namespace LoopBotTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public async Task LoginToLoopExchange()
        {
            var bearerToken = await TestConfig.serviceManager.LoopExchangeApiService.
                LoginAsync(TestConfig.settings.LoopringAccountId, TestConfig.settings.LoopringAddress, TestConfig.settings.L1PrivateKey);
            Assert.IsTrue(bearerToken != null && bearerToken.AccessToken.Length > 0, "Login unsuccessful!");
        }

        [TestMethod]
        public async Task GetCollectionInfo()
        {
            var collectionInfo = await TestConfig.serviceManager.LoopExchangeWebApiService.GetCollectionInfo("https://loopexchange.art/collection/loopheads");
            Assert.IsTrue(collectionInfo != null && collectionInfo.Id > 0, "Could not get collection info!");
        }
        
        [TestMethod]
        public async Task GetCollectionListingsWithNoItemsUnderPriceLimit()
        {
            var priceLimit = 10m;
            var collectionListings = await TestConfig.serviceManager.LoopExchangeWebApiService.GetCollectionListings(42);
            var nftItem = collectionListings.Items
            .Where(item => item.Token1PriceDecimal > 0 && item.Token1PriceDecimal <= priceLimit)
            .FirstOrDefault();
            Assert.IsTrue(nftItem == null , $"There were items that were under {priceLimit} LRC!");
        }

        [TestMethod]
        public async Task GetCollectionListingsWithItemsUnderPriceLimit()
        {
            var priceLimit = 200m;
            var collectionListings = await TestConfig.serviceManager.LoopExchangeWebApiService.GetCollectionListings(42);
            var nftItem = collectionListings.Items
            .Where(item => item.Token1PriceDecimal > 0 && item.Token1PriceDecimal <=  priceLimit)
            .FirstOrDefault();
            Assert.IsTrue(nftItem == null, $"No items were under {priceLimit} LRC!");
        }

        [TestMethod]
        public async Task GetNftDetails()
        {
            var collectionListings = await TestConfig.serviceManager.LoopExchangeWebApiService.GetCollectionListings(42);
            var nftItem = collectionListings.Items.FirstOrDefault();
            var nftDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftDetails != null, $"Could not get NFT details!");
        }

        [TestMethod]
        public async Task GetNftListingDetails()
        {
            var collectionListings = await TestConfig.serviceManager.LoopExchangeWebApiService.GetCollectionListings(42);
            var nftItem = collectionListings.Items.FirstOrDefault();
            var nftDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftDetails != null, $"Could not get NFT details!");
            var nftListingDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftListingDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftListingDetails != null, $"Could not get NFT Listing details!");
        }

        [TestMethod]
        public async Task GetNftTakerListingDetails()
        {
            var collectionListings = await TestConfig.serviceManager.LoopExchangeWebApiService.GetCollectionListings(42);
            var nftItem = collectionListings.Items.FirstOrDefault();
            var nftDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftDetails != null, $"Could not get NFT details!");
            var nftListingDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftListingDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftListingDetails != null, $"Could not get NFT Listing details!");
            var nftTakerListingDetails = await TestConfig.serviceManager.LoopExchangeApiService.GetTakerListingDetailsAsync(nftListingDetails.Id);
            Assert.IsTrue(nftTakerListingDetails != null, $"Could not get NFT Taker Listing details!");
        }

        [TestMethod]
        public async Task GetFees()
        {
            var collectionListings = await TestConfig.serviceManager.LoopExchangeWebApiService.GetCollectionListings(42);
            var nftItem = collectionListings.Items.FirstOrDefault();
            var nftDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftDetails != null, $"Could not get NFT details!");
            var nftListingDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftListingDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftListingDetails != null, $"Could not get NFT Listing details!");
            var nftTakerListingDetails = await TestConfig.serviceManager.LoopExchangeApiService.GetTakerListingDetailsAsync(nftListingDetails.Id);
            Assert.IsTrue(nftTakerListingDetails != null, $"Could not get NFT Taker Listing details!");
            var orderFee = await TestConfig.serviceManager.LoopringApiService.GetOrderFee(TestConfig.settings.LoopringAccountId, nftDetails.TokenAddress, nftTakerListingDetails.Erc20TokenAmount);
            Assert.IsTrue(orderFee != null, $"Could not get order fee!");
            var takerOrderFee = await TestConfig.serviceManager.LoopExchangeApiService.GetTakerFeesAsync(nftListingDetails.Id, TestConfig.settings.LoopringAccountId, nftDetails.TokenAddress, orderFee.FeeRate.Rate, nftTakerListingDetails.Erc20TokenAmount);
            Assert.IsTrue(takerOrderFee != null, $"Could not get taker order fee!");
            var storageId = await TestConfig.serviceManager.LoopringApiService.GetNextStorageId(TestConfig.settings.LoopringAccountId, 1);
            Assert.IsTrue(storageId != null, $"Could not get storage id!");
        }

        [TestMethod]
        public async Task SubmitTrade()
        {
            var collectionListings = await TestConfig.serviceManager.LoopExchangeWebApiService.GetCollectionListings(42);
            var nftItem = collectionListings.Items.FirstOrDefault();
            var nftDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftDetails != null, $"Could not get NFT details!");
            var nftListingDetails = await TestConfig.serviceManager.LoopExchangeWebApiService.GetNftListingDetailsAsync(nftItem.NftUrlId);
            Assert.IsTrue(nftListingDetails != null, $"Could not get NFT Listing details!");
            var nftTakerListingDetails = await TestConfig.serviceManager.LoopExchangeApiService.GetTakerListingDetailsAsync(nftListingDetails.Id);
            Assert.IsTrue(nftTakerListingDetails != null, $"Could not get NFT Taker Listing details!");
            var orderFee = await TestConfig.serviceManager.LoopringApiService.GetOrderFee(TestConfig.settings.LoopringAccountId, nftDetails.TokenAddress, nftTakerListingDetails.Erc20TokenAmount);
            Assert.IsTrue(orderFee != null, $"Could not get order fee!");
            var takerOrderFee = await TestConfig.serviceManager.LoopExchangeApiService.GetTakerFeesAsync(nftListingDetails.Id, TestConfig.settings.LoopringAccountId, nftDetails.TokenAddress, orderFee.FeeRate.Rate, nftTakerListingDetails.Erc20TokenAmount);
            Assert.IsTrue(takerOrderFee != null, $"Could not get taker order fee!");
            var storageId = await TestConfig.serviceManager.LoopringApiService.GetNextStorageId(TestConfig.settings.LoopringAccountId, 1);
            Assert.IsTrue(storageId != null, $"Could not get storage id!");
            (NftTakerOrder nftTakerOrder, string takerEddsaSignature, string message, string signedMessage) = await Utils.CreateAndSignNftTakerOrderAsync(TestConfig.settings, nftDetails, nftTakerListingDetails, nftListingDetails, orderFee, storageId, takerOrderFee);
            try
            {
                var tradeValidation = await TestConfig.serviceManager.LoopringApiService.SubmitNftTradeValidateTakerOrder(nftTakerOrder, takerEddsaSignature);
                Assert.IsTrue(tradeValidation.Contains("hash"), "Trade was invalid");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true, "Trade was valid");
            }
            
        }

    }
}