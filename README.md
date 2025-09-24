# [Hollow Knight: Silksong] Cheat Pack 4

## Building and installing

1. Install [Hollow Knight: Silksong](https://store.steampowered.com/app/1030300/Hollow_Knight_Silksong/) (tested on Steam version only, but should work on others)
2. Install Hollow Knight: Silksong's version of BepInEx: [BepInEx 5 with Configuration Manager](https://www.nexusmods.com/hollowknightsilksong/mods/26)
3. Clone this repository
4. Verify that game installation path corresponds to path listed in `install-references.ps1` script. Modify `install-references.ps1` script if not.
5. Run `install-references.ps1` powershell script (`powershell -File install-references.ps1` from repository root)
6. Open Visual Studio (tested in VS 2022 CE, but should work in other versions as well) and open `Cheat Pack 4.sln` with "Open a project or solution"
7. Switch configuration to Release by opening Build → Configuration Manager and choosing "Release" in "Active solution configuration"
8. Leave "Active solution platform" as "Any CPU"
9. Build with Build → Build Solution or by pressing `Ctrl+Shift+B`
10. Grab your plugin from `Cheat Pack 4/bin/Release/Cheat Pack 4.dll`
11. Put this plugin in `<your Silksong installation dir>/BepInEx/plugins/Cheat Pack 4/` directory. If `Cheat Pack 4` directory doesn't exist in `plugins`, create it
12. Run the game and use `F1` to open configuration manager. All mod settings should be available here.
