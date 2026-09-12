using System;
using System.IO;
using System.Collections.Generic;
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
        private AX.ModLoader.Mod _mod;
        private HarmonyLib.Harmony _harmony;

        public override void OnLoad(AX.ModLoader.Mod mod)
        {
            Instance = this;
            _mod = mod;
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
                string p = Path.Combine(_mod.ModPath, "TABCheats.log");
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
            // 瞬间建造：建造(Build)/升级(Upgrade) 走的是 CBuildable，训练(Train)/维修(Repair) 走的是 CBuilder，
            // 两个组件各有一份 _BuildingFactor，只打 CBuilder 会导致"建筑和升级一点都不快"。
            PatchGetter(typeof(ZX.Components.CBuildable), "get_BuildingFactor", "BuildPostfix");
            PatchGetter(typeof(ZX.Components.CBuilder), "get_BuildingFactor", "BuildPostfix");
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
        public static void BuildPostfix(ref float __result)
        {
            if (ON && ModEntry.Cfg.InstantBuild) __result = 10000f;
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
}
