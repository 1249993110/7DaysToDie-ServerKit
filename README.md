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
| ty-SayToPlayer			| Sends a message to a single player.														|
| ty-RemovePlayerItems		| Removes a player's items from the player's inventory.										|
| ty-RemovePlayerLandClaims	| Removes a player's land claims.															|
| ty-ResetPlayerProfile		| Reset a player's profile.																	|
| ty-RestartServer			| Restart server, optional parameter -f/force.												|

## API Documentation
https://docs.7dtd.top

## Changelog
Detailed changes for each release are documented in the [release notes](./CHANGELOG.md)

## Links
[Official] [Official TFP 7 Days To Die Forum](https://community.7daystodie.com/topic/37613-tianyiserverkit-a-server-panel-management-tool-for-v10)  
[Official] [Nexus Mods](https://www.nexusmods.com/7daystodie/mods/5924)  
[Unofficial] [7daystodiemods.com](https://7daystodiemods.com/dedicated-server-api-integration-visual-management-kit)

## Support
Welcome join [our Discord server](<https://discord.gg/zdnmngsBK4>) and we can help.

## Contributing
We welcome and appreciate contributions. Feel free to find your own way and put up a pull request.

## Donate
[!["Buy Me a Coffee at ko-fi.com"](https://storage.ko-fi.com/cdn/kofi1.png?v=3)](https://ko-fi.com/L3L012RJ8R)

## Screenshots
![image](https://github.com/user-attachments/assets/581cd03d-e271-4011-b547-b82ad16f64a3)
<table>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/fa9e18a5-65d1-46a1-bd3f-3d136bf4411c">
    </td>
    <td>
      <img src="https://github.com/user-attachments/assets/748b33cb-bfbc-4585-848f-0ac07a192121">
    </td>
  </tr>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/84661343-8a20-414b-90be-a27705555259">
    </td>
    <td>
      <img src="https://github.com/user-attachments/assets/92c87d6d-8406-415b-9d13-a07a18ddb087">
    </td>
  </tr>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/c068c9bf-0ebb-4b3d-a3c9-e01c957cfca4">
    </td>
    <td>
      <img src="https://github.com/user-attachments/assets/dfdb1dfb-f801-463b-a8d1-dc64ab88736e">
    </td>
  </tr>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/52a5d65a-0c9e-4812-baad-f6fd0c84ef95">
    </td>
    <td>
      <img src="https://github.com/user-attachments/assets/4d8317ab-388b-4191-9d31-df778d93f7a6">
    </td>
  </tr>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/193760ae-9b66-4c81-82e9-a31860130f4d">
    </td>
    <td>
      <img src="https://github.com/user-attachments/assets/5b3d551b-8d77-4c0d-bd93-83b74316fff6">
    </td>
  </tr>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/3c5e6605-3c16-45ed-a0ee-a5327e7a3056">
    </td>
    <td>
      <img src="https://github.com/user-attachments/assets/0fb8610f-8a6e-4005-b2ed-5043aa066b99">
    </td>
  </tr>
</table>

