# Symphony of the Night Randomizer Tools

A collection of tools to enhance the [Castlevania:Symphony of the Night Randomizer](https://sotn.io) experience. 
[![(latest) release | GitHub](https://img.shields.io/github/release/TalicZealot/SotnRandoTools.svg?logo=github&logoColor=333333&style=popout)](https://github.com/TalicZealot/SotnRandoTools/releases/latest)

This tool and the accompanying library and app are open source. The idea is to implement these features with the perspective of SotN players and provide the source for other developers to learn from and contribute just like the randomizer itself.

## Associated Projects
* [SotnApi](https://github.com/TalicZealot/SotnApi)
* [SimpleLatestReleaseUpdater](https://github.com/TalicZealot/SimpleLatestReleaseUpdater)
* [SotN Randomizer Source](https://github.com/3snowp7im/SotN-Randomizer)

## Installation
This tool requires Bizhawk version 2.6 or higher.
Extract the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) and put the contents in the `Bizhawk/ExternalTools/` folder.
It should look like this:
```
BizHawk
└───ExternalTools
│   │   SotnRandoTools.dll
│   │
│   └───SotnRandoTools
│       │   SotnApi.dll
│       │   ...
```

## Usage
After launching the game in BizHawk open through ```Tools > Extarnal Tool > Symphony of the Night Randomizer Tools```
Set your preferences and open the tool you want to use. You can then minimize the main tools window, but don't close it.
Every tool's window possition and the Tracker's size are all saved and will open where you last left them.
If the Extarnal Tool says that the game is not supported for the tool and BizHawk is displaying a question mark in the lower left corner your rom is either not recognized or you have to make sure the cue file is pointing to the correct files. I recommend creating a separate folder for Randomizer where you copy both tracks and the cue and replace track1 every time you randomize.

## Updating
On lunching the tool it will check for a new release and inform the user. If there is a newer release the update button apepars. Clicking it shuts down BizHawk and updates the tool. If it displays "Installation failed" please run the updater manually by going to ```BizHawk\ExternalTools\SotnRandoTools\Updater\SimpleLatestReleaseUpdater.exe``` or get the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) from GitHub and update manually.

## Autotracker
The new tracker has been re-written from the ground up for better performance and usability. Can be manually rescaled and supports different rendering modes. Saves size and location. Locations are drawn on the game map iself instead of relying on BizHawk GUI. It doesn't rely on the PSX display mode anymore and automatically detects everything it needs.

## Co-Op
Currently coop requires the host to have the port they want to use forwarded. Hosting automatically copies your address(ip:port) to the clipboard. The other player uses that address to connect. Please be careful to not leak your IP!
The Select button is used to send the currently highlighted item in the inventory or relic in the relic menu.
The Circle button is used to perform an assist action. Using a potion in your inventory and activating it for the other player.

## Khaos
Khaos is in an alpha stage at the moment. I am still working on implementing the in-game effects. If you want to play around with it you can use it through the manual control panel, where you can set it to execute or queue. Alternatively you can set up a command for StreamlabsChatbot with the provided template file at: 
[https://github.com/TalicZealot/SotnRandoTools/blob/main/BotCommands/khaos-action.abcom](https://github.com/TalicZealot/SotnRandoTools/blob/main/BotCommands/khaos-action.abcom) and set the actions file in the Khaos settings panel. 
You can also try using ```$savetofile(\"D:\\actions.txt\",\"$msg\")``` in the response section of the bot notifications for more automation. (I have not tested this yet)

## Useful links
* [SotN Randomizer](https://sotn.io)
* [Latest BizHawk release](https://github.com/TASVideos/BizHawk/releases/latest)
