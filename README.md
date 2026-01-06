# Resonite Photo Metadata Logger

> [!WARNING]
> This mod is WIP and is not fully working yet.

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that exports photo metadata when a screenshot is exported.

The log (will be) kept at `~/Photos/Resonite.jsonl`

## TODO

- Add configurations
  - Make the output path not hard coded
- Figure out a way to link metadata entries to image files
- See if the code can be improved/cleaned up

## Installation

1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
1. Place [PhotoMetadataLogger.dll](https://github.com/YourGithubUsername/YourModRepoName/releases/latest/download/ExampleModName.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create this folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.

Based on the [ExampleMod Template](https://github.com/resonite-modding-group/ExampleMod)
