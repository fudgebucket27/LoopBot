using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class NftOrderFee
    {
        [JsonProperty("feeRate")]
        public FeeRate FeeRate { get; set; }
        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }
    }

    
    public class FeeRate
    {
        [JsonProperty("nftTokenAddress")]
        public string NftTokenAddress { get; set; }

        [JsonProperty("rate")]
        public int Rate { get; set; }
    }
}
