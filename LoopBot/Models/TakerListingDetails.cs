using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Models
{
    using Newtonsoft.Json;
    using System;

    public class TakerListingDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("chainID")]
        public int ChainID { get; set; }

        [JsonProperty("batchCount")]
        public int BatchCount { get; set; }

        [JsonProperty("takenCount")]
        public int TakenCount { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("maker")]
        public string Maker { get; set; }

        [JsonProperty("makerDomain")]
        public string MakerDomain { get; set; }

        [JsonProperty("expires")]
        public DateTime Expires { get; set; }

        [JsonProperty("nftTokenId")]
        public int NftTokenId { get; set; }

        [JsonProperty("nftTokenAmount")]
        public string NftTokenAmount { get; set; }

        [JsonProperty("nftTokenData")]
        public string NftTokenData { get; set; }

        [JsonProperty("erc20TokenID")]
        public int Erc20TokenID { get; set; }

        [JsonProperty("erc20TokenAmount")]
        public string Erc20TokenAmount { get; set; }

        [JsonProperty("erc20TokenName")]
        public string Erc20TokenName { get; set; }

        [JsonProperty("metadataUri")]
        public string MetadataUri { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; }

        [JsonProperty("nftUrlId")]
        public string NftUrlId { get; set; }

        [JsonProperty("nftMinterAddress")]
        public string NftMinterAddress { get; set; }

        [JsonProperty("nftMinterDomain")]
        public string NftMinterDomain { get; set; }

        [JsonProperty("nftCollectionId")]
        public string NftCollectionId { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("collectionId")]
        public int CollectionId { get; set; }

        [JsonProperty("collectionItemId")]
        public int CollectionItemId { get; set; }

        [JsonProperty("collectionName")]
        public string CollectionName { get; set; }

        [JsonProperty("collectionSlug")]
        public string CollectionSlug { get; set; }

        [JsonProperty("collectionAddress")]
        public string CollectionAddress { get; set; }
    }

}
