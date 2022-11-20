/*
using Verse;
using HarmonyLib;
using VanillaFurnitureExpanded;
using System.Linq;
using System.Collections.Generic;

namespace WallUtilities
{
	//If a wall mounted thing spawns without a wall, this sanity check patch will replace the wall.
	[HarmonyPatch(typeof(MapGenerator), nameof(MapGenerator.GenerateMap))]
	public class Patch_MapGenerator
	{
		static public void Postfix(ref Map __result)
		{
			Map map = __result;
			//Apparently the thing listers aren't ready yet at this point, so gotta use the grid instead?
			List<Thing> list = __result.thingGrid.thingGrid.SelectMany(x => x).Where(y => y.def?.HasComp(typeof(CompMountableOnWall)) ?? false).ToList();

			bool wasFixed = false;
			foreach (var parent in list)
			{
				if (parent == null) return;
				IntVec3 ventCell = parent.Position;
				
				if (ventCell == null || map == null) return;
				Building edifice = ventCell.GetEdifice(map);

				//There's a problem if null
				if (edifice == null)
				{
					var cells = parent.OccupiedRect().AdjacentCellsCardinal.ToList();
					foreach (IntVec3 cell in cells)
					{
						if (cell.InBounds(map))
						{
							Building newWall = cell.GetEdifice(map);
							if (newWall?.def?.defName?.ToLower().Contains("wall") == true || (newWall?.def.IsSmoothed ?? false))
							{
								Thing replacement = ThingMaker.MakeThing(newWall.def, newWall.Stuff);
								GenPlace.TryPlaceThing(replacement, ventCell, map, ThingPlaceMode.Direct);
								replacement.SetFaction(newWall.Faction);
								if (Prefs.DevMode) Log.Message("[Vanilla Expanded Framework] Found a missing wall underneath " + parent.ThingID + 
									" at " + parent.Position.ToString() + ", placing a replacement " + replacement.def.defName + " down...");

								//We're done here
								wasFixed = true;
								break;
							}
						}
					}
				}
			}
			if (wasFixed) map.mapDrawer.RegenerateEverythingNow();
		}
	}
}
*/