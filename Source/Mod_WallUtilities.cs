using HarmonyLib;
using Verse;

namespace WallUtilities
{
	public class WallUtilitiesMod : Mod
	{	
		public WallUtilitiesMod(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
		}
    }
}