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
			//Try to determine if this is a wall. There isn't a wall type so we check for some assumed values.
			foreach (Thing thinghere in map.thingGrid.ThingsListAtFast(loc + IntVec3.North.RotatedBy(rot)))
			{
				string type = thinghere?.def.thingClass.Name;
				if
				(
					(type == nameof(Building) || type == nameof(Mineable) || type == "GL_Building") &&
				 	(thinghere.def.fillPercent == 1f || thinghere.def.passability == Traversability.Impassable || thinghere.def.blockLight == true)
				) return true;
			}

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