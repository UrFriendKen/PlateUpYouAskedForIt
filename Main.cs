﻿using Kitchen;
using KitchenData;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.References;
using KitchenMods;
using PreferenceSystem;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YouAskedForIt.Customs;

// Namespace should have "Kitchen" in the beginning
namespace YouAskedForIt
{
    public class Main : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.YouAskedForIt";
        public const string MOD_NAME = "You Asked For It!";
        public const string MOD_VERSION = "0.1.1";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.6";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        public static AssetBundle Bundle;

        internal static PreferenceSystemManager PrefManager;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        public const string SERVING_BOARD_WASHING_ID = "servingBoardWashing";
        static ServingBoardDirty _servingBoardDirty;

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            _servingBoardDirty = AddGameDataObject<ServingBoardDirty>();
            AddGameDataObject<KuluBin>();

            LogInfo("Done loading game data.");
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            LogInfo("Attempting to load asset bundle...");
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            LogInfo("Done loading asset bundle.");

            // Register custom GDOs
            AddGameData();

            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PrefManager
                .AddLabel("Serving Board Requires Washing")
                .AddInfo("Requires restart to take effect")
                .AddOption<bool>(
                    SERVING_BOARD_WASHING_ID,
                    true,
                    new bool[] { false, true },
                    new string[] { "Disabled", "Enabled" });

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);

            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
                if (PrefManager.Get<bool>(SERVING_BOARD_WASHING_ID))
                {
                    foreach (Item item in args.gamedata.Get<Item>())
                    {
                        if ((item.DirtiesTo?.ID ?? 0) == ItemReferences.ServingBoard)
                        {
                            item.DirtiesTo = _servingBoardDirty.GameDataObject;
                        }
                    }
                }
            };
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}