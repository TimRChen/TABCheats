# TABCheats — 《They Are Billions》亿万僵尸 游戏内作弊/辅助 MOD

TABCheats 是一个基于 **TABModLoader + Harmony** 的《They Are Billions》(亿万僵尸) 游戏内作弊 MOD。
它把原外部修改器（v1.0.13 Plus 20 Trainer）的核心功能**放进游戏进程内**实现，因此：

- 与中文汉化、TABHelper、存档管理器等 MOD **同进程共存**，互不影响；
- **不修改 TABHelper**，不改 TABHelper 的任何文件；
- 不依赖外部进程 / 内存修改器，不触发 Eazfuscator 反篡改；
- 所有作弊项都支持**热键切换**，也支持在游戏内 **MOD 选项页**直接勾选、即时生效。

> 本项目是源码开源的 MOD。游戏本体、DXVision.dll、0Harmony.dll 等私有二进制不属于本项目，请从你自己的游戏安装中获取/提取。

---

## 功能

| 功能 | 默认热键 | 说明 |
|---|---|---|
| 启用 TABCheats 总开关 | — | 勾选后全部作弊生效；取消即关闭 |
| 无限金币 | F9 | 金币显示/使用恒为极大值 |
| 无限资源（木/石/铁/油） | F8 | 四种资源恒为极大值 |
| 无限食物 | F7 | 剩余食物恒为极大值 |
| 无限能量 | F6 | 剩余能量恒为极大值 |
| 无限工人 | F5 | 剩余工人恒为极大值 |
| 人口上限拉满 | F4 | 人口上限恒为极大值 |
| 无限库存/建筑上限 | F3 | 仓库/建筑上限恒为极大值 |
| 瞬间建造/训练 | F2 | BuildingFactor 拉满，建筑/训练即刻完成 |
| 瞬间研究 + 任意解锁 | F1 | 研究点极大 + 可解锁任意科技 |
| 无敌（选中单位不掉血） | F10 | 关闭 CLife.AddDamage |
| 超级速度 | F11 | 游戏速度设为倍率 |
| 全图显示 | F12 | 迷雾全开 |
| 摧毁选中单位 | Delete | 对当前选中单位造成致命伤害 |

所有热键与默认开关均可通过游戏内选项页或 Mods/Configs/TABCheats.json 修改。

---

## 安装

1. 把仓库里的 Mods/TABCheats 文件夹整个复制到游戏根目录的 Mods 下：

        <游戏根目录>/Mods/TABCheats/
            TABCheats.dll
            ModInfos.json

2. （可选）把 Mods/Configs/TABCheats.json 放到 <游戏根目录>/Mods/Configs/；不放也会由游戏自动生成默认配置。
3. 启动游戏 → **MOD 管理器/选项页** 找到 **TABCheats** → 勾选“启用 TABCheats 作弊总开关”。
4. 进战役，用上面的热键或选项页开关作弊。

> 需要游戏已部署 TABModLoader（codeberg.org/JKstring/TABModLoader）并能正常加载 TABHelper 类 MOD。

---

## 从源码构建

要求：Windows + .NET Framework 4.x（自带 csc.exe），以及你的本地游戏安装。

    # 1. 从游戏 exe 内嵌的 Costura 资源提取 0Harmony.dll 与 DXVision.dll（只用于编译，不重新分发）
    powershell -ExecutionPolicy Bypass -File scripts/extract-refs.ps1 -GameExe "Z:/zombie_game/zombit_army_game/YiWanJiangShiJunTuan v1.0.14/TheyAreBillions.exe"

    # 2. 编译生成 dist/TABCheats.dll
    powershell -ExecutionPolicy Bypass -File scripts/build.ps1 -GameExe "Z:/zombie_game/zombit_army_game/YiWanJiangShiJunTuan v1.0.14/TheyAreBillions.exe"

    # 3. 复制到游戏
    copy dist\TABCheats.dll "<游戏根目录>\Mods\TABCheats\TABCheats.dll"

脚本说明：

- scripts/extract-refs.ps1 通过 .NET 反射读取 TheyAreBillions.exe 的 costura.*.compressed 资源并解压（Deflate），得到 refs/0Harmony.dll 和 refs/DXVision.dll。**不包含任何游戏私有二进制。**
- scripts/build.ps1 用 csc.exe 编译 src/TABCheats.cs，引用 refs/*.dll 与游戏 exe。

---

## 配置文件 Mods/Configs/TABCheats.json

示例（可在游戏选项页改，也会写回此文件）：

    {
      "EnableCheats": true,
      "InfiniteGold": true,
      "InfiniteResources": true,
      "InfiniteFood": true,
      "InfiniteEnergy": true,
      "InfiniteWorkers": true,
      "MaxColonists": true,
      "InfiniteStorage": true,
      "InstantBuild": true,
      "InstantResearch": true,
      "GodMode": false,
      "FastGameSpeed": false,
      "GameSpeedMultiplier": 3.0,
      "ShowFullMap": false,
      "Amount": 99999999,
      "GoldKey": "F9",
      "ResourcesKey": "F8",
      "FoodKey": "F7",
      "EnergyKey": "F6",
      "WorkersKey": "F5",
      "ColonistsKey": "F4",
      "StorageKey": "F3",
      "BuildKey": "F2",
      "ResearchKey": "F1",
      "GodModeKey": "F10",
      "SpeedKey": "F11",
      "ShowFullMapKey": "F12",
      "DestroyKey": "Delete"
    }

---

## 技术实现

- 通过 RegisterConfig<TABCheatsConfig>() 把配置注册进 ModLoader 的配置系统，[ConfigOption] 属性驱动游戏内选项页。
- 通过 Harmony 对以下游戏方法打补丁：
  - ZX.ZXLevelState 的资源/人口/库存 getter（无限值）
  - ZX.Components.CBuilder.get_BuildingFactor（瞬间建造）
  - ZX.ZXCampaignState.get_ResearchPoints / CanUnlockResearch（瞬间研究）
  - ZX.DXGameState.get_GameSpeed（超级速度）
  - ZX.Components.CLife.AddDamage（无敌）
  - ZX.ZXSystem_GameLevel.OnKeyUp（热键）
- 全程不写游戏存档、不修改游戏文件、不注入外部进程。

---

## 许可证

MIT

---

## 免责声明

本项目仅供单机游戏研究与个人娱乐。请自行遵守游戏 EULA、当地法律与托管平台规则。含作弊功能的 MOD 可能影响成就/排行榜，请谨慎使用。
