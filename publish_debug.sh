#!/bin/bash

DLL=JotunnModStub/bin/Debug/net48/HuntersArsenal.dll
PLUGINS="${R2MODMAN_PROFILE:-/home/$USER/.config/r2modmanPlus-local/Valheim/profiles/Mod Test}/BepInEx/plugins/HuntersArsenal"

# Check that source files exist and are readable
if [ ! -f "$DLL" ]; then
    echo "Error: $DLL does not exist or is not readable."
    exit 1
fi

# Check that target directory exists and is writable
mkdir -p "$PLUGINS"

if [ ! -w "$PLUGINS" ]; then
    echo "Error: $PLUGINS directory is not writable."
    exit 1
fi

cp -f "$DLL" "$PLUGINS" || { echo "Error: Failed to copy $DLL"; exit 1; }
