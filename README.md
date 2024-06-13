# Symphony of the Night Randomizer Tools

[![(latest) release | GitHub](https://img.shields.io/github/release/TalicZealot/SotnRandoTools.svg?logo=github&logoColor=333333&style=popout)](https://github.com/TalicZealot/SotnRandoTools/releases/latest)

A collection of tools to enhance the [Castlevania:Symphony of the Night Randomizer](https://sotn.io) experience.

This tool and the accompanying library and app are open source. The idea is to implement these features with the perspective of SotN players and provide the source for other developers to learn from and contribute just like the randomizer itself.

## Associated Projects
* [SotnApi](https://github.com/TalicZealot/SotnApi)
* [SimpleLatestReleaseUpdater](https://github.com/TalicZealot/SimpleLatestReleaseUpdater)
* [SotN Randomizer Source](https://github.com/3snowp7im/SotN-Randomizer)
* [SotN Decomp](https://github.com/Xeeynamo/sotn-decomp)

## Table of Contents

- [Symphony of the Night Randomizer Tools](#symphony-of-the-night-randomizer-tools)
  - [Table of Contents](#table-of-contents)
  - [Installation](#installation)
  - [Installation Linux](#installation-linux)
  - [Usage](#usage)
  - [Updating](#updating)
  - [Autotracker](#autotracker)
  - [Autosplitter](#autosplitter)
  - [Co-Op](#co-op)
  - [Khaos-Setup](#Khaos-Setup)
  - [Useful links](#useful-links)
  - [Contributors](#contributors)
  - [Special Thanks](#special-thanks)

## Installation
This tool requires [the latest BizHawk version](https://github.com/TASEmulators/BizHawk/releases/latest).
Download the full version from the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) that looks like this `SotnRandoTools-x.x.x.zip`
Right click on it and select `Extract all...` then navigate to your BizHawkfolder and press `Extract`.
File structure should look like this:
```
BizHawk
└───ExternalTools
│   │   SotnRandoTools.dll
│   │
│   └───SotnRandoTools
│   │     └───dll
│   │     └───Images
│   │     └───...
```

## Installation Linux
When playing on Linux get SotnRandoToolsLinux-x.x.x.zip and install as described above, but after that copy the contents of `BizHawk/ExternalTools/SotnRandoTools/dll/` to `BizHawk/ExternalTools/` and `BizHawk/dll/`.

## Usage
After launching the game in BizHawk open through ```Tools > Extarnal Tool > Symphony of the Night Randomizer Tools```
Set your preferences and open the tool you want to use. You can then minimize the main tools window, but don't close it.
Every tool's window possition and the Tracker's size are all saved and will open where you last left them.
If the Extarnal Tool says that the game is not supported for the tool and BizHawk is displaying a question mark in the lower left corner your rom is either not recognized or you have to make sure the cue file is pointing to the correct files. I recommend creating a separate folder for Randomizer where you copy both tracks and the cue and replace track1 every time you randomize.

## Updating
On lunching the tool it will check for a new release and inform the user. If there is a newer release the update button apepars. Clicking it shuts down BizHawk and updates the tool. If it displays "Installation failed" please run the updater manually by going to ```BizHawk\ExternalTools\SotnRandoTools\Updater\SimpleLatestReleaseUpdater.exe``` or get the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) from GitHub and update manually. If you get an error notifying you that your system lacks the necessary .NET version to run the updater click [the link](https://dotnet.microsoft.com/download/dotnet/5.0/runtime?utm_source=getdotnetcore&utm_medium=referral) and download the x64 and x86 redistributable packages for desktop apps.

## Autotracker
The new tracker has been re-written from the ground up for better performance and usability. Can be manually rescaled. Saves size and location. Locations are drawn on the game map.
Supports OBS html widget overlay located in ```BizHawk\ExternalTools\SotnRandoTools\TrackerOverlay\autotracker.html``` and seed info text in  ```BizHawk\ExternalTools\SotnRandoTools\TrackerOverlay\SeedInfo.txt```

## Autosplitter
The autotracker will also automatically start, restart and split when Dracula dies if you have LiveSplit running.

## Co-Op
Coop requires the host to have the port they want to use forwarded. Hosting automatically copies your address(ip:port) to the clipboard. The other player uses that address to connect. Please be careful to not leak your IP!
Bindings over at: [https://taliczealot.github.io/coop/](https://taliczealot.github.io/coop/)

## Khaos-Setup
Khaos has moved to [https://github.com/TalicZealot/SotnKhaosTools](https://github.com/TalicZealot/SotnKhaosTools)

## Useful links
* [SotN Randomizer](https://sotn.io)
* [Latest BizHawk release](https://github.com/TASVideos/BizHawk/releases/latest)

## Contributors
* [fatihG](https://twitter.com/fatihG_) - Familiar card icons, replay system concept.

## Special Thanks
* Wild Mouse - the goat
* SotN Rando Community
* SotN Decomp Researchers
* Everybody who donated during the development of the project