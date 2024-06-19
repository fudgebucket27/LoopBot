using LoopBot.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Models
{
    public class NftCollectionListing
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("items")]
        public List<NftItem> Items { get; set; }
    }

    public class NftItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nftId")]
        public string NftId { get; set; }

        [JsonProperty("nftUrlId")]
        public string NftUrlId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("imageCached")]
        public string ImageCached { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("listingCount")]
        public int ListingCount { get; set; }

        [JsonProperty("token1Price")]
        public string Token1Price { get; set; }

        [JsonIgnore]
        public decimal Token1PriceDecimal => Utils.ConvertStringToDecimal(Token1Price);
    }
}
