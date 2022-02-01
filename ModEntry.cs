using System;
using System.IO;
using log = DebugLogger.DebugLogger;
using Object = StardewValley.Object;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarawayIsles
{
    public interface IJsonAssetsApi
    {
        int GetObjectId(string name);
        void LoadAssets(string path);
        event EventHandler IdsFixed;
    }

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public static Mod Instance;

        private IJsonAssetsApi JsonAssets;
        public IModEvents ModEvents;

        #region ITEM IDs
        private int SapphireBirchWood_ID = -1;
        #endregion
        #region ITEM SPECIFICATIONS (modData)
        Object Amaranth;
        Object UnluckyClover;
        #endregion

        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            log.DebugLog("");
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JsonAssets = this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (JsonAssets == null)
            {
                log.DebugLog("Can't load Json Assets API, which is needed for test mod to function", LogLevel.Error);
            }
            else
            {
                log.DebugLog("Loading JSON assets...", LogLevel.Info);
                JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", "[JA] Faraway Isles"));
            }

            //SetItemProperties();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            log.DebugLog("Mod has been loaded");
            if (JsonAssets != null)
            {
                SapphireBirchWood_ID = JsonAssets.GetObjectId("Sapphire Birch Wood");
                if (SapphireBirchWood_ID == -1)
                {
                    Monitor.Log("ID for Sapphire Birch Wood not found.", LogLevel.Warn);
                }
                else
                {
                    Monitor.Log($"{JsonAssets} ID is {SapphireBirchWood_ID}.", LogLevel.Info);
                }
            }
        }

        private void SetItemProperties()
        {
            (new Object(300, 1)).modData.Add("isMedicinal", "true");
            if (JsonAssets != null)
            {
                SapphireBirchWood_ID = JsonAssets.GetObjectId("Sapphire Birch Wood");
                if (SapphireBirchWood_ID == -1)
                {
                    Monitor.Log("ID for Sapphire Birch Wood not found.", LogLevel.Warn);
                }
                else
                {
                    Monitor.Log($"{JsonAssets} ID is {SapphireBirchWood_ID}.", LogLevel.Info);
                }
            }
        }
    }
}