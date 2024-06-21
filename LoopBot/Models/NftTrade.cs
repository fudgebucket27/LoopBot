using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBot.Models
{
    public class NftTrade
    {
        public NftTakerOrder maker { get; set; }
        public int makerFeeBips { get; set; }
        public NftTakerOrder taker { get; set; }
        public int takerFeeBips { get; set; }
    }
}
