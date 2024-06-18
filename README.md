# LoopTradeSharp
NFT Trading via the Loopring API in C#. This is made in .NET 6. This is more of a demo on how to make a valid trade between two accounts. You will need two seperate Loopring Accounts to use this demo. You will need a maker and taker account. In this example the taker will settle the trade. 

You will need to create an appsettings.json file in the directory with the setting "Copy to Output Directory" set to "Copy Always" like below. 

```json
{

  "Settings": {
    "LoopringApiKey": "kdlblahaha", //The maker api key
    "LoopringPrivateKey": "0xbalahha", //The maker private key
    "LoopringAddress": "0xblahaha", //The maker address
    "LoopringAccountId": 40940, //The maker account id

    "LoopringApiKey2": "0Vtblahaha", //The taker api key
    "LoopringPrivateKey2": "0xblahaha", //The taker private key
    "LoopringAddress2": "0xblahblah", //The taker address
    "LoopringAccountId2": 77900, //The taker account id
    
    "Exchange": "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4" //Loopring Exchange address
  }
}
```

Edit the nftTokenId and nftData in the code to the specific NFT in the maker's account.


A successful NFT Trade returns a response like below:

```json
{
  "makerFills": {
    "orderHash": "0x2833cbdf9214273ab23458ec5b3ac4db1811fbfdaf49b72f8c7d0c6266941b7c",
    "sellFilled": {
      "tokenId": 33769,
      "nftData": "0x19b1d363f3cc86b43f01813bfd4daa9acc1afd4a81f0d50a4b780bf005d074c5",
      "amount": "1"
    },
    "buyFilled": {
      "tokenId": 1,
      "amount": "10000000000000"
    },
    "fee": {
      "tokenId": 1,
      "amount": "1000000000000"
    }
  },
  "takerFills": {
    "orderHash": "0x2657a8867af935eb9f13f19236f5ff4c25d811ca3a4107a197a7aae61fe313d6",
    "sellFilled": {
      "tokenId": 1,
      "amount": "10000000000000"
    },
    "buyFilled": {
      "tokenId": 33022,
      "nftData": "0x19b1d363f3cc86b43f01813bfd4daa9acc1afd4a81f0d50a4b780bf005d074c5",
      "amount": "1"
    },
    "fee": {
      "tokenId": 1,
      "amount": "100000000000"
    }
  },
  "tradeHash": "0x0bb745f5ddad10e56465235b265efde37fcc598e929a1e9b76684d50388d3a79"
}
```
