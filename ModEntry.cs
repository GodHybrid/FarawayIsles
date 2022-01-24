using System;
using System.IO;
using log = DebugLogger.DebugLogger;
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

        #region ITEM IDs
        private int SapphireBirchWood_ID = -1;
        #endregion

        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            log.DebugLog("");
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
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

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JsonAssets = this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (JsonAssets == null)
            {
                log.DebugLog("Can't load Json Assets API, which is needed for test mod to function", LogLevel.Error);
            }
            else
            {
                JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets/[JA] Faraway Isles"));
            }
            //JsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
            //JsonAssets.IdsFixed += this.OnIdsFixed;
        }

        private void OnIdsFixed(object sender, EventHandler e)
        { 

        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            log.DebugLog("Mod has been loaded");
        }
    }
}