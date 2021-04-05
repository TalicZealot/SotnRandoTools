# SotN Rando Tools

A collection of tools to enhance the `Castlevania:Symphony of the Night Randomizer` experience. 

This tool and the accompanying library and app are open source. The idea is to implement these features with the perspective of SotN players and provide the source for other developers to learn from and contribute just like the randomizer itself.

## Installation
This tool requires Bizhawk version 2.6 or higher.
Extract the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) and put the contents in the `Bizhawk/ExternalTools/` folder.
It should look like this:
```
BizHawk
└───ExtternalTools
│   │   SotnRandoTools.dll
│   │
│   └───SotnRandoTools
│       │   SotnApi.dll
│       │   ...
```

## Autotracker
The new tracker has been re-written from the ground up for better performance and usability. Can be manually rescaled and supports different rendering modes. Saves size and location. Locations are drawn on the game map iself instead of relying on BizHawk GUI. It doesn't rely on the PSX display mode anymore and automatically detects everything it needs.

## Co-Op
Currently coop requires the host to have the port they want to use forwarded.
Hosting automatically copies your address(ip:port) to the clipboard. The other player uses that address to connect.
Enable BizHawk messages through `View > Display Messages`.
Open the BizHawk console through `View > Open Log Window` for detailed information.
The Select button is used to send the currently highlighted item in the inventory or relic in the relic menu.
The Circle button is used to perform an assist action. Using a potion in your inventory and activating it for the other player.

## Khaos
Khaos is in an alpha stage at the moment. I am still working on implementing the in-game effects. If you want to play around with it you can use it through the manual control panel, where you can set it to execute or queue. Alternatively you can set up a command for StreamlabsChatbot with the provided template file at: 
[https://github.com/TalicZealot/SotnRandoTools/blob/main/BotCommands/khaos-action.abcom](https://github.com/TalicZealot/SotnRandoTools/blob/main/BotCommands/khaos-action.abcom) and set the actions file in the Khaos settings panel. 
You can also try using ```$savetofile(\"D:\\actions.txt\",\"$msg\")``` in the response section of the bot notifications for more automation. (I have not tested this yet)

## Useful links
* [SotN Randomizer](https://sotn.io)
* [Latest BizHawk release](https://github.com/TASVideos/BizHawk/releases/latest)
