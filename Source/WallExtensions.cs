
using Verse;
using HarmonyLib;

namespace WallUtilities
{
	[StaticConstructorOnStartup]
	public static class WallExtensions
	{
		static WallExtensions()
		{
			new Harmony("owlchemist.wallutilities").PatchAll();
		}
		public static bool IsWall(this IntVec3 loc, Map map, Rot4 rot)
		{
			var facingCell = loc + rot.FacingCell;
			if (!loc.InBounds(map) || !facingCell.InBounds(map)) return false;
			
			//This cell should be free of any walls
			var edifice = (facingCell).GetEdifice(map);
			if (edifice != null && (edifice.def.defName.ToLower().Contains("wall") || edifice.def.IsSmoothed || edifice.def.fillPercent == 1f)) return false;

			//Check the placement cell itself
			edifice = loc.GetEdifice(map);
			if (edifice != null && (edifice.def.defName.ToLower().Contains("wall") || edifice.def.IsSmoothed)) return true;

			return false;
		}
	}
}