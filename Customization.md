## Custom presets
Put custom presets in `ExternalTools\SotnRandoTools\Presets`.
Custom allowed locations can be placed in an array named `lockLocationAllowed` in the same format as the standard `lockLocation` array.
Customize which relics the tracker considers progression by adding the `progressionRelics` array.
```
"progressionRelics": [
      "Demon Card", "Soul of Bat", "Spirit Orb"
    ]
```

## Using custom location extensions
extends: Include if your extension adds custom locations on top of an existing one.
Custom locations:
 location: Name of the location. If this location is already tracked there is no need for further data.
 x,y: Represent coordinates on the map of the top left pixel of the room.
 rooms: Rooms from which the location can be checked or peaked. Each room has an address in ram and corresponding values that represent flags of the room being checked. 
Example:
```
{
  "extends": "equipment",
  "customLocations": [{
    "location": "Confessional",
    "x": 65,
    "y": 66,
    "secondCastle": false,
    "rooms": [
        {"address": "0x06BCC8", "values": ["0x40"]}
    ]
  }, {
    "location": "Colosseum Green tea",
    "x": 77,
    "y": 78,
    "secondCastle": false,
    "rooms": [
        {"address": "0x06BCF8", "values": ["0x01"]}
    ]
  }, {
    "location": "Bekatowa"
  }]
}
```