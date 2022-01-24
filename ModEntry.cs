using System;
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

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.Monitor.Log($"Mod has been loaded");
        }
    }
}