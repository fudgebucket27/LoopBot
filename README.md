![image](https://github.com/fudgebucket27/LoopBot/assets/5258063/02d482ae-d6b8-4d42-8dd8-a87d855b15d5)

# What is this?
A simple bot that will buy NFTs from LoopExchange.  

# Demo
https://github.com/fudgebucket27/LoopBot/assets/5258063/461b90cd-1fd9-4282-a55d-355ef5d01e80

# Features
- Monitor a collection for new listings and buy an NFT under a certain price point
- Monitor a NFT for new listings and buy it under a certain price point.

# Install
Download a release [here](https://github.com/fudgebucket27/LoopBot/releases) for your platform, LoopBot is is available on Windows,macOS and Linux. Unzip the contents after downloading it.

# macOS users
For macOS users: You also need to run the following command in the unzipped folder of LoopBot to turn it into an executable in order to run it. You may also need to add it as a trusted application if it get's blocked from running.

```bash
chmod +x LoopBot
```

# Usage
After the contents have been unzipped run the main executable to begin.

If an appsettings.json file hasn't been created yet, you will be prompted to enter the values. 

All of the Loopring related information can be exported from the [Loopring Pro DApp](https://loopring.io/#/pro). Connect and unlock your account and get the related information under the security section shown here: 

![image](https://github.com/fudgebucket27/LoopBot/assets/5258063/5335b866-3682-4b0e-863d-3ab8d6a9209d)

LoopBot will direct you with onscreen prompts on how to use it.

# Appsettings.json
Keep your appsettings.json file private! Do not share any of the keys or the file itself with anyone!

# Building from source
Clone this repository.

This is a .NET application so you will need an IDE like Visual Studio / Visual Studio Code to compile it. You will also need .NET 6.

Make sure the appsettings.json is not included in your source files, though the current .gitignore should have this set already. 

# TO DO
- Ability to monitor and buy multiple nfts
- LayerLoot support
