using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoopBot.Models
{
    public class TakerFee
    {
        [JsonProperty("eth")]
        public string Eth { get; set; }

        [JsonProperty("fee")]
        public string Fee { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }
    }
}
