# Ascension Adjuster

**Slay the Spire 2** Mod &nbsp;|&nbsp; [中文](README.md)

Skip the grind — set any character's Ascension level freely, from 0 to 10, without having to unlock each level by beating the game.

---

## Contents

- [Features](#features)
- [Installation](#installation)
- [Activation](#activation)
- [Configuration](#configuration)
- [Multiplayer](#multiplayer)
- [Ascension Levels Reference](#ascension-levels-reference)
- [How It Works](#how-it-works)
- [Compatibility](#compatibility)
- [License](#license)

---

## Features

- Instantly unlock any character to any Ascension level (default: all characters unlocked to level 10)
- Per-character overrides — set different caps for each character independently
- Works on characters that have never been played before
- Optional multiplayer Ascension override (disabled by default, opt-in via config)
- Non-destructive — no save files are modified; disabling the mod restores original progress
- Does not interfere with natural unlocking — beating the game still unlocks the next level as normal

---

## Installation

### Option 1: Auto Install (Recommended)

1. Download the latest zip from the [Releases](../../releases) page
2. Extract it anywhere
3. Double-click `install.bat`
4. The script automatically detects your Steam library and copies the mod files

### Option 2: Manual Install

1. Download `AscensionAdjuster.dll` and `AscensionAdjuster.json`
2. Create the folder `mods\AscensionAdjuster\` inside your game directory
3. Copy both files into that folder

```
[Game directory]\mods\AscensionAdjuster\
e.g. D:\Steam\steamapps\common\Slay the Spire 2\mods\AscensionAdjuster\
```

> Can't find your game folder? In Steam: right-click the game → Manage → Browse local files.

### Option 3: Build from Source

Requires [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

```bash
git clone https://github.com/VariianWrynn/STS2-Acension-level-unlock.git
cd STS2-Acension-level-unlock
dotnet build
```

After building, the DLL and JSON are copied to your game's `mods` directory automatically.

> Note: update the `Sts2Dir` path in `AscensionAdjuster.csproj` to point to your game install directory.

---

## Activation

1. Launch Slay the Spire 2
2. Click **Mods** on the main menu
3. Enable **Ascension Adjuster**
4. Restart the game

Once active, the Ascension selector in the character select screen will be available for all characters (0–10).

---

## Configuration

`config.json` is generated automatically on first run. Open it with any text editor.

```
[Game directory]\mods\AscensionAdjuster\config.json
```

### Default Config

```json
{
  "enabled": true,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

All characters are unlocked to Ascension 10 in singleplayer. Multiplayer is not overridden by default.

### Config Reference

| Key | Type | Description |
|---|---|---|
| `enabled` | `true` / `false` | Master on/off switch. Set to `false` to temporarily disable without uninstalling |
| `global_ascension_override` | `0`–`10` or `-1` | Singleplayer cap applied to all characters. `-1` = don't override, use game's natural progress |
| `character_overrides` | Object | Per-character caps; takes priority over the global setting |
| `multiplayer_ascension_override` | `0`–`10` or `-1` | Multiplayer cap. `-1` = don't override (default) |

### Per-Character Overrides

Add character IDs and their desired caps under `character_overrides`:

```json
{
  "enabled": true,
  "global_ascension_override": -1,
  "character_overrides": {
    "CHARACTER.IRONCLAD":    10,
    "CHARACTER.SILENT":       5,
    "CHARACTER.DEFECT":      10,
    "CHARACTER.REGENT":       3,
    "CHARACTER.NECROBINDER":  7
  },
  "multiplayer_ascension_override": -1
}
```

### Character ID Reference

| Character | ID |
|---|---|
| Ironclad | `CHARACTER.IRONCLAD` |
| Silent | `CHARACTER.SILENT` |
| Defect | `CHARACTER.DEFECT` |
| Regent | `CHARACTER.REGENT` |
| Necrobinder | `CHARACTER.NECROBINDER` |

### Quick Examples

**Unlock only one character, leave others at natural progress:**
```json
{
  "enabled": true,
  "global_ascension_override": -1,
  "character_overrides": { "CHARACTER.IRONCLAD": 10 },
  "multiplayer_ascension_override": -1
}
```

**Unlock everyone to level 5:**
```json
{
  "enabled": true,
  "global_ascension_override": 5,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

**Temporarily disable the mod (keeps your config):**
```json
{
  "enabled": false,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

---

## Multiplayer

### Singleplayer vs Multiplayer Ascension

STS2's Ascension system is entirely separate between singleplayer and multiplayer:

| | Singleplayer | Multiplayer |
|---|---|---|
| Storage | Per character | Single global value |
| Cap source | Each character's own unlock progress | Lowest unlock level among all players in the lobby |
| Who controls it | The player | Host only |

### Enabling the Multiplayer Override

Set `multiplayer_ascension_override` to your desired level (0–10):

```json
{
  "enabled": true,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": 10
}
```

### Important Notes

- STS2 uses **P2P networking** — there is no central server. The lobby's Ascension cap is always the lowest unlock level among all connected players.
- For the override to fully take effect, **all players in the session must have this mod installed** and set the same `multiplayer_ascension_override` value.
- A player without the mod will still cap the entire lobby at their natural unlock level.
- This setting defaults to `-1` (disabled) and has no effect on players who haven't configured it.

---

## Ascension Levels Reference

| Level | Name | Effect |
|:---:|---|---|
| 1 | Elite Surge | Elites appear more frequently |
| 2 | Weary Traveler | Ancient Relics restore only 80% of missing HP |
| 3 | Impoverished | 25% less gold from enemies and chests |
| 4 | Frugal | Start with 1 fewer potion slot |
| 5 | Cursed Climb | Start each run with a Curse card |
| 6 | Dim Hearths | Fewer campfire rest sites |
| 7 | Scarce Supplies | Rare and upgraded cards appear less often |
| 8 | Hardened Foes | All enemies are tougher to kill |
| 9 | Lethal Blows | All enemies deal more damage |
| 10 | Double Trouble | Face two Bosses simultaneously at the end of Act 3 |

---

## How It Works

This mod uses [Harmony](https://github.com/pardeike/Harmony) runtime patching. All five patches are in-memory only — no save files or game files are ever modified.

**Singleplayer (3 patches):**
1. `CharacterStats.MaxAscension` getter — replaces the max Ascension level with the configured value
2. `CharacterStats.PreferredAscension` getter — syncs the default selected level to the configured value
3. `StartRunLobby.IsAscensionEpochRevealed` — bypasses the "must clear Act 3 first" gate so characters that have never been played also show the Ascension selector

**Multiplayer (2 patches):**
4. `ProgressState.MaxMultiplayerAscension` getter — overrides the multiplayer max Ascension (this value is also reported to the host when joining a lobby)
5. `ProgressState.PreferredMultiplayerAscension` getter — syncs the default multiplayer selected level to the configured value

---

## Project Structure

```
STS2-Acension-level-unlock/
├── scripts/
│   ├── Entry.cs              # Mod entry point, registers Harmony patches
│   ├── Patches.cs            # Five Harmony patch implementations
│   └── AscensionConfig.cs    # Config file read/write logic
├── AscensionAdjuster.csproj  # C# project file
├── AscensionAdjuster.json    # Mod manifest (read by the game)
├── install.bat               # Installer launcher (pure ASCII)
├── install.ps1               # Installer script (with UI)
├── project.godot             # Godot project config
└── export_presets.cfg        # Godot export config
```

---

## Compatibility

- Game version: Slay the Spire 2 Early Access (Godot 4.5 / .NET 9)
- Platform: Windows

### Compatibility with RemoveMultiplayerPlayerLimit

Fully compatible with RemoveMultiplayerPlayerLimit (raises the lobby player cap from 4 to more). There are no overlapping Harmony patches between the two mods.

When both mods are active:
- Singleplayer Ascension override works normally, unaffected
- The multiplayer Ascension cap is still determined by the lowest unlock level in the lobby
- With more players, it becomes increasingly likely someone will pull the cap down — it is recommended that all players install AscensionAdjuster and set the same `multiplayer_ascension_override`

---

## License

[MIT License](LICENSE) — free to use, modify, and distribute.
