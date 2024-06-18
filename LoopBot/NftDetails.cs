using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class NftDetails
    {
        [JsonProperty("minter")]
        public string Minter { get; set; }

        [JsonProperty("nftType")]
        public int NftType { get; set; }

        [JsonProperty("tokenAddress")]
        public string TokenAddress { get; set; }

        [JsonProperty("nftId")]
        public string NftId { get; set; }

        [JsonProperty("royaltyPercentage")]
        public int RoyaltyPercentage { get; set; }

        [JsonProperty("nftUrlId")]
        public string NftUrlId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("metadataUri")]
        public string MetadataUri { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; }

        [JsonProperty("animationUri")]
        public string AnimationUri { get; set; }

        [JsonProperty("amountMinted")]
        public int AmountMinted { get; set; }

        [JsonProperty("collectionItem")]
        public object CollectionItem { get; set; }

    }

}
