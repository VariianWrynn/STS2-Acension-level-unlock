# Ascension Adjuster — 天梯难度调整器

**杀戮尖塔 2** 模组 &nbsp;|&nbsp; Slay the Spire 2 Mod

无需逐级通关解锁，直接为任意角色自由设定天梯难度等级（0–10 级）。

*Skip the grind — set any character's Ascension level freely, from 0 to 10.*

---

## 目录 / Contents

- [功能特性 / Features](#功能特性--features)
- [安装方法 / Installation](#安装方法--installation)
- [激活模组 / Activation](#激活模组--activation)
- [配置说明 / Configuration](#配置说明--configuration)
- [多人联机 / Multiplayer](#多人联机--multiplayer)
- [天梯等级一览 / Ascension Levels](#天梯等级一览--ascension-levels)
- [技术原理 / How It Works](#技术原理--how-it-works)
- [兼容性 / Compatibility](#兼容性--compatibility)
- [许可协议 / License](#许可协议--license)

---

## 功能特性 / Features

- 全角色天梯等级一键解锁至任意级别（默认全部解锁至 10 级）
- 支持为每个角色单独设定不同的天梯上限
- 对从未上过天梯、甚至从未使用过的角色同样有效
- 支持多人联机天梯等级覆盖（需单独配置，默认关闭）
- 不修改任何存档文件，禁用模组后游戏恢复原始进度
- 不影响自然解锁流程：正常通关仍会正常解锁下一级

> Unlock all characters to any Ascension level instantly (default: 10). Per-character overrides supported. Works on characters never played before. Optional multiplayer override. Non-destructive — disabling the mod restores original progress.

---

## 安装方法 / Installation

### 方法一：自动安装（推荐） / Auto Install (Recommended)

1. 前往 [Releases](../../releases) 页面下载最新版本的压缩包
2. 解压到任意位置
3. 双击运行 `install.bat`
4. 脚本会自动检测 Steam 安装位置并复制模组文件

> 1. Download the latest zip from the [Releases](../../releases) page.
> 2. Extract anywhere.
> 3. Double-click `install.bat`.
> 4. The script auto-detects your Steam library and copies the mod files.

### 方法二：手动安装 / Manual Install

1. 下载 `AscensionAdjuster.dll` 和 `AscensionAdjuster.json`
2. 在游戏目录下新建文件夹 `mods\AscensionAdjuster\`
3. 将上述两个文件复制到该文件夹中

```
[游戏目录]\mods\AscensionAdjuster\
例如: D:\Steam\steamapps\common\Slay the Spire 2\mods\AscensionAdjuster\
```

> Download `AscensionAdjuster.dll` and `AscensionAdjuster.json`, create the folder `mods\AscensionAdjuster\` inside your game directory, and copy both files there.
>
> *Can't find your game folder? In Steam: right-click the game → Manage → Browse local files.*

### 方法三：从源码构建 / Build from Source

需要 [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)。 / Requires [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

```bash
git clone https://github.com/VariianWrynn/STS2-Acension-level-unlock.git
cd STS2-Acension-level-unlock
dotnet build
```

构建完成后，DLL 和 JSON 会自动复制到游戏的 `mods` 目录。注意：`AscensionAdjuster.csproj` 中的 `Sts2Dir` 路径需指向你的游戏安装目录。

> After building, the DLL and JSON are copied to your game's `mods` directory automatically. Update the `Sts2Dir` path in `AscensionAdjuster.csproj` to match your game install location.

---

## 激活模组 / Activation

1. 启动「杀戮尖塔 2」
2. 在主菜单点击 **Mods（模组）** 按钮
3. 勾选 **Ascension Adjuster**
4. 重启游戏

激活后，在角色选择界面即可看到天梯等级选择器，所有角色均可选择 0–10 级。

> Launch the game → click **Mods** on the main menu → enable **Ascension Adjuster** → restart. The Ascension selector will be available for all characters (0–10).

---

## 配置说明 / Configuration

首次运行游戏后，模组目录中会自动生成 `config.json`，用记事本打开即可编辑。

*`config.json` is auto-generated on first run. Edit it with any text editor.*

```
[游戏目录]\mods\AscensionAdjuster\config.json
```

### 默认配置 / Default Config

```json
{
  "enabled": true,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

### 配置项说明 / Config Reference

| 配置项 / Key | 类型 / Type | 说明 / Description |
|---|---|---|
| `enabled` | `true` / `false` | 模组总开关 / Master switch |
| `global_ascension_override` | `0`–`10` 或 `-1` | 单人全局天梯上限；`-1` 表示不干预 / Singleplayer cap for all characters; `-1` = don't override |
| `character_overrides` | 对象 / Object | 角色单独上限，优先级高于全局 / Per-character caps, takes priority over global |
| `multiplayer_ascension_override` | `0`–`10` 或 `-1` | 多人联机上限；默认 `-1` 不干预 / Multiplayer cap; default `-1` = don't override |

### 角色单独设置 / Per-Character Overrides

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

### 角色 ID 对照表 / Character ID Reference

| 角色 / Character | ID |
|---|---|
| 铁甲战士 / Ironclad | `CHARACTER.IRONCLAD` |
| 无声者 / Silent | `CHARACTER.SILENT` |
| 缺陷体 / Defect | `CHARACTER.DEFECT` |
| 摄政王 / Regent | `CHARACTER.REGENT` |
| 死灵缚者 / Necrobinder | `CHARACTER.NECROBINDER` |

### 常用配置示例 / Quick Examples

**只解锁一个角色 / Unlock only one character:**
```json
{
  "enabled": true,
  "global_ascension_override": -1,
  "character_overrides": { "CHARACTER.IRONCLAD": 10 },
  "multiplayer_ascension_override": -1
}
```

**全部解锁至第 5 级 / Unlock everyone to level 5:**
```json
{
  "enabled": true,
  "global_ascension_override": 5,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

**临时关闭模组 / Temporarily disable (keeps config):**
```json
{
  "enabled": false,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

---

## 多人联机 / Multiplayer

### 单人与多人的区别 / Singleplayer vs Multiplayer

杀戮尖塔 2 的天梯系统在单人和多人模式下完全独立。

*The Ascension system is entirely separate between singleplayer and multiplayer.*

| | 单人 / Singleplayer | 多人 / Multiplayer |
|---|---|---|
| 等级存储 / Storage | 每角色独立 / Per character | 全局共用 / Global single value |
| 上限来源 / Cap source | 角色自身进度 / Character progress | 房间内所有玩家的最低等级 / Lowest unlock among all players |
| 控制权 / Control | 玩家自由选择 / Player chooses freely | 仅房主 / Host only |

### 启用多人覆盖 / Enabling Multiplayer Override

```json
{
  "enabled": true,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": 10
}
```

### 重要须知 / Important Notes

- 游戏采用 **P2P 联机**，无中央服务器。天梯上限 = 房间内所有玩家中最低的解锁等级。
- **房间内所有玩家都需要安装本模组**，并设置相同的 `multiplayer_ascension_override` 值，覆盖才能完全生效。
- 若某位玩家未安装模组，其较低的解锁等级仍会拉低整个房间的天梯上限。
- 此设置默认关闭（`-1`），对未配置的玩家没有任何影响。

> STS2 uses **P2P networking** — there is no central server. The multiplayer Ascension cap is determined by the lowest unlock level among all players in the lobby. For the override to fully work, **all players in the session should install this mod** and set the same `multiplayer_ascension_override` value. Players without the mod will still cap the lobby at their natural unlock level.

---

## 天梯等级一览 / Ascension Levels

| 等级 / Level | 中文名 | English Name | 效果 / Effect |
|:---:|---|---|---|
| 1 | 蜂拥精英 | Elite Surge | 精英怪出现频率增加 / Elites appear more often |
| 2 | 疲惫旅人 | Weary Traveler | 遗迹仅恢复 80% 缺失生命 / Relics restore only 80% missing HP |
| 3 | 贫困潦倒 | Impoverished | 金币掉落减少 25% / 25% less gold from enemies and chests |
| 4 | 节衣缩食 | Frugal | 起始药水栏位减少 1 / Start with 1 fewer potion slot |
| 5 | 登塔者之灾 | Cursed Climb | 起始获得一张诅咒牌 / Start each run with a Curse |
| 6 | 阴霾笼罩 | Dim Hearths | 篝火休息点减少 / Fewer campfire rest sites |
| 7 | 物资匮乏 | Scarce Supplies | 稀有卡和升级卡出现率降低 / Rare and upgraded cards appear less often |
| 8 | 强敌环伺 | Hardened Foes | 所有敌人更难击杀 / All enemies are tougher |
| 9 | 致命一击 | Lethal Blows | 所有敌人攻击更致命 / All enemies hit harder |
| 10 | 双重Boss | Double Trouble | 第三幕末尾同时面对两个 Boss / Face two Bosses at the end of Act 3 |

---

## 技术原理 / How It Works

本模组使用 [Harmony](https://github.com/pardeike/Harmony) 运行时补丁，通过五个 Postfix 补丁实现。所有补丁仅在游戏运行时生效，不修改任何存档或游戏文件。

*Uses [Harmony](https://github.com/pardeike/Harmony) runtime patching. All patches are in-memory only — no save files or game files are ever modified.*

**单人 / Singleplayer (3 patches):**
1. `CharacterStats.MaxAscension` getter — 替换最大天梯等级 / Replaces the max Ascension level
2. `CharacterStats.PreferredAscension` getter — 同步默认选中等级 / Syncs the default selected level
3. `StartRunLobby.IsAscensionEpochRevealed` — 绕过首次通关前置限制，使新角色也能显示选择器 / Bypasses the "must clear Act 3 first" gate so never-played characters show the selector

**多人 / Multiplayer (2 patches):**

4. `ProgressState.MaxMultiplayerAscension` getter — 覆盖多人联机最大天梯等级 / Overrides the multiplayer max Ascension (also affects the value reported to the host on join)
5. `ProgressState.PreferredMultiplayerAscension` getter — 同步多人默认选中等级 / Syncs the default multiplayer selected level

---

## 项目结构 / Project Structure

```
STS2-Ascension-level-unlock/
├── scripts/
│   ├── Entry.cs              # 模组入口，注册补丁 / Mod entry point, registers patches
│   ├── Patches.cs            # 五个 Harmony 补丁 / Five Harmony patches
│   └── AscensionConfig.cs    # 配置读写 / Config read/write
├── AscensionAdjuster.csproj  # C# 项目文件 / C# project file
├── AscensionAdjuster.json    # 模组清单 / Mod manifest
├── install.bat               # 安装启动器（纯 ASCII）/ Installer launcher (pure ASCII)
├── install.ps1               # 安装脚本（含界面）/ Installer script (with UI)
├── project.godot             # Godot 项目配置 / Godot project config
└── export_presets.cfg        # Godot 导出配置 / Godot export config
```

---

## 兼容性 / Compatibility

- 游戏版本 / Game version: 杀戮尖塔 2 Early Access（Godot 4.5 / .NET 9）
- 平台 / Platform: Windows

### 与 RemoveMultiplayerPlayerLimit 的兼容性 / Compatibility with RemoveMultiplayerPlayerLimit

本模组与 RemoveMultiplayerPlayerLimit（将联机人数上限从 4 人提升至 8 人以上）**完全兼容**，两者的 Harmony 补丁无任何重叠。

两个模组同时启用时：
- 单人天梯覆盖正常工作，不受影响
- 多人联机天梯上限仍取房间内所有玩家中的最低解锁等级
- 人数越多，越可能有玩家拉低天梯上限 — 建议所有玩家均安装 AscensionAdjuster 并设置相同的 `multiplayer_ascension_override`

> Fully compatible with RemoveMultiplayerPlayerLimit (raises lobby cap from 4 to 8+). No overlapping patches. Singleplayer override works as normal. With more players in the lobby, it becomes more important that everyone has AscensionAdjuster installed and configured with the same `multiplayer_ascension_override`.

---

## 许可协议 / License

MIT License — 自由使用、修改和分发。 / Free to use, modify, and distribute.
