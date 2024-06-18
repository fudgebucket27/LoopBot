using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class ListingDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("batchCount")]
        public int BatchCount { get; set; }

        [JsonProperty("takenCount")]
        public int TakenCount { get; set; }

        [JsonProperty("maker")]
        public string Maker { get; set; }

        [JsonProperty("makerDomain")]
        public string MakerDomain { get; set; }

        [JsonProperty("erc20TokenId")]
        public int Erc20TokenId { get; set; }

        [JsonProperty("erc20TokenAmount")]
        public string Erc20TokenAmount { get; set; }

        [JsonProperty("expires")]
        public DateTimeOffset Expires { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
