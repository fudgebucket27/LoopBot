using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Models
{
    public class NftListingResponse
    {
        [JsonProperty("ids")]
        public List<string> Ids { get; set; }
    }
}
