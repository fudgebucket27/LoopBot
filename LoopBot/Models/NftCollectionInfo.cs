using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Models
{
    public class NftCollectionInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("chainID")]
        public int ChainID { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("hasRarity")]
        public bool HasRarity { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("profileImage")]
        public string ProfileImage { get; set; }

        [JsonProperty("bannerImage")]
        public string BannerImage { get; set; }

        [JsonProperty("featuredImage")]
        public string FeaturedImage { get; set; }

        [JsonProperty("floorPrice1")]
        public string FloorPrice1 { get; set; }

        [JsonProperty("volume1")]
        public string Volume1 { get; set; }

        [JsonProperty("minter")]
        public string Minter { get; set; }

        [JsonProperty("minterDomain")]
        public string MinterDomain { get; set; }

        [JsonProperty("minterDisplayName")]
        public string MinterDisplayName { get; set; }

        [JsonProperty("minterVerified")]
        public bool MinterVerified { get; set; }
    }
}
