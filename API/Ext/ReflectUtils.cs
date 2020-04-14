using System;
using System.Linq;
using System.Collections.Generic;

using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

namespace MysticHunter.API.Ext
{
	internal static class ReflectUtils
	{
		/// <summary>
		/// Loads all PreHMSouls from the <see cref="MysticHunter"/> assembly instance.
		/// </summary>
		/// <returns>An <see cref="IEnumerable{T}"/> with a list of class types.</returns>
		public static IEnumerable<Type> GetPreHMSouls(Mod mod)
			=> GetNonAbstractClasses(mod, t => t.IsSubclassOf(typeof(PreHMSoul)));
		
		/// <summary>
		/// Loads all PostHMSouls from the <see cref="MysticHunter"/> assembly instance.
		/// </summary>
		/// <returns>An <see cref="IEnumerable{T}"/> with a list of class types.</returns>
		public static IEnumerable<Type> GetPostHMSouls(Mod mod)
			=> GetNonAbstractClasses(mod, t => t.IsSubclassOf(typeof(PostHMSoul)));

		public static IEnumerable<Type> GetNonAbstractClasses(Mod loadMod, Func<Type, bool> func = null)
			=> loadMod.Code.GetTypes().Where(t => t.IsClass && !t.IsAbstract && (func?.Invoke(t) ?? true));
	}
}
