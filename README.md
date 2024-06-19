# LoopBot
A simple demo bot that will buy NFTs from LoopExchange.

# Install
Download a release [here][(https://github.com/fudgebucket27/LoopBot/releases) for your platform.

# Appsettings.json
Keep your appsettings.json file secure and do not share any of the keys or the file itself with anyone!

# Building from source
This is a .NET application so you will need an IDE like Visual Studio / Visual Studio code to compile it. You will also need .NET 6.

You will need to create an appsettings.json file in the LoopBot directory with the setting "Copy to Output Directory" set to "Copy Always" like below. 

DO NOT SHARE ANY OF YOUR KEYS OR THE FILE WITH ANYONE!
```json
{

  "Settings": {
    "LoopringApiKey": "kdlblahaha", //Your loopring api key
    "LoopringPrivateKey": "0xbalahha", //Your Loopring private key
    "LoopringAddress": "0xblahaha", //Your loopring address
    "LoopringAccountId": 40940, //Your loopring account id
    "L1PrivateKey": "assdsxa", //Your L1 Private Key
    "Exchange": "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4" //Loopring Exchange address
  }
}
```

## TO DO
Ability to monitor multiple nfts

