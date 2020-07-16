# Outward-MapPlayerMarker-Mod
This is a simple map marker mod for Outward.
This mod shows the player position and the direction they are looking at.
It has been only tested in __Single Player__, but it *should* also work for co-op.

While making this mod, I have used some code from __https://github.com/sinaioutlander/Outward-Mods__. The mentioned repository also contains a lot of different mods.

# Installation
1. This mod (also a lot of other Outward mods) requires BepInEx. In order to install BepInEx and learn how to install mods in general, please follow the well written guide in __https://outward.gamepedia.com/Installing_Mods__.
1. Go to the Release page of this repository __https://github.com/ArmanHZ/Outward-MapPlayerMarker-Mod/releases__ and install the latest versions of _**MapPlayerMarker.dll**_ and _**custom_player_marker.png**_.
1. Go to your Outward directory by **Steam\steamapps\common\Outward** or in your steam library, right click on Outward and click on Properties, Local Files, and then Browse Local Files.
1. **REMINDER: You must be on the Mono (modding) branch. Refer to the first item on the list.**
1. Place the _**MapPlayerMarker.dll**_ file into **Outward\BepInEx\plugins** folder.
1. Place the _**custom_player_marker.png**_ file into **Outward\Mods\MapPlayerMarker** folder.
   1. It this is your first mod, the Mods folder will not exists, simply create the Mods folder and the MapPlayerMarker folder inside the Mods folder.
   1. You can change the _**custom_player_marker.png**_ as you wish.

# Known Bugs
* Sometimes when opening the map, an NPC will also get his or her marker updated and it will show as the mod's marker. In the code, I always use the Game's method **_IsAI()_** before drawing. I suspect that, this bug is caused in the Game's method. However it does not affect you, nor your gameplay in anyway.
  * If you know a fix to this, please create an Issue.

# Todo
* Add pictures of the mod.
