using Verse;
using RimWorld;
using HarmonyLib;

namespace WallUtilities
{
	public class WallMountable : DefModExtension { }
	public class WallMountedGlower : DefModExtension { }

	[HarmonyPatch(typeof(Building), nameof(Building.Destroy))]
	public static class Patch_Building_Destroy
	{
		public static void Prefix(Building __instance)
		{
			var def = __instance.def;
			if (def == null) return;

			Map map = __instance.Map;
			if (map == null) return;

			if (def.building?.isFence ?? false) CheckFence();
			else CheckWall();

			//Embedded methods
			void CheckFence()
			{
				var things = map.thingGrid.ThingsListAtFast(__instance.Position);
				for (int i = things.Count; i-- > 0;)
				{
					var thing = things[i];
					if (thing.def.placeWorkers?.Contains(typeof(PlaceWorker_OnlyOnFence)) ?? false )
					{
						thing.Destroy();
						return;
					}
				}	
			}
			void CheckWall()
			{
				if (def.passability != Traversability.Impassable || def.HasModExtension<WallMountable>()) return; //Avoid recursive loop
			
				var things = map.thingGrid.ThingsListAtFast(__instance.Position);
				for (int i = things.Count; i-- > 0;)
				{
					var thing = things[i];
					if (thing.def.HasModExtension<WallMountable>())
					{
						thing.Destroy();
						return;
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.BlocksConstruction))]
	public static class Patch_GenConstruct_BlocksConstruction
	{
		public static bool Postfix(bool __result, Thing constructible)
		{
			if (!__result) return __result;

			ThingDef thingDef = constructible is Blueprint ? constructible.def : constructible is Frame ? constructible.def.entityDefToBuild.blueprintDef : constructible.def.blueprintDef;
			return thingDef.entityDefToBuild == null || !thingDef.entityDefToBuild.HasModExtension<WallMountable>();
		}
	}

	//TODO: Convert these two to transpilers, probably.
	[HarmonyPatch(typeof(GlowGrid), nameof(GlowGrid.RegisterGlower))]
	public static class Patch_GlowGrid_RegisterGlower
	{
		public static bool Prefix(GlowGrid __instance, CompGlower newGlow)
		{
			if (!newGlow.parent.def.HasModExtension<WallMountedGlower>()) return true;

			var offSetCell = newGlow.parent.Position + newGlow.parent.Rotation.FacingCell;
			
			__instance.litGlowers.Add(newGlow);
			__instance.MarkGlowGridDirty(offSetCell);
			if (Current.ProgramState != ProgramState.Playing)
			{
				__instance.initialGlowerLocs.Add(offSetCell);
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(GlowGrid), nameof(GlowGrid.DeRegisterGlower))]
	public static class Patch_GlowGrid_DeRegisterGlower
	{
		public static bool Prefix(GlowGrid __instance, CompGlower oldGlow)
		{
			if (!oldGlow.parent.def.HasModExtension<WallMountedGlower>()) return true;

			__instance.litGlowers.Remove(oldGlow);
			__instance.MarkGlowGridDirty(oldGlow.parent.Position + oldGlow.parent.Rotation.FacingCell);
			
			return false;
		}
	}

	//When the AddFloodGlowFor brings in the CompGlower, it only cares about the props and the parent's position. We make a dummy, copy the props over and use a dummy parent location.
	//Both instances should get picked up by the garbage collector later as it's not referenced anywhere
	//The alternative to this is to have a permanent dummy entity that remains in play, which seems like an even dirtier hack than this is
	[HarmonyPatch(typeof(GlowFlooder), nameof(GlowFlooder.AddFloodGlowFor))]
	public static class Patch_GlowFlooder_AddFloodGlowFor
	{
		public static void Prefix(ref CompGlower theGlower)
		{
			if (!theGlower.parent.def.HasModExtension<WallMountedGlower>()) return;

			CompGlower dummyGlow = new CompGlower() { props = theGlower.props, glowColorOverride = theGlower.glowColorOverride };
			dummyGlow.parent = new ThingWithComps() { positionInt = theGlower.parent.Position + theGlower.parent.Rotation.FacingCell };
			theGlower = dummyGlow;
		}
	}
}