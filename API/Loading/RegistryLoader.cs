using System;
using System.Collections.Generic;

using Terraria.ModLoader;

namespace MysticHunter.API.Loading
{
	public class RegistryLoader
	{
		internal static Dictionary<string, Mod> Mods;
		public static int ModCount => Mods.Count;

		internal static void Load()
			=> Mods = new Dictionary<string, Mod>();

		internal static void Unload()
		{
			UnloadModContent();
			Mods?.Clear();
			Mods = null;
		}

		internal static bool CheckModRegistered(Mod mod, string source, bool @throw = true)
		{
			bool modIsPresent = Mods.ContainsKey(mod.Name);
			if (modIsPresent && @throw)
			{
				throw new Exception($"Mod {mod.Name} is already registered in {source}");
			}

			return modIsPresent;
		}

		/// <summary>
		/// Registers specified <see cref="Mod"/>, enabling autoloading for that mod.
		/// </summary>
		public static void RegisterMod(Mod mod)
		{
			CheckModRegistered(mod, "RegisterMod");

			Mods[mod.Name] = mod;
		}

		internal static void LoadModContent()
		{
			foreach (var kvp in Mods)
			{
				MysticHunter.Instance.Logger.Info($"RegistryLoader - Loading MysticHunter content for mod '{kvp.Key}'.");
				ContentLoader.LoadModContent(kvp.Value);
			}
		}
		internal static void UnloadModContent()
		{
			MysticHunter.Instance.SoulDict.Clear();
			MysticHunter.Instance.SoulDict = null;
		}
	}
}
