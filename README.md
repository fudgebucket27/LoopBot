# LoopBot
![image](https://github.com/fudgebucket27/LoopBot/assets/5258063/2fbc4150-a1d1-4826-b4b5-bc55836023aa)

A simple bot that will buy NFTs from LoopExchange.

# Install
Download a release [here](https://github.com/fudgebucket27/LoopBot/releases) for your platform. Unzip the contents after downloading it.

# Usage
After the contents have been unzipped run the main executable to begin.

If an appsettings.json file hasn't been created yet you will be prompted to enter the values. 

All of the Loopring related information can be exported from the [Loopring Pro DApp](https://loopring.io/#/pro). Connect and unlock your account and get the related information under the security section show here: 

![image](https://github.com/fudgebucket27/LoopBot/assets/5258063/5335b866-3682-4b0e-863d-3ab8d6a9209d)

LoopBot will direct you with onscreen prompts on how to use it.

# Appsettings.json
Keep your appsettings.json file private!Do not share any of the keys or the file itself with anyone!

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

# TO DO
Ability to monitor multiple nfts

