using System;
using System.IO;
using log = DebugLogger.DebugLogger;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace FarawayIsles.logic
{
	class VolcanicHive : StardewValley.Object
	{
		public float timePenalty = 2f;

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
			if (this.name.Equals("Volcanic Hive"))
				if (Game1.GetSeasonForLocation(who.currentLocation).Equals("summer"))
				{
					this.heldObject.Value = new VolcanicHive();//new StardewValley.Object(Vector2.Zero, 340, null, canBeSetDown: true, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
				}
				else
				{
					this.heldObject.Value = new StardewValley.Object(Vector2.Zero, 340, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
					return false;
				}

				if (this.readyForHarvest)
				{
					if (justCheckingForActivity)
					{
						return true;
					}
					if (who.isMoving())
					{
						Game1.haltAfterCheck = false;
					}

					bool check_for_reload = false;
					{
						int hiveRad = 7;
						bool? isPure = null;
						int honey_type = -1;
						string honeyName = "Muddy";
						int honeyPriceAddition = 0;

						List<Crop> cl = getAllCropsInRadius(this.TileLocation, hiveRad);
					
						if (cl != null && cl.Count > 0)
						{
							Dictionary<Crop, int> sortCrops = new Dictionary<Crop, int>(cl.Count);
						
							foreach (Crop crop in cl)
							{
								if (!sortCrops.Keys.Contains<Crop>(crop)) sortCrops.Add(crop, 1);
								else sortCrops[crop]++;
							}
							sortCrops = (from entry in sortCrops orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);

							#region timePenalty evaluation
							{
								float totalPoints = 0f;
								foreach (var pair in sortCrops)
								{
									if (new StardewValley.Object(pair.Key.indexOfHarvest.Value, 1).Category == -80) { totalPoints += 0.20f * pair.Value; continue; }
									if (new StardewValley.Object(pair.Key.indexOfHarvest.Value, 1).Category == -79) { totalPoints += 0.16f * pair.Value; continue; }
									if (new StardewValley.Object(pair.Key.indexOfHarvest.Value, 1).Category == -75) { totalPoints += 0.08f * pair.Value; continue; }
									if (new StardewValley.Object(pair.Key.indexOfHarvest.Value, 1).Category == -81) { totalPoints += 0.07f * pair.Value; continue; }
								}
								this.minutesUntilReady.Value = (int)(Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4) * timePenalty/totalPoints);
							}
							#endregion

							if (sortCrops.Count == 1 || sortCrops.ElementAt(0).Value / sortCrops.ElementAt(1).Value > 4f)
							{
								if (sortCrops.ElementAt(0).Key.indexOfHarvest == 300) honeyName = "Pure Medicinal " + Game1.objectInformation[sortCrops.ElementAt(0).Key.indexOfHarvest].Split('/')[0];
								else honeyName = "Pure " + Game1.objectInformation[sortCrops.ElementAt(0).Key.indexOfHarvest].Split('/')[0];
								honey_type = sortCrops.ElementAt(0).Key.indexOfHarvest.Value;
								isPure = true;
							}
							else
							{
								if (sortCrops.ElementAt(0).Key.indexOfHarvest == 300 && sortCrops.ElementAt(0).Value / sortCrops.ElementAt(1).Value > 1.5f) 
									honeyName = "Wild Medicinal " + Game1.objectInformation[sortCrops.ElementAt(0).Key.indexOfHarvest].Split('/')[0] + "-"
																	+ Game1.objectInformation[sortCrops.ElementAt(1).Key.indexOfHarvest].Split('/')[0];
								else honeyName = "Wild " + Game1.objectInformation[sortCrops.ElementAt(0).Key.indexOfHarvest].Split('/')[0] + "-"
																	+ Game1.objectInformation[sortCrops.ElementAt(1).Key.indexOfHarvest].Split('/')[0];
								honey_type = sortCrops.ElementAt(0).Key.indexOfHarvest.Value;
								isPure = false;
							}

							honey_type = sortCrops.ElementAt(0).Key.indexOfHarvest.Value;
							if (!(isPure ?? false))
							{
								float priceTotal = 0f;
								for(int i = 0; i < sortCrops.Count; i++)
								{
									priceTotal += Convert.ToInt32(Game1.objectInformation[sortCrops.ElementAt(i).Key.indexOfHarvest].Split('/')[1]) * (sortCrops.ElementAt(i).Value / cl.Count);
								}
								honeyPriceAddition = (int)(priceTotal * 1.1f);
							}
							else honeyPriceAddition = (int)(Convert.ToInt32(Game1.objectInformation[sortCrops.ElementAt(0).Key.indexOfHarvest].Split('/')[1]) * 1.2f);

						}
						if (this.heldObject.Value != null)
						{
							this.heldObject.Value.name = honeyName + " Honey";
							this.heldObject.Value.displayName = this.loadDisplayName();
							this.heldObject.Value.Price = Convert.ToInt32(Game1.objectInformation[340].Split('/')[1]) + honeyPriceAddition;
							this.heldObject.Value.preservedParentSheetIndex.Value = honey_type;
						
							if (who.IsLocalPlayer)
							{
								StardewValley.Object item = this.heldObject.Value;
								this.heldObject.Value = null;
								if (!who.addItemToInventoryBool(item))
								{
									this.heldObject.Value = item;
									Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
									return false;
								}
							}
							Game1.playSound("coin");
							check_for_reload = true;
						}
					}
				}
				return false;
		}

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
			if ((int)this.minutesUntilReady <= 0)
			{
				if (!this.readyForHarvest)
				{
					environment.playSound("dwop");
				}
				this.readyForHarvest.Value = true;
				this.minutesUntilReady.Value = 0;
				this.onReadyForHarvest(environment);
				this.showNextIndex.Value = true;
			}
			return false;
		}

		public List<Crop> getAllCropsInRadius(Vector2 location, int range = 1, Func<Crop, bool> additional_check = null)
		{
			HashSet<Vector2> coordsInRadius = new HashSet<Vector2>();
			List<Crop> CropList = new List<Crop>();
			
			for (int i = range; i >= -range; i--)
			{
				for (int j = (range - Math.Abs(i)); j >= -(range - Math.Abs(i)); j--)
				{
					int tmpX = (int)location.X - i;
					int tmpY = (int)location.Y - j;
					coordsInRadius.Add(new Vector2(tmpX > 0 ? tmpX : 0, tmpY > 0 ? tmpY : 0));
				}
			}

			{
				GameLocation cropPlacement = new GameLocation();
				foreach (Vector2 t in coordsInRadius)
				{
					if (cropPlacement.terrainFeatures[t] is HoeDirt 
						&& (cropPlacement.terrainFeatures[t] as HoeDirt).crop != null
						&& (int)(cropPlacement.terrainFeatures[t] as HoeDirt).crop.currentPhase >= (cropPlacement.terrainFeatures[t] as HoeDirt).crop.phaseDays.Count - 1 
						&& !(cropPlacement.terrainFeatures[t] as HoeDirt).crop.dead
						&& (new StardewValley.Object((cropPlacement.terrainFeatures[t] as HoeDirt).crop.indexOfHarvest.Value, 1).Category is >= -81 and <= -75))
					{
						CropList.Add((cropPlacement.terrainFeatures[t] as HoeDirt).crop);
					}
				}
			}

			return CropList;
		}
	}
}
