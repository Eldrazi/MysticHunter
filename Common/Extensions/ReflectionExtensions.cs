#region Using directives

using System;
using System.Linq;
using System.Collections.Generic;

using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.API.Ext
{
	internal static class ReflectionExtensions
	{
		/// <summary>
		/// Loads all <see cref="PreHMSoul"/> types from the given <see cref="Mod"/> assembly.
		/// </summary>
		/// <returns>An <see cref="IEnumerable{T}"/> with a list of class types.</returns>
		public static IEnumerable<Type> GetPreHMSouls(this Mod mod)
			=> GetNonAbstractClasses(mod, t => t.IsSubclassOf(typeof(PreHMSoul)));

		/// <summary>
		/// Loads all <see cref="PostHMSoul"/> types from the given <see cref="Mod"/> assembly.
		/// </summary>
		/// <returns>An <see cref="IEnumerable{T}"/> with a list of class types.</returns>
		public static IEnumerable<Type> GetPostHMSouls(this Mod mod)
			=> GetNonAbstractClasses(mod, t => t.IsSubclassOf(typeof(PostHMSoul)));

		/// <summary>
		/// Loads all <see cref="BaseSoul"/> types from the given <see cref="Mod"/> assembly.
		/// </summary>
		/// <returns>An <see cref="IEnumerable{T}"/> with a list of class types.</returns>
		public static IEnumerable<Type> GetAllSouls(this Mod mod)
			=> GetNonAbstractClasses(mod, t => t.IsSubclassOf(typeof(BaseSoul)));

		public static IEnumerable<Type> GetNonAbstractClasses(Mod loadMod, Func<Type, bool> func = null)
			=> loadMod.Code.GetTypes().Where(t => t.IsClass && !t.IsAbstract && (func?.Invoke(t) ?? true));
	}
}
