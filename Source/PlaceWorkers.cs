using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;

namespace WallUtilities
{
	public class PlaceWorker_OnlyNextToWall : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			//Check roof
			if (map.roofGrid.Roofed(loc) == false) return new AcceptanceReport("WallUtilities.Report.NoRoof".Translate());

			//Check walls/doors or other heaters
			Building building = map.thingGrid.ThingAt<Building>(loc);
			if (building != null && (building.def.fillPercent == 1f || building.def.thingClass == typeof(Building_Heater))) return new AcceptanceReport("WallUtilities.Report.FreeSpace".Translate());

			//Check blueprint
			if (map.thingGrid.ThingAt<Blueprint>(loc) != null) return new AcceptanceReport("WallUtilities.Report.Blueprints".Translate());
			
			//Determine other cell coords
			//Try to determine if this is a wall. We used to do this by assuming wall-like properties but it didn't play well with custom classes.
			//So now we use VE Framework's way about trying to determine this.
			var edifice = (loc + IntVec3.North.RotatedBy(rot)).GetEdifice(map);
			if (edifice != null && ((edifice?.def.defName.ToLower().Contains("wall") ?? false) || edifice.def.IsSmoothed)) return true;

			return new AcceptanceReport("WallUtilities.Report.ByWall".Translate());
		}

		public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
		{
			GenDraw.DrawFieldEdges(new List<IntVec3>{center}, GenTemperature.ColorSpotHot, null);
		}
	}

	public class PlaceWorker_OnlyOnFence : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{		
			foreach (Thing thingHere in map.thingGrid.ThingsListAtFast(loc))
			{
				if (thingHere.def.building?.isFence ?? false) return true;
			}

			return new AcceptanceReport("WallUtilities.Report.OnFence".Translate());
		}
	}
}