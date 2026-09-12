using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using HarmonyLib;
using AX.ModLoader.Config;
using ZX;
using ZX.Components;
using DXVision;
using ZX.Entities;

namespace TABCheats
{
    public class TABCheatsConfig : ModConfig
    {
        [ConfigOption("启用 TABCheats 作弊总开关", ConfigOptionType.Checkbox, Category = "通用", Order = 0)]
        public bool EnableCheats { get; set; }

        [ConfigOption("无限金币", ConfigOptionType.Checkbox, Category = "资源", Order = 1)]
        public bool InfiniteGold { get; set; }

        [ConfigOption("无限资源(木/石/铁/油)", ConfigOptionType.Checkbox, Category = "资源", Order = 2)]
        public bool InfiniteResources { get; set; }

        [ConfigOption("无限食物", ConfigOptionType.Checkbox, Category = "资源", Order = 3)]
        public bool InfiniteFood { get; set; }

        [ConfigOption("无限能量", ConfigOptionType.Checkbox, Category = "资源", Order = 4)]
        public bool InfiniteEnergy { get; set; }

        [ConfigOption("无限工人", ConfigOptionType.Checkbox, Category = "资源", Order = 5)]
        public bool InfiniteWorkers { get; set; }

        [ConfigOption("人口上限拉满", ConfigOptionType.Checkbox, Category = "资源", Order = 6)]
        public bool MaxColonists { get; set; }

        [ConfigOption("无限库存/建筑上限", ConfigOptionType.Checkbox, Category = "资源", Order = 7)]
        public bool InfiniteStorage { get; set; }

        [ConfigOption("瞬间建造/训练", ConfigOptionType.Checkbox, Category = "速度", Order = 8)]
        public bool InstantBuild { get; set; }

        // 建造/训练/研究时长（秒）。游戏里这类时长是"秒"的整数，最小 1。
        // 走游戏自己的进度流程（进度条、血量增长、完工事件全都正常），只是把时长压到最短。
        // 故意**不挂 [ConfigOption]**：选项页的滑块容易被误拖成 60（那会让建造/研究重新变慢），
        // 需要微调的人直接改 Mods/Configs/TABCheats.json 里的 BuildSeconds 即可。
        [Range(1.0, 60.0, 1.0)]
        public double BuildSeconds { get; set; }

        [ConfigOption("瞬间研究+任意解锁", ConfigOptionType.Checkbox, Category = "速度", Order = 9)]
        public bool InstantResearch { get; set; }

        [ConfigOption("超级速度", ConfigOptionType.Checkbox, Category = "速度", Order = 10)]
        public bool FastGameSpeed { get; set; }

        // 注意：必须带 [Range]，否则 ModLoader 的滑块 Min=Max=0，选项页里根本改不动，
        // 而且会被写回 0.0 —— 0 在引擎里等于"暂停"（DXGame.Paused == GameSpeed==0）。
        [ConfigOption("游戏速度倍率(1=原速)", ConfigOptionType.Slider, Category = "速度", Order = 11)]
        [Range(1.0, 10.0, 1.0)]
        public double GameSpeedMultiplier { get; set; }

        [ConfigOption("无敌(选中单位不掉血)", ConfigOptionType.Checkbox, Category = "战斗", Order = 12)]
        public bool GodMode { get; set; }

        [ConfigOption("全图显示", ConfigOptionType.Checkbox, Category = "战斗", Order = 13)]
        public bool ShowFullMap { get; set; }

        // 同理：NumberInput 也会按 [Range] 的 Min/Max 校验，没有 Range 就只能填 0。
        [ConfigOption("作弊数值", ConfigOptionType.NumberInput, Category = "通用", Order = 14)]
        [Range(1.0, 2000000000.0, 1.0)]
        public int Amount { get; set; }

        // 排查用：把结构体销毁/建造命令生命周期带调用栈写进 TABCheats.log（平时关掉）
        [ConfigOption("诊断日志(排查用)", ConfigOptionType.Checkbox, Category = "通用", Order = 15)]
        public bool DiagLog { get; set; }

