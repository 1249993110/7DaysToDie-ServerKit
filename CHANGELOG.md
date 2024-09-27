## 10.43 (2024-09-27)
### New Features
- The console page supports pause refresh, automatic command completion, and echoing historical commands using the up and down keys
- Added 5 new console commands: `ty-CheckBlockType` (check the block type under the feet), `ty-DuplicateArea` (duplicate area), `ty-ExportPrefab` (export prefab), `ty-FillBlock` (fill area with blocks), `ty-PlaceBlock` (place a single block), for detailed instructions, please use `help {command}` to view
- Add the last time the owner was online under the land claim information on the GPS map

### Bug Fixes
- Fix a sporadic bug that stack overflow when gameplay moved from night to day (4:00 AM in the game)
- Fixed the bug that the Mod item name contains special characters such as spaces and cannot be sent to player backpack

### Notable Changes
- The `ty-RestartServer` command supports delay parameters


## 10.42 (2024-09-22)
### New Features
- Added commands for placing prefabs, undo prefab, and set undo history size, enter `help ty-PlacePrefab` in the console to view details
- Added a simple web ui for placing prefabs
- Add the last time the owner was online under the land claim information on the GPS map

### Notable Changes
- Update GameIcon font size


## 10.41 (2024-09-13)
### New Features
- Added color chat function, supports customizing player name color and text color, supports using variable `{PlayerName}` to customize title
- Added automatic zombie cleanup function
- Supports customizing global chat server name and whisper chat server name

### Bug Fixes
- Fixed unexpected errors in game chat page


## 10.40 (2024-09-09)
### Bug Fixes
- Fixed spanish translation issues.
- Fixed the display issues of the automatic backup record page.
- Fixed the issue that sending commands from the console page may cause the game to crash.
- Fixed the issue that deleting player archives may cause unexpected situations.
### Notable Changes
- If the configuration file directory `LSTY_Data` is not writable, fall back to the path `Mods/TianYiSdtdServerKit/Config/appsettings.json`
- The database directory and automatic backup directory support relative and absolute paths.


## 10.39 (2024-09-03)
### Notable Changes
- Players can teleport to each other by agreeing or rejecting without adding friends. You may need to reset the configuration to load the default value.


## 10.38 (2024-09-01)
### Bug Fixes
- Fixed the problem that items given by ty-give command do not support Mod and Cosmetic.
- Fixed the problem that ItemBlockSelector cannot search for hidden development items and localized name display issues.
### Notable Changes
- Remove the maximum quality limit for given items and change the default quality to 1.


## 10.37 (2024-08-31)
### New Features
- Allow users to manually switch languages
