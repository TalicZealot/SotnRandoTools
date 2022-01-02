# Symphony of the Night Randomizer Tools

[![(latest) release | GitHub](https://img.shields.io/github/release/TalicZealot/SotnRandoTools.svg?logo=github&logoColor=333333&style=popout)](https://github.com/TalicZealot/SotnRandoTools/releases/latest)

A collection of tools to enhance the [Castlevania:Symphony of the Night Randomizer](https://sotn.io) experience.

This tool and the accompanying library and app are open source. The idea is to implement these features with the perspective of SotN players and provide the source for other developers to learn from and contribute just like the randomizer itself.

## Associated Projects
* [SotnApi](https://github.com/TalicZealot/SotnApi)
* [SimpleLatestReleaseUpdater](https://github.com/TalicZealot/SimpleLatestReleaseUpdater)
* [SotN Randomizer Source](https://github.com/3snowp7im/SotN-Randomizer)

## Table of Contents

- [Symphony of the Night Randomizer Tools](#symphony-of-the-night-randomizer-tools)
  - [Table of Contents](#table-of-contents)
  - [Installation](#installation)
  - [Usage](#usage)
  - [Updating](#updating)
  - [Autotracker](#autotracker)
  - [Co-Op](#co-op)
  - [Khaos-Setup](#Khaos-Setup)
  - [Useful links](#useful-links)
  - [Contributors](#contributors)
  - [Special Thanks](#special-thanks)

## Installation
This tool requires Bizhawk version 2.7 or higher.
Download the full version from the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) that looks like this `SotnRandoTools-x.x.x.zip`
Right click on it and select `Extract all...` then navigate to your BizHawk 2.6+ folder and press `Extract`.
File structure should look like this:
```
BizHawk
└───ExternalTools
│   │   SotnRandoTools.dll
│   │
│   └───SotnRandoTools
│   │     │   SotnApi.dll
│   │     │   ...
│   └───TwitchLib.Api
│   └───TwitchLib.Api.Helix.Models
```

## Usage
After launching the game in BizHawk open through ```Tools > Extarnal Tool > Symphony of the Night Randomizer Tools```
Set your preferences and open the tool you want to use. You can then minimize the main tools window, but don't close it.
Every tool's window possition and the Tracker's size are all saved and will open where you last left them.
If the Extarnal Tool says that the game is not supported for the tool and BizHawk is displaying a question mark in the lower left corner your rom is either not recognized or you have to make sure the cue file is pointing to the correct files. I recommend creating a separate folder for Randomizer where you copy both tracks and the cue and replace track1 every time you randomize.

## Updating
On lunching the tool it will check for a new release and inform the user. If there is a newer release the update button apepars. Clicking it shuts down BizHawk and updates the tool. If it displays "Installation failed" please run the updater manually by going to ```BizHawk\ExternalTools\SotnRandoTools\Updater\SimpleLatestReleaseUpdater.exe``` or get the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) from GitHub and update manually. If you get an error notifying you that your system lacks the necessary .NET version to run the updater click [the link](https://dotnet.microsoft.com/download/dotnet/5.0/runtime?utm_source=getdotnetcore&utm_medium=referral) and download the x64 and x86 redistributable packages for desktop apps.

## Autotracker
The new tracker has been re-written from the ground up for better performance and usability. Can be manually rescaled. Saves size and location. Locations are drawn on the game map iself instead of relying on BizHawk GUI. It doesn't rely on the PSX display mode anymore and automatically detects everything it needs.

## Co-Op
Coop requires the host to have the port they want to use forwarded. Hosting automatically copies your address(ip:port) to the clipboard. The other player uses that address to connect. Please be careful to not leak your IP!
Bindings over at: [https://taliczealot.github.io/coop/](https://taliczealot.github.io/coop/)

## Khaos-Setup
Inside the folder ```BizHawk\ExternalTools\SotnRandoTools\Khaos\Overlay\``` you will find ```action-queue.html``` and ```timers.html``` which you can add in OBS as web sources.
After starting Khaos you will be able to connect to Twitch and start Auto Khaos.
* Clickign "Connect to Twitch" will take you to a website to confirm that you allow SotN Rando Tools to manage Channel Points and see your subscribers. After accepting it will create custom Channel Point rewards for every action and listen for redemptions. Whne you disconnect or close Khaos the Custom Rewards should get deleted. For now the rewards get automatically set to fulfilled, in the future I may opt to leave them in case the streamer wants to refund some things.
* Auto Khaos automatically activates random actions periodically, adhering to the cooldowns.

## Useful links
* [SotN Randomizer](https://sotn.io)
* [Latest BizHawk release](https://github.com/TASVideos/BizHawk/releases/latest)

## Contributors
* [3snowp7im](https://github.com/3snowp7im) - SotN Randomizer developer
* [fatihG](https://twitter.com/fatihG_) - Familiar card icons, replay system concept.

## Special Thanks
* asdheyb
* fatihG
* EmilyNyx
* DinnerDog
* Gods666thChild
* LordalexZader
* ziggypigster