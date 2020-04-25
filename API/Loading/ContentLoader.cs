using System;

using Terraria.ModLoader;

using MysticHunter.API.Ext;
using MysticHunter.Souls.Framework;

namespace MysticHunter.API.Loading
{
	internal static class ContentLoader
	{
		internal static void LoadModContent(Mod mod)
		{
			foreach (var soul in ReflectUtils.GetPreHMSouls(mod))
			{
				SoulManager.AddSoul((BaseSoul)Activator.CreateInstance(soul));
			}

			/*foreach (var soul in ReflectUtils.GetPostHMSouls(mod))
			{
				SoulManager.AddSoul((BaseSoul)Activator.CreateInstance(soul));
			}*/
		}

		// TODO: Eldrazi - Do we want to be able to unload specific mod content?
		internal static void UnloadModContent(Mod mod)
		{

		}
	}
}
