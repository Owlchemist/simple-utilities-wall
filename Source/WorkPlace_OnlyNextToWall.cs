using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;

namespace WallUtilities
{
	public class WorkPlace_OnlyNextToWall : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			//check roof
			if (map.roofGrid.Roofed(loc) == false) return new AcceptanceReport("Space in front of heater must be under a roof.");

			//check walls/doors or other heaters
			Building building = map.thingGrid.ThingAt<Building>(loc);
			if (building != null && (building.def.fillPercent == 1f || building.def.thingClass == typeof(Building_Heater))) return new AcceptanceReport("Space in front of heater must not be a wall, door, or other heater.");
			//check blueprint
			if (map.thingGrid.ThingAt<Blueprint>(loc) != null) return new AcceptanceReport("Cannot be placed on blueprint.");
			
			//Determine other cell coords
			IntVec3 loc2 = loc;
			if (rot.AsInt == 0) loc2.z++;
			else if (rot.AsInt == 1) loc2.x++;
			else if (rot.AsInt == 2) loc2.z--;
			else loc2.x--;
				
			//try to determine if this is a wall. There isn't a wall type so we check for some assumed values.
			IEnumerable<Thing> building2 = map.thingGrid.ThingsAt(loc2);
			foreach (Thing thinghere in building2)
			{
				if (thinghere?.def.thingClass == typeof(Building))
				{
					if (thinghere != null && (thinghere.def.fillPercent == 1f || thinghere.def.passability == Traversability.Impassable || thinghere.def.blockLight == true)) return true;
				}
			}

			return new AcceptanceReport("Must be placed next to wall.");
		}

		public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
		{
			GenDraw.DrawFieldEdges(new List<IntVec3>{center}, GenTemperature.ColorSpotHot, null);
		}

		public WorkPlace_OnlyNextToWall()
		{
		}
	}
}
