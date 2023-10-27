#!/bin/bash
if [ $# -eq 0 ]; then
    >&2 echo "Usage: create-release version [-l linux]"
    exit 1
fi

mkdir NewRelease
cp -r BizHawk/ExternalTools NewRelease
rm NewRelease/ExternalTools/SotnRandoTools/ToolConfig.ini
rm -r NewRelease/ExternalTools/SotnRandoTools/Replays/*
cd NewRelease
if [ "$2" == "-l" ]; then
    rm -r ExternalTools/SotnRandoTools/Updater
    7z a -tzip $HOME/Desktop/SotnRandoTools-Linux-$1.zip ExternalTools
    cd ..
else
    7z a -tzip $HOME/Desktop/SotnRandoTools-$1.zip ExternalTools
    rm -r ExternalTools/SotnRandoTools/Updater
    cd ExternalTools
    7z a -tzip $HOME/Desktop/Update-$1.zip SotnRandoTools/*
    7z a -tzip $HOME/Desktop/Update-$1.zip SotnRandoTools.dll
    cd ../..
fi
rm -r NewRelease