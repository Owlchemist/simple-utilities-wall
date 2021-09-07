using Verse;
using System.Collections.Generic;

namespace WallUtilities
{
	public class WorkPlacer_OnlyOnFence : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{		
			//Try to determine if this is a workbench that deals with food
			IEnumerable<Thing> building = map.thingGrid.ThingsAt(loc);
			foreach (var thingHere in building)
			{
				if (thingHere.def.building != null && thingHere.def.building.isFence) return true;
			}

			return new AcceptanceReport("Must be placed along a fence");
		}

		public WorkPlacer_OnlyOnFence()
		{
		}
	}
}
