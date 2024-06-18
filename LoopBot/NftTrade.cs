using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopTradeSharp
{
    public class NftTrade
    {
        public NftOrder maker { get; set; }
        public int makerFeeBips { get; set; }
        public NftOrder taker { get; set; } 
        public int takerFeeBips { get; set; }
    }
}
