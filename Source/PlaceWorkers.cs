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
			if (building != null && (building.def.fillPercent == 1f || building is Building_Heater)) return new AcceptanceReport("WallUtilities.Report.FreeSpace".Translate());

			//Check blueprint
			if (map.thingGrid.ThingAt<Blueprint>(loc) != null) return new AcceptanceReport("WallUtilities.Report.Blueprints".Translate());
			
			//Determine other cell coords
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
			var list = map.thingGrid.ThingsListAtFast(loc);
			for (int i = list.Count; i-- > 0;)
			{
				var thingHere = list[i];
				if (thingHere.def.building?.isFence ?? false) return true;
			}

			return new AcceptanceReport("WallUtilities.Report.OnFence".Translate());
		}
	}

	public class PlaceWorker_OnlyOnWall : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{		
			if (loc.IsWall(map, rot)) return true;

			return new AcceptanceReport("WallUtilities.Report.OnWall".Translate());
		}
	}
}