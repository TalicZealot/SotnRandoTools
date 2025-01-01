#!/bin/bash
if [ $# -eq 0 ]; then
    >&2 echo "Usage: create-release version"
    exit 1
fi

mkdir NewRelease
cp -r BizHawk/ExternalTools NewRelease
rm NewRelease/ExternalTools/SotnRandoTools/ToolConfig.ini
rm -r NewRelease/ExternalTools/SotnRandoTools/Replays/*
cd NewRelease
7z a -tzip $HOME/Desktop/SotnRandoTools-$1.zip ExternalTools
rm -r ExternalTools/SotnRandoTools/Updater
cd ExternalTools
7z a -tzip $HOME/Desktop/Update-$1.zip SotnRandoTools/*
7z a -tzip $HOME/Desktop/Update-$1.zip SotnRandoTools.dll
cd ../..
rm -r NewRelease