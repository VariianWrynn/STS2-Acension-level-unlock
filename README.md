# Ascension Adjuster — 天梯难度调整器

**杀戮尖塔 2** 模组 &nbsp;|&nbsp; [English](README_EN.md)

无需逐级通关解锁，直接为任意角色自由设定天梯难度等级（0–10 级）。

---

## 目录

- [功能特性](#功能特性)
- [安装方法](#安装方法)
- [激活模组](#激活模组)
- [配置说明](#配置说明)
- [多人联机](#多人联机)
- [天梯等级一览](#天梯等级一览)
- [技术原理](#技术原理)
- [兼容性](#兼容性)
- [许可协议](#许可协议)

---

## 功能特性

- 全角色天梯等级一键解锁至任意级别（默认全部解锁至 10 级）
- 支持为每个角色单独设定不同的天梯上限
- 对从未上过天梯、甚至从未使用过的角色同样有效
- 支持多人联机天梯等级覆盖（需单独配置，默认关闭）
- 不修改任何存档文件，禁用模组后游戏恢复原始进度
- 不影响自然解锁流程：正常通关仍会正常解锁下一级

---

## 安装方法

### 方法一：自动安装（推荐）

1. 前往 [Releases](../../releases) 页面下载最新版本的压缩包
2. 解压到任意位置
3. 双击运行 `install.bat`
4. 脚本会自动检测 Steam 安装位置并复制模组文件

### 方法二：手动安装

1. 下载 `AscensionAdjuster.dll` 和 `AscensionAdjuster.json`
2. 在游戏目录下新建文件夹 `mods\AscensionAdjuster\`
3. 将上述两个文件复制到该文件夹中

```
[游戏目录]\mods\AscensionAdjuster\
例如: D:\Steam\steamapps\common\Slay the Spire 2\mods\AscensionAdjuster\
```

> 若不确定游戏位置，请在 Steam 库中右键游戏 → 管理 → 浏览本地文件

### 方法三：从源码构建

需要安装 [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)。

```bash
git clone https://github.com/VariianWrynn/STS2-Acension-level-unlock.git
cd STS2-Acension-level-unlock
dotnet build
```

构建完成后，DLL 和 JSON 会自动复制到游戏的 `mods` 目录。

> 注意：`AscensionAdjuster.csproj` 中的 `Sts2Dir` 路径需指向你的游戏安装目录。

---

## 激活模组

1. 启动「杀戮尖塔 2」
2. 在主菜单点击 **Mods（模组）** 按钮
3. 勾选 **Ascension Adjuster**
4. 重启游戏

激活后，在角色选择界面即可看到天梯等级选择器，所有角色均可选择 0–10 级。

---

## 配置说明

首次运行游戏后，模组目录中会自动生成 `config.json`，用记事本打开即可编辑。

```
[游戏目录]\mods\AscensionAdjuster\config.json
```

### 默认配置

```json
{
  "enabled": true,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

所有角色的单人天梯上限统一解锁至第 10 级，多人联机天梯默认不干预。

### 配置项说明

| 配置项 | 类型 | 说明 |
|---|---|---|
| `enabled` | `true` / `false` | 模组总开关。设为 `false` 可临时禁用，无需卸载模组 |
| `global_ascension_override` | `0`–`10` 或 `-1` | 单人全局天梯上限，对所有角色生效。`-1` 表示不干预，使用游戏原始进度 |
| `character_overrides` | 对象 | 为单个角色单独设置上限，优先级高于全局设置 |
| `multiplayer_ascension_override` | `0`–`10` 或 `-1` | 多人联机天梯上限。`-1` 表示不干预（默认） |

### 角色单独设置

在 `character_overrides` 中填写角色 ID 和对应的天梯上限：

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

### 角色 ID 对照表

| 角色名称 | 角色 ID |
|---|---|
| 铁甲战士 | `CHARACTER.IRONCLAD` |
| 无声者 | `CHARACTER.SILENT` |
| 缺陷体 | `CHARACTER.DEFECT` |
| 摄政王 | `CHARACTER.REGENT` |
| 死灵缚者 | `CHARACTER.NECROBINDER` |

### 常用配置示例

**只解锁铁甲战士，其余角色保持原始进度：**
```json
{
  "enabled": true,
  "global_ascension_override": -1,
  "character_overrides": { "CHARACTER.IRONCLAD": 10 },
  "multiplayer_ascension_override": -1
}
```

**全部角色解锁至第 5 级：**
```json
{
  "enabled": true,
  "global_ascension_override": 5,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

**临时关闭模组（保留配置）：**
```json
{
  "enabled": false,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": -1
}
```

---

## 多人联机

### 单人与多人天梯的区别

杀戮尖塔 2 的天梯系统在单人和多人模式下**完全独立**：

| | 单人模式 | 多人联机 |
|---|---|---|
| 等级存储 | 每个角色各自独立 | 全局共用一个等级 |
| 上限来源 | 角色自身的解锁进度 | 房间内所有玩家中最低的解锁等级 |
| 控制权 | 玩家自由选择 | 仅房主可修改 |

### 启用多人联机天梯覆盖

将 `multiplayer_ascension_override` 设为目标等级（0–10）：

```json
{
  "enabled": true,
  "global_ascension_override": 10,
  "character_overrides": {},
  "multiplayer_ascension_override": 10
}
```

### 重要须知

- 游戏采用 **P2P 联机**，无中央服务器。房间天梯上限由所有玩家中最低的解锁等级决定。
- **房间内所有玩家都需要安装本模组**，并设置相同的 `multiplayer_ascension_override` 值，覆盖才能完全生效。
- 若某位玩家未安装模组，其较低的解锁等级仍会拉低整个房间的天梯上限。
- 此设置默认关闭（`-1`），不会对未配置的玩家产生任何影响。

---

## 天梯等级一览

| 等级 | 名称 | 效果 |
|:---:|---|---|
| 1 | 蜂拥精英 | 精英怪出现频率增加 |
| 2 | 疲惫旅人 | 古老遗迹仅恢复 80% 缺失生命 |
| 3 | 贫困潦倒 | 敌人和宝箱掉落金币减少 25% |
| 4 | 节衣缩食 | 起始药水栏位减少 1 个 |
| 5 | 登塔者之灾 | 起始时获得一张诅咒牌 |
| 6 | 阴霾笼罩 | 篝火休息点减少 |
| 7 | 物资匮乏 | 稀有卡和升级卡出现概率降低 |
| 8 | 强敌环伺 | 所有敌人更难击杀 |
| 9 | 致命一击 | 所有敌人攻击更致命 |
| 10 | 双重 Boss | 第三幕末尾同时面对两个 Boss |

---

## 技术原理

本模组使用 [Harmony](https://github.com/pardeike/Harmony) 运行时补丁技术，共五个 Postfix 补丁。所有补丁仅在游戏运行时生效，不修改任何存档或游戏文件。

**单人模式（3 个补丁）：**
1. `CharacterStats.MaxAscension` getter — 将最大天梯等级替换为配置值
2. `CharacterStats.PreferredAscension` getter — 将默认选中等级同步为配置值
3. `StartRunLobby.IsAscensionEpochRevealed` — 绕过「必须先通关第三幕」的前置限制，使从未使用过的角色也能显示天梯选择器

**多人联机（2 个补丁）：**
4. `ProgressState.MaxMultiplayerAscension` getter — 覆盖多人联机最大天梯等级（同时影响加入房间时上报给房主的解锁等级）
5. `ProgressState.PreferredMultiplayerAscension` getter — 将多人联机默认选中等级同步为配置值

---

## 项目结构

```
STS2-Acension-level-unlock/
├── scripts/
│   ├── Entry.cs              # 模组入口点，注册 Harmony 补丁
│   ├── Patches.cs            # 五个 Harmony 补丁的实现
│   └── AscensionConfig.cs    # 配置文件读写逻辑
├── AscensionAdjuster.csproj  # C# 项目文件
├── AscensionAdjuster.json    # 模组清单（游戏识别用）
├── install.bat               # 自动安装启动器（纯 ASCII）
├── install.ps1               # 自动安装脚本（含中文界面）
├── project.godot             # Godot 项目配置
└── export_presets.cfg        # Godot 导出配置
```

---

## 兼容性

- 游戏版本：杀戮尖塔 2 Early Access（Godot 4.5 / .NET 9）
- 运行平台：Windows

### 与 RemoveMultiplayerPlayerLimit 的兼容性

本模组与 RemoveMultiplayerPlayerLimit（将联机人数上限从 4 人提升至更多）**完全兼容**，两者的 Harmony 补丁无任何重叠。

两个模组同时启用时：
- 单人天梯覆盖正常工作，不受影响
- 多人联机天梯上限仍取房间内所有玩家中的最低解锁等级
- 人数越多，越可能有玩家拉低天梯上限——建议所有玩家均安装 AscensionAdjuster 并设置相同的 `multiplayer_ascension_override`

---

## 许可协议

[MIT License](LICENSE) — 自由使用、修改和分发。
