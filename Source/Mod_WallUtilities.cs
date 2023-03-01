using Verse;
using HarmonyLib;
 
namespace WallUtilities
{
    public class Mod_WallUtilities : Mod
	{
		public Mod_WallUtilities(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
		}
	}
}