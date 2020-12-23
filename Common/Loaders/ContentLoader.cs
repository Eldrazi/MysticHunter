#region Using directives

using System;

using Terraria.ModLoader;

using MysticHunter.API.Ext;
using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Common.Loaders
{
	internal static class ContentLoader
	{
		internal static void LoadModContent(Mod mod)
		{
			foreach (var soul in mod.GetAllSouls())
			{
				SoulManager.AddSoul((BaseSoul)Activator.CreateInstance(soul));
			}
		}

		// TODO: Eldrazi - Do we want to be able to unload specific mod content?
		internal static void UnloadModContent(Mod mod)
		{

		}
	}
}
