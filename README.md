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
| 全局生效/失效（总开关） | Home | 一键开启/关闭全部作弊（默认关闭） |
| 无限金币 | F9 | 金币显示/使用恒为极大值 |
| 无限资源（木/石/铁/油） | F8 | 四种资源恒为极大值 |
| 无限食物 | F7 | 剩余食物恒为极大值 |
| 无限能量 | F6 | 剩余能量恒为极大值 |
| 无限工人 | F5 | 剩余工人恒为极大值 |
| 人口上限拉满 | F4 | 人口上限恒为极大值 |
| 无限库存/建筑上限 | F3 | 仓库/建筑上限恒为极大值 |
| 瞬间建造/训练 | F2 | 把"建造时长"压到 1 秒（可调），建造/升级/训练/维修走游戏自己的进度流程 |
| 建造/训练时长(秒) | — | 选项页滑块 1~60（默认 1）。走原版流程，进度条/血量增长/完工事件都正常 |
| 瞬间研究 + 任意解锁 | F1 | 研究点极大 + 可解锁任意科技 + 作坊研究进度同样压到 1 秒（可调） |
| 无敌（选中单位不掉血） | F10 | 关闭 CLife.AddDamage |
| 超级速度 | F11 | 引擎级变速：直接改写 DXVision.DXGame 的游戏速度倍率 |
| 游戏速度倍率 | — | 选项页滑块 1~10（1=原速，默认 3）。游戏暂停（速度 0）不受影响 |
| 全图显示 | F12 | 迷雾全开 |
| 摧毁选中单位 | Delete | 对当前选中单位造成致命伤害 |

所有热键与默认开关均可通过游戏内选项页或 Mods/Configs/TABCheats.json 修改。**默认 EnableCheats=false，TABCheats 不会自动生效**；按 Home（或勾选项里的“启用 TABCheats 作弊总开关”）才会整体开启。

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
      "EnableCheats": false,
      "InfiniteGold": true,
      "InfiniteResources": true,
      "InfiniteFood": true,
      "InfiniteEnergy": true,
      "InfiniteWorkers": true,
      "MaxColonists": true,
      "InfiniteStorage": true,
      "InstantBuild": true,
      "BuildSeconds": 1.0,
      "InstantResearch": true,
      "GodMode": false,
      "FastGameSpeed": false,
      "GameSpeedMultiplier": 3.0,
      "ShowFullMap": false,
      "Amount": 99999999,
      "DiagLog": false,
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
      "DestroyKey": "Delete",
      "MasterKey": "Home"
    }

---

## 技术实现

- 通过 RegisterConfig<TABCheatsConfig>() 把配置注册进 ModLoader 的配置系统，[ConfigOption] 属性驱动游戏内选项页。
- 通过 Harmony 对以下游戏方法打补丁（每次加载都把成败写进 `Mods/TABCheats/TABCheats.log`，`PATCH MISS/ERROR` 一眼可见）：
  - ZX.ZXLevelState 的资源/人口/库存 getter（无限值）
  - **ZX.ZXEntityDefaultParams.get_BuildingTime**（瞬间建造：建筑自身 CBuildable 的进度与血量增长按这个时长走）
  - **ZX.ZXCommandDefaultParams.get_BuildingTime**（瞬间建造：Build/Upgrade 命令的进度按这个时长走）
  - **ZX.Commands.ZXCommand / Technology / Train / Repair 的 `GetExecutionTimeFor`**（四个 override 各自公式不同：
    基类与 Train 是 `round(1.4 × factor × 20)`、**研究（Technology）是 `round(1.4 × factor × 50)`**、维修按受损血量算；
    只压 `get_BuildingTime` 管不到研究/训练，所以这四个都封顶）
    `BuildingTime` 单位是秒且是整数，所以最快只能压到 1 秒；补丁只"封顶"不"抬高"，0 表示瞬发命令（攻击/移动等）不受影响。
  - ZX.ZXCampaignState.get_ResearchPoints / CanUnlockResearch（瞬间研究）
  - **DXVision.DXGame.get_GameSpeed**（超级速度：引擎的物理/逻辑时钟每帧读它；`ZX.DXGameState.get_GameSpeed` 只是存档里的速度快照，改它对游戏速度毫无作用）
  - ZX.Components.CLife.AddDamage（无敌）
  - ZX.ZXSystem_GameLevel.OnKeyUp（热键）
- 全程不写游戏存档、不修改游戏文件、不注入外部进程。

### 选项页（MOD 选项）注意事项

