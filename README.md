# 7DaysToDie-ServerKit
English | [简体中文](./README.zh.md)

[Front-end](https://github.com/1249993110/7DaysToDie-ServerKit-webui)

A open source mod for 7 Days to Die dedicated servers. Provides RESTful APIs and a powerful web management panel to help owners and administrators operate their dedicated server. Works with both Linux and Windows. (Ubuntu 22.04 and Windows Server 2016+ test passed)

## Compatibility
Dedicated Server only. Required game version: V1.0

## Features
- Support online rendering of any map
- Support viewing and modifying serverconfig.xml
- You can switch the server on and off and automatically restart the server through the web panel
- With a web console, you can view background information and execute commands in real time
- Support viewing all players' online and offline information, including but not limited to all details of backpacks, belts, equipment or skills
- Supports checking IP, unbanning and banning players, etc.
- Supports teleportation function, supports custom teleportation commands for TP and sethome, etc.
- Has a store and points system，
- Provide many APIs for everyone to use 
- Support automatic backup archive。
- Support server item distribution and real-time removal of player items
- There are more features for you to explore.

# Getting started

## **Video Tutorial**
https://youtu.be/8VklfFHeJJ8?t=982

## **Installation Guide**

### 1. **Download the Latest Release**
- Get the latest version [here](https://github.com/1249993110/7DaysToDie-ServerKit/releases).

### 2. **Place the Release in Your Mods Folder**
- Extract the downloaded .zip archive file to your `7 Days to Die Dedicated Server\Mods` folder.
- Here are the server files it needs:
  - 0_TFP_Harmony
  - TFP_CommandExtensions
  - TFP_MapRendering
  - TFP_WebServer

### 3. **Start the server**
- Usually run startdedicated.bat
- Wait for the server process to start completely.
- Open browser access http://IP:8888
- Default username: admin
- Default password: 123456

## Configuration
- To configure your login username and password, edit the `appsettings.json` file located in `7 Days to Die Dedicated Server\LSTY_Data`.
- Restart the server to make it take effect.

## Commands
You can use the 'help' command to view all available commands.

| Command					| Explanation																				|
| ---						| ---																						|
| ty-GiveItem				| Gives a item directly to a player's inventory. Drops to the ground if inventory is full.	|
| ty-GlobalMessage			| Sends a message to all connected clients.													|
| ty-SayToPlayer				| Send a message to a single player.														|
| ty-RemovePlayerItems		| Removes a player's items from the player's inventory.										|
| ty-RemovePlayerLandClaims	| Removes a player's land claims.															|
| ty-ResetPlayerProfile		| Reset a player's profile.																	|
| ty-RestartServer			| Restart server, optional parameter -f/force.												|

## API Documentation
https://docs.7dtd.top

# Links
[Official] [Official TFP 7 Days To Die Forum](https://community.7daystodie.com/topic/37613-tianyiserverkit-a-server-panel-management-tool-for-v10)  
[Official] [Nexus Mods](https://www.nexusmods.com/7daystodie/mods/5924)  
[Unofficial] [7daystodiemods.com](https://7daystodiemods.com/dedicated-server-api-integration-visual-management-kit)

## Support
Welcome join [our Discord server](<https://discord.gg/zdnmngsBK4>) and we can help.

## Contributing
We welcome and appreciate contributions. Feel free to find your own way and put up a pull request.

# Donate
[!["Buy Me a Coffee at ko-fi.com"](https://storage.ko-fi.com/cdn/kofi1.png?v=3)](https://ko-fi.com/L3L012RJ8R)

# Screenshots
![天依10 1](https://github.com/user-attachments/assets/3f2defea-4344-43f9-a18b-5428c384060c)