        [ConfigOption("摧毁选中单位 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 20)]
        public string DestroyKey { get; set; }

        [ConfigOption("金币 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 21)]
        public string GoldKey { get; set; }

        [ConfigOption("资源 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 22)]
        public string ResourcesKey { get; set; }

        [ConfigOption("食物 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 23)]
        public string FoodKey { get; set; }

        [ConfigOption("能量 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 24)]
        public string EnergyKey { get; set; }

        [ConfigOption("工人 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 25)]
        public string WorkersKey { get; set; }

        [ConfigOption("人口 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 26)]
        public string ColonistsKey { get; set; }

        [ConfigOption("库存 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 27)]
        public string StorageKey { get; set; }

        [ConfigOption("建造 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 28)]
        public string BuildKey { get; set; }

        [ConfigOption("研究 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 29)]
        public string ResearchKey { get; set; }

        [ConfigOption("无敌 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 30)]
        public string GodModeKey { get; set; }

        [ConfigOption("加速 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 31)]
        public string SpeedKey { get; set; }

        [ConfigOption("全图 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 32)]
        public string ShowFullMapKey { get; set; }

        [ConfigOption("全局生效/失效 热键", ConfigOptionType.KeyBinding, Category = "热键", Order = 33)]
        public string MasterKey { get; set; }

        public TABCheatsConfig()
        {
            EnableCheats = false;
            InfiniteGold = true;
            InfiniteResources = true;
            InfiniteFood = true;
            InfiniteEnergy = true;
            InfiniteWorkers = true;
            MaxColonists = true;
            InfiniteStorage = true;
            InstantBuild = true;
            InstantResearch = true;
            GodMode = false;
            FastGameSpeed = false;
            GameSpeedMultiplier = 3.0;
            BuildSeconds = 1.0;
            DiagLog = false;
            ShowFullMap = false;
            Amount = 99999999;
            GoldKey = "F9";
            ResourcesKey = "F8";
            FoodKey = "F7";
            EnergyKey = "F6";
            WorkersKey = "F5";
            ColonistsKey = "F4";
            StorageKey = "F3";
            BuildKey = "F2";
            ResearchKey = "F1";
            GodModeKey = "F10";
            SpeedKey = "F11";
            ShowFullMapKey = "F12";
            DestroyKey = "Delete";
            MasterKey = "Home";
        }
    }

    public class ModEntry : AX.ModLoader.IModEntry
    {
        public static ModEntry Instance;
        public static TABCheatsConfig Cfg;
        public static string LogPath;
        private AX.ModLoader.Mod _mod;
        private HarmonyLib.Harmony _harmony;

        public override void OnLoad(AX.ModLoader.Mod mod)
        {
            Instance = this;
            _mod = mod;
            try { LogPath = Path.Combine(mod.ModPath, "TABCheats.log"); } catch (Exception) { }
            WriteLog("OnLoad start, ModPath=" + mod.ModPath);
            try
            {
                Cfg = _mod.RegisterConfig<TABCheatsConfig>();
                if (Cfg == null) { Cfg = new TABCheatsConfig(); }
                WriteLog("Config registered. EnableCheats=" + Cfg.EnableCheats);
                _harmony = new HarmonyLib.Harmony("TABCheats");
                PatchAll();
                WriteLog("OnLoad OK");
            }
            catch (Exception ex)
            {
                WriteLog("OnLoad ERROR: " + ex);
                throw;
            }
        }

        public override void OnLoadResources() { }

        public override void OnUnload()
        {
            if (_harmony != null)
            {
                try { _harmony.UnpatchAll("TABCheats"); } catch (Exception) { }
            }
        }

        private void WriteLog(string msg)
        {
            try
            {
                string p = LogPath;
                if (string.IsNullOrEmpty(p)) p = Path.Combine(_mod.ModPath, "TABCheats.log");
                File.AppendAllText(p, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg + Environment.NewLine);
            }
            catch (Exception) { }
        }

        private void PatchAll()
        {
            PatchGetter(typeof(ZX.ZXLevelState), "get_Gold", "GoldPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_Wood", "ResourcesPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_Stone", "ResourcesPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_Iron", "ResourcesPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_Oil", "ResourcesPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_RemainingFood", "FoodPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_RemainingEnergy", "EnergyPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_RemainingWorkers", "WorkersPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_MaxColonists", "MaxColonistsPostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_TotalGoldStorage", "StoragePostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_TotalResourcesStorage", "StoragePostfix");
            PatchGetter(typeof(ZX.ZXLevelState), "get_ShowFullMap", "ShowFullMapPostfix");
            // 瞬间建造：不去伪造 BuildingFactor（伪造进度会让建筑"刚点完就消失"：
            // 进度被瞬间推到 >=1 时，建造命令会在同一个 tick 里走 OnFinish，建造站点还没走完自己的初始化就被判完工）。
            // 这里改为压缩"建造时长"本身，建造流程 100% 走游戏原逻辑：
            //   ZXEntityDefaultParams.BuildingTime  -> CBuildable.Entity_EventOnUpdate 的进度与血量增长
            //   ZXCommandDefaultParams.BuildingTime -> ZXCommand.OnUpdate / GetExecutionTimeFor（训练/维修/升级同样生效）
            PatchGetter(typeof(ZX.ZXEntityDefaultParams), "get_BuildingTime", "BuildTimePostfix");
            PatchGetter(typeof(ZX.ZXCommandDefaultParams), "get_BuildingTime", "BuildTimePostfix");
            // 研究/训练/维修的时长各自是 override，公式还不一样（研究界面上那条进度条走的是 Technology ×50、
            // 训练是 Train ×20、维修按血量算），只压 get_BuildingTime 管不到它们，所以四个 override 都封顶。
            PatchGetter(typeof(ZX.Commands.ZXCommand), "GetExecutionTimeFor", "ExecutionTimePostfix");
            PatchGetter(typeof(ZX.Commands.Technology), "GetExecutionTimeFor", "ExecutionTimePostfix");
            PatchGetter(typeof(ZX.Commands.Train), "GetExecutionTimeFor", "ExecutionTimePostfix");
            PatchGetter(typeof(ZX.Commands.Repair), "GetExecutionTimeFor", "ExecutionTimePostfix");
            PatchGetter(typeof(ZX.ZXCampaignState), "get_ResearchPoints", "ResearchPostfix");
            PatchAnyMethod(typeof(ZX.ZXCampaignState), "CanUnlockResearch", "CanUnlockResearchPrefix", true);
            // 真正的游戏速度在引擎 DXVision.DXGame._GameSpeed（物理/逻辑每帧都读它）。
            // ZX.DXGameState.get_GameSpeed 只是存档里的速度快照，改它对游戏速度没有任何作用。
            PatchGetter(typeof(DXVision.DXGame), "get_GameSpeed", "GameSpeedPostfix");
            PatchAnyMethod(typeof(ZX.Components.CLife), "AddDamage", "AddDamagePrefix", true);

            MethodInfo keyUp = typeof(ZX.ZXSystem_GameLevel).GetMethod("OnKeyUp");
            if (keyUp != null)
            {
                _harmony.Patch(keyUp, null, new HarmonyMethod(typeof(Patches).GetMethod("OnKeyUpPostfix", BindingFlags.Static | BindingFlags.Public)));
            }

            PatchDiag();
        }

        // "诊断日志"开启时才有输出：谁把建筑销毁了、建造命令怎么走完的，全带调用栈。
        private void PatchDiag()
        {
            PatchDiagOne("DXVision.DXEntity.Dispose", "DisposePrefix", false);
            PatchDiagOne("ZX.Commands.Build.OnCancel", "BuildOnCancelPrefix", true);
            PatchDiagOne("ZX.Commands.Build.OnFinish", "BuildOnFinishPrefix", true);
            PatchDiagOne("ZX.Commands.Build.IsCommandFinished", "BuildFinishedPostfix", false);
            PatchDiagOne("ZX.Components.CBuildable.Finish", "BuildableFinishPrefix", true);
            PatchDiagOne("ZX.Commands.Destroy.OnExecute", "DestroyPrefix", true);
            PatchDiagOne("ZX.Commands.UndoBuilding.OnExecute", "UndoPrefix", true);
            PatchDiagOne("ZX.Commands.Technology.OnFinish", "TechnologyFinishPrefix", true);
        }

        private void PatchDiagOne(string typeDotMethod, string patchName, bool prefix)
        {
            try
            {
                string tn = typeDotMethod.Substring(0, typeDotMethod.LastIndexOf('.'));
                string mn = typeDotMethod.Substring(typeDotMethod.LastIndexOf('.') + 1);
                Type t = FindType(tn);
                if (t == null) { Report("DIAG PATCH MISS " + typeDotMethod + " (找不到类型)"); return; }
                MethodInfo mi = t.GetMethod(mn, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (mi == null) { Report("DIAG PATCH MISS " + typeDotMethod + " (找不到方法)"); return; }
                MethodInfo pm = typeof(DiagPatches).GetMethod(patchName, BindingFlags.Static | BindingFlags.Public);
                if (pm == null) { Report("DIAG PATCH MISS " + patchName); return; }
                if (prefix) _harmony.Patch(mi, new HarmonyMethod(pm), null);
                else _harmony.Patch(mi, null, new HarmonyMethod(pm));
                Report("DIAG PATCH OK  " + typeDotMethod + " <- " + patchName);
            }
            catch (Exception ex)
            {
                Report("DIAG PATCH ERROR " + typeDotMethod + " : " + ex.Message);
            }
        }

        // 在已加载的程序集里找类型（DXVision / 游戏本体都可能）
        private static Type FindType(string fullName)
        {
            Type t = Type.GetType(fullName + ", DXVision");
            if (t != null) return t;
            t = Type.GetType(fullName + ", TheyAreBillions");
            if (t != null) return t;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try { t = a.GetType(fullName, false); } catch (Exception) { t = null; }
                if (t != null) return t;
            }
            return null;
        }

        // 每次加载都会把每个补丁的成败写进 TABCheats.log，出了问题一眼能看出是哪个补丁没打上。
        public static readonly List<string> PatchReport = new List<string>();

        private void Report(string line)
        {
            PatchReport.Add(line);
            WriteLog(line);
        }

        private void PatchGetter(Type type, string methodName, string patchName)
        {
            MethodInfo mi = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (mi == null) { Report("PATCH MISS  " + type.FullName + "." + methodName + " (游戏里找不到这个方法)"); return; }
            MethodInfo pm = typeof(Patches).GetMethod(patchName, BindingFlags.Static | BindingFlags.Public);
            if (pm == null) { Report("PATCH MISS  " + patchName); return; }
            try
            {
                _harmony.Patch(mi, null, new HarmonyMethod(pm));
                Report("PATCH OK    " + type.FullName + "." + methodName + " <- " + patchName);
            }
            catch (Exception ex)
            {
                Report("PATCH ERROR " + type.FullName + "." + methodName + " : " + ex.Message);
            }
        }

        private void PatchAnyMethod(Type type, string methodName, string patchName, bool prefix)
        {
            MethodInfo mi = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (mi == null) { Report("PATCH MISS  " + type.FullName + "." + methodName + " (游戏里找不到这个方法)"); return; }
            MethodInfo pm = typeof(Patches).GetMethod(patchName, BindingFlags.Static | BindingFlags.Public);
            if (pm == null) { Report("PATCH MISS  " + patchName); return; }
            try
            {
                if (prefix) _harmony.Patch(mi, new HarmonyMethod(pm), null);
                else _harmony.Patch(mi, null, new HarmonyMethod(pm));
                Report("PATCH OK    " + type.FullName + "." + methodName + " <- " + patchName);
            }
            catch (Exception ex)
            {
                Report("PATCH ERROR " + type.FullName + "." + methodName + " : " + ex.Message);
            }
        }
    }

    public class Patches
    {
        public const int Big = 99999999;
        public const double DefaultSpeedMultiplier = 3.0;
        public const double MaxSpeedMultiplier = 10.0;

        private static bool ON { get { return ModEntry.Cfg != null && ModEntry.Cfg.EnableCheats; } }

        public static void GoldPostfix(ref int __result)
        {
            if (ON && ModEntry.Cfg.InfiniteGold) __result = ModEntry.Cfg.Amount;
        }
        public static void ResourcesPostfix(ref int __result)
        {
            if (ON && ModEntry.Cfg.InfiniteResources) __result = ModEntry.Cfg.Amount;
        }
        public static void FoodPostfix(ref int __result)
        {
            if (ON && ModEntry.Cfg.InfiniteFood) __result = ModEntry.Cfg.Amount;
        }
        public static void EnergyPostfix(ref int __result)
        {
            if (ON && ModEntry.Cfg.InfiniteEnergy) __result = ModEntry.Cfg.Amount;
        }
        public static void WorkersPostfix(ref int __result)
        {
            if (ON && ModEntry.Cfg.InfiniteWorkers) __result = ModEntry.Cfg.Amount;
        }
        public static void MaxColonistsPostfix(ref int __result)
        {
            if (ON && ModEntry.Cfg.MaxColonists) __result = ModEntry.Cfg.Amount;
        }
        public static void StoragePostfix(ref int __result)
        {
            if (ON && ModEntry.Cfg.InfiniteStorage) __result = ModEntry.Cfg.Amount;
        }
        // 目标时长（秒）。必须 >= 1：0 会让进度公式除零（进度一步跳到 Infinity），
        // 也会让 ZXCommand.Execute 跳过 AddComponent<CBuilder>() 导致命令永远跑不完。
        private static int TargetSeconds()
        {
            int secs = (int)Math.Round(ModEntry.Cfg.BuildSeconds);
            if (secs < 1) secs = 1;
            if (secs > 60) secs = 60;
            return secs;
        }

        public static void BuildTimePostfix(ref int __result)
        {
            if (!ON || !ModEntry.Cfg.InstantBuild) return;
            int secs = TargetSeconds();
            if (__result > secs) __result = secs;
        }

        // 只封顶、不抬高：值为 0 的是"瞬发"命令（攻击/移动等），保持原样。
        public static void ExecutionTimePostfix(ref int __result)
        {
            if (!ON || !ModEntry.Cfg.InstantBuild) return;
            int secs = TargetSeconds();
            if (__result > secs) __result = secs;
        }
        public static void ResearchPostfix(ref int __result)
        {
            if (ON && ModEntry.Cfg.InstantResearch) __result = ModEntry.Cfg.Amount;
        }
        public static bool CanUnlockResearchPrefix(ref bool __result)
        {
            if (ON && ModEntry.Cfg.InstantResearch) { __result = true; return false; }
            return true;
        }
        public static void GameSpeedPostfix(ref double __result)
        {
            if (!ON || !ModEntry.Cfg.FastGameSpeed) return;
            // __result == 0 是引擎的"暂停"状态（DXGame.Paused == (GameSpeed == 0)），不能覆盖，
            // 否则开着作弊就没法暂停游戏了。
            if (__result <= 0.0) return;
            double m = ModEntry.Cfg.GameSpeedMultiplier;
            if (double.IsNaN(m) || m < 1.0) m = DefaultSpeedMultiplier;   // 0 / 负值会把游戏卡成暂停
            if (m > MaxSpeedMultiplier) m = MaxSpeedMultiplier;
            __result = m;
        }
        public static void ShowFullMapPostfix(ref bool __result)
        {
            if (ON && ModEntry.Cfg.ShowFullMap) __result = true;
        }

        public static bool AddDamagePrefix()
        {
            if (ON && ModEntry.Cfg.GodMode) return false;
            return true;
        }

        public static void OnKeyUpPostfix(DXVision.DXKeys key)
        {
            if (ModEntry.Cfg == null) return;
            if (KeyEq(ModEntry.Cfg.MasterKey, key))
            {
                ModEntry.Cfg.EnableCheats = !ModEntry.Cfg.EnableCheats;
                try { ModEntry.Cfg.Save(); } catch (Exception) { }
                return;
            }
            if (!ModEntry.Cfg.EnableCheats) return;
            if (KeyEq(ModEntry.Cfg.GoldKey, key)) ModEntry.Cfg.InfiniteGold = !ModEntry.Cfg.InfiniteGold;
            if (KeyEq(ModEntry.Cfg.ResourcesKey, key)) ModEntry.Cfg.InfiniteResources = !ModEntry.Cfg.InfiniteResources;
            if (KeyEq(ModEntry.Cfg.FoodKey, key)) ModEntry.Cfg.InfiniteFood = !ModEntry.Cfg.InfiniteFood;
            if (KeyEq(ModEntry.Cfg.EnergyKey, key)) ModEntry.Cfg.InfiniteEnergy = !ModEntry.Cfg.InfiniteEnergy;
            if (KeyEq(ModEntry.Cfg.WorkersKey, key)) ModEntry.Cfg.InfiniteWorkers = !ModEntry.Cfg.InfiniteWorkers;
            if (KeyEq(ModEntry.Cfg.ColonistsKey, key)) ModEntry.Cfg.MaxColonists = !ModEntry.Cfg.MaxColonists;
            if (KeyEq(ModEntry.Cfg.StorageKey, key)) ModEntry.Cfg.InfiniteStorage = !ModEntry.Cfg.InfiniteStorage;
            if (KeyEq(ModEntry.Cfg.BuildKey, key)) ModEntry.Cfg.InstantBuild = !ModEntry.Cfg.InstantBuild;
            if (KeyEq(ModEntry.Cfg.ResearchKey, key)) ModEntry.Cfg.InstantResearch = !ModEntry.Cfg.InstantResearch;
            if (KeyEq(ModEntry.Cfg.GodModeKey, key)) ModEntry.Cfg.GodMode = !ModEntry.Cfg.GodMode;
            if (KeyEq(ModEntry.Cfg.SpeedKey, key)) ModEntry.Cfg.FastGameSpeed = !ModEntry.Cfg.FastGameSpeed;
            if (KeyEq(ModEntry.Cfg.ShowFullMapKey, key)) ModEntry.Cfg.ShowFullMap = !ModEntry.Cfg.ShowFullMap;
            if (KeyEq(ModEntry.Cfg.DestroyKey, key)) DestroySelected();
            try { ModEntry.Cfg.Save(); } catch (Exception) { }
        }

        private static bool KeyEq(string name, DXKeys key)
        {
            if (string.IsNullOrEmpty(name)) return false;
            try
            {
                DXKeys k = (DXKeys)Enum.Parse(typeof(DXKeys), name, true);
                return k == key;
            }
            catch (Exception) { return false; }
        }

        private static void DestroySelected()
        {
            try
            {
                var all = CSelectable.AllSelected;
                if (all == null) return;
                var list = all.ToList();
                foreach (var sel in list)
                {
                    var ent = sel.Entity as ZXEntity;
                    if (ent == null) continue;
                    var life = ent.get_CLife();
                    if (life != null) life.AddDamage(int.MaxValue, true, default(ZX.ZXDamageType));
                }
            }
            catch (Exception) { }
        }
    }

    // ===== 诊断（只在"诊断日志"开启时输出）=====
    public static class Diag
    {
        public static bool On
        {
            get { return ModEntry.Cfg != null && ModEntry.Cfg.DiagLog; }
        }

        private static bool _tooBig;
        private static int _writes;

        public static void Log(string msg)
        {
            if (!On || _tooBig) return;
            try
            {
                string p = ModEntry.LogPath;
                if (string.IsNullOrEmpty(p)) return;
                if (++_writes % 200 == 0)
                {
                    var fi = new FileInfo(p);
                    if (fi.Exists && fi.Length > 4 * 1024 * 1024) { _tooBig = true; return; }
                }
                File.AppendAllText(p, DateTime.Now.ToString("HH:mm:ss.fff") + " DIAG " + msg + Environment.NewLine + ShortStack());
            }
            catch (Exception) { }
        }

        private static string ShortStack()
        {
            try
            {
                var st = new System.Diagnostics.StackTrace(1, false);
                var sb = new StringBuilder();
                int n = 0;
                for (int i = 0; i < st.FrameCount && n < 10; i++)
                {
                    StackFrame fr = st.GetFrame(i);
                    if (fr == null) continue;
                    MethodBase mb = fr.GetMethod();
                    if (mb == null || mb.DeclaringType == null) continue;
                    sb.Append("        at ").Append(mb.DeclaringType.Name).Append('.').Append(mb.Name).AppendLine();
                    n++;
                }
                return sb.ToString();
            }
            catch (Exception) { return ""; }
        }

        // 实体描述：类型 + 格子 + 建造进度 + 是否在建
        public static string Desc(DXVision.DXEntity e)
        {
            if (e == null) return "<null>";
            try
            {
                string s = e.GetType().Name + "@" + e.Cell.X + "," + e.Cell.Y;
                try
                {
                    ZX.Components.CBuildable cb = e.GetComponent<ZX.Components.CBuildable>();
                    if (cb != null) s += " factor=" + cb.BuildingFactor.ToString("F3");
                }
                catch (Exception) { }
                ZX.Entities.Structure st = e as ZX.Entities.Structure;
                if (st != null) s += " isBeingBuilt=" + (st.IsBeingBuilt ? "1" : "0");
                return s;
            }
            catch (Exception) { return "<?>"; }
        }

        // 命令目标描述（Build 命令的 actor 身上挂着当前建造目标）。
        // CCommandable/Target 在游戏程序集里不是 public，只能反射取。
        private static readonly BindingFlags AnyFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static string TargetDesc(ZX.Entities.ZXEntity actor)
        {
            try
            {
                if (actor == null) return "<null actor>";
                PropertyInfo ccProp = actor.GetType().GetProperty("CCommandable", AnyFlag);
                if (ccProp == null) return "<no CCommandable>";
                object cc = ccProp.GetValue(actor, null);
                if (cc == null) return "<commandable null>";
                PropertyInfo tProp = cc.GetType().GetProperty("Target", AnyFlag);
                if (tProp == null) return "<no Target>";
                object target = tProp.GetValue(cc, null);
                if (target == null) return "<target null>";
                FieldInfo refField = target.GetType().GetField("EntityRef", AnyFlag);
                if (refField == null) return "<no EntityRef>";
                object entityRef = refField.GetValue(target);
                if (entityRef == null) return "<entityRef null>";
                PropertyInfo eProp = entityRef.GetType().GetProperty("Entity", AnyFlag);
                if (eProp == null) return "<no Entity>";
                return Desc(eProp.GetValue(entityRef, null) as DXVision.DXEntity);
            }
            catch (Exception ex) { return "<err " + ex.GetType().Name + ">"; }
        }
    }

    public class DiagPatches
    {
        public static void DisposePrefix(DXVision.DXEntity __instance)
        {
            if (!Diag.On) return;
            if (__instance is ZX.Entities.Structure) Diag.Log("Structure.Dispose " + Diag.Desc(__instance));
        }

        public static void BuildOnCancelPrefix(ZX.Entities.ZXEntity actor)
        {
            Diag.Log("Build.OnCancel actor=" + Diag.Desc(actor) + " target=" + Diag.TargetDesc(actor));
        }

        public static void BuildOnFinishPrefix(ZX.Entities.ZXEntity actor)
        {
            Diag.Log("Build.OnFinish actor=" + Diag.Desc(actor) + " target=" + Diag.TargetDesc(actor));
        }

        // IsCommandFinished 每帧都会被问，只在"确实有建造目标"时记一行（否则日志会被刷爆）
        public static void BuildFinishedPostfix(ZX.Entities.ZXEntity actor, ref bool __result)
        {
            if (!__result || !Diag.On) return;
            string target = Diag.TargetDesc(actor);
            if (target == "<null>" || target.StartsWith("<no") || target.StartsWith("<target") || target.StartsWith("<entityRef")) return;
            Diag.Log("Build.IsCommandFinished=true actor=" + Diag.Desc(actor) + " target=" + target);
        }

        public static void BuildableFinishPrefix(ZX.Components.CBuildable __instance)
        {
            if (!Diag.On) return;
            try { Diag.Log("CBuildable.Finish " + Diag.Desc(__instance.Entity)); }
            catch (Exception) { Diag.Log("CBuildable.Finish <no entity>"); }
        }

        public static void DestroyPrefix(ZX.Entities.ZXEntity actor)
        {
            Diag.Log("Destroy.OnExecute " + Diag.Desc(actor));
        }

        public static void UndoPrefix(ZX.Entities.ZXEntity actor)
        {
            Diag.Log("UndoBuilding.OnExecute " + Diag.Desc(actor));
        }

        public static void TechnologyFinishPrefix(ZX.Entities.ZXEntity actor)
        {
            Diag.Log("Technology.OnFinish (研究完成) " + Diag.Desc(actor));
        }
    }
}