- **滑块/数字框必须带 `[Range(min, max, step)]`**：ModLoader 的 `ConfigUIHelper.ValidateValue` 会拿 `[Range]` 的 Min/Max 校验，缺省时 Min=Max=0 —— 滑块范围变成 0~0，选项页里根本改不动，还会把值写回 0。本项目所有 Slider / NumberInput 都已带上 `[Range]`。
- 标签页里的改动会即时写回 `Mods/Configs/TABCheats.json` 并立即生效（无需重启）。
- 钥匙绑定（KeyBinding）建议直接用文本编辑器改 JSON，最省事。
- 存档提示：游戏会把"当前速度"一起存进存档。开着超级速度存档后，即使关掉作弊，读档也会保留该速度（按 `+`/`-` 或选项页倍率即可调回）。

## 更新日志

### v1.0.4（修复"研究了半天没变快"）

- **根因**：v1.0.3 只压了 `ZXEntityDefaultParams/ZXCommandDefaultParams.get_BuildingTime`，而**研究**走的是
  `ZX.Commands.Technology.GetExecutionTimeFor()` —— 它自己 override 了时长公式（`round(1.4 × buildingTimeFactor × 50)`），
  根本不读 `BuildingTime`，所以作坊里那条研究进度条一直按原速走。`Train.GetExecutionTimeFor()` 同理（也是 override）。
- **改法**：对 `ZX.Commands.ZXCommand / Technology / Train / Repair` 四个 `GetExecutionTimeFor` 都加"封顶"补丁
  （只压不涨，0 的瞬发命令不动）→ 研究、训练、建造、升级、维修统一走"建造/训练时长(秒)"（默认 1 秒）。
- 诊断日志新增研究完成事件（`Technology.OnFinish (研究完成)`），方便确认。

### v1.0.3（修复"开启瞬间建造后建筑刚放下就消失"）

- **根因**：v1.0.2 用"把 CBuildable/CBuilder 的 BuildingFactor 直接改成 10000"来实现瞬间完工。
  `ZX.Commands.Build.IsCommandFinished` 判的是 `BuildingFactor >= 1`，于是**建造命令在放置的同一个 tick 里就被判定完工**，
  建造站点自己的 `CBuildable.Entity_EventOnUpdate`（设置 20% 初始血量 → 逐渐涨满 → 调 `Finish()` → 清理组件/视觉）
  根本没机会跑完，站点就被当成"完工/无效"收拾掉了 —— 表现就是建筑刚落下就没了。
- **改法**：不再伪造进度，改成压缩**建造时长**本身（`get_BuildingTime`），整条建造链路（进度条、血量增长、
  `Finish()`、`NotifyEntityBuilt`、任务目标统计）100% 走游戏原逻辑，只是从 28 秒变成 1 秒。
  新增选项 `建造/训练时长(秒)`（1~60，默认 1）；把时长设成 0 会除以零，代码里已强制下限为 1。
- **诊断**：新增 `诊断日志(排查用)` 开关，打开后 `TABCheats.log` 会记录
  "哪个结构被 Dispose（带调用栈）/ Build 命令 OnCancel/OnFinish/完工判定 / Destroy/撤销命令"，
  以后再出"建筑消失"这类问题能直接看日志定位。日志超过 4 MB 自动停止写入。

### v1.0.2（修复"选项页调不了速度 / 瞬间建造不生效 / 资源不增长"）

1. **选项页无法调节游戏速度**：`GameSpeedMultiplier` 是 Slider 却没有 `[Range]`，ModLoader 给它的 Min=Max=0，
   滑块是 0~0 的空条；一旦被拖动/重置就会把值写成 `0.0`。而速度 0 在引擎里等于 **暂停**
   （`DXGame.Paused == (GameSpeed == 0)`），于是游戏时间不再推进 —— 建筑不完工、资源不增长，
   正好对应用户报的另外两个现象。现在：滑块 `[Range(1,10,1)]` 可正常调节；倍率 < 1 一律按 3.0 处理，永远不会把游戏卡成暂停。
2. **超级速度打错了方法**：原来补的是 `ZX.DXGameState.get_GameSpeed`，而它只被 `WriteCurrentGameState`（进关卡/读档时把存档里的速度快照推给引擎）读取，
   对运行中的游戏速度没有任何影响。现在补的是 `DXVision.DXGame.get_GameSpeed`（引擎每帧读的那个）。
3. **瞬间建造只对训练/维修生效**：新建与升级走 `CBuildable.get_BuildingFactor`，原来只补了 `CBuilder` 那一份。现在两个组件都补。
4. 暂停保护：速度 0 不覆盖，开着作弊也能正常暂停。
5. 诊断：每次加载输出每个补丁的 OK/MISS/ERROR 到 `TABCheats.log`，并顺手修好了 `scripts/build.ps1`（原来根目录算错、路径含空格时 csc 报 CS2005）。

---

## 许可证

MIT

---

## 免责声明

本项目仅供单机游戏研究与个人娱乐。请自行遵守游戏 EULA、当地法律与托管平台规则。含作弊功能的 MOD 可能影响成就/排行榜，请谨慎使用。
