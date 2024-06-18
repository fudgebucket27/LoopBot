using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class BearerToken
    {
        [JsonProperty("accessToken")]
        public string AccessToken {  get; set; }
    }
}
