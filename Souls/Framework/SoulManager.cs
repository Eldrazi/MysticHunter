using System;
using System.Collections.Generic;

using Terraria.ID;

using MysticHunter.API.Ext;

using MysticHunter.Souls.Data.HM;
using MysticHunter.Souls.Data.Event;
using MysticHunter.Souls.Data.Pre_HM;
using MysticHunter.Souls.Data.Bosses;

namespace MysticHunter.Souls.Framework
{
	public class SoulManager
	{
		/// <summary>
		/// Load in all <see cref="BaseSoul"/> objects from the <see cref="MysticHunter"/> assembly.
		/// </summary>
		public static void LoadSouls()
		{
			MysticHunter.Instance.SoulDict = new Dictionary<short, BaseSoul>();

			/// Load in all <see cref="MysticHunter"/> <see cref="PreHMSoul"/> instances.
			foreach (var s in ReflectUtils.GetPreHMSouls(MysticHunter.Instance))
			{
				AddSoul((PreHMSoul)Activator.CreateInstance(s));
			}

			/// Load in all <see cref="MysticHunter"/> <see cref="PostHMSoul"/> instances.
			foreach (var s in ReflectUtils.GetPostHMSouls(MysticHunter.Instance))
			{
				AddSoul((PostHMSoul)Activator.CreateInstance(s));
			}
		}

		/// <summary>
		/// Unload all souls from <see cref="MysticHunter.SoulDict"/>.
		/// </summary>
		public static void UnloadSouls()
			=> MysticHunter.Instance.SoulDict.Clear();

		/// <summary>
		/// Tries to add the <paramref name="data"/> to the <see cref="MysticHunter.SoulDict"/> dictionary.
		/// </summary>
		/// <param name="data">The <see cref="BaseSoul"/> to add to the dictionary.</param>
		/// <param name="alternateKeys">If set will try to add more keys to the same <paramref name="data"/> value.</param>
		public static void AddSoul(BaseSoul data, short[] alternateKeys = null)
		{
			AddSoulWithKey(data.soulNPC, data);

			short[] additionalTypes = data.GetAdditionalTypes();
			for (int i = 0; i < additionalTypes?.Length; ++i)
				AddSoulWithKey(additionalTypes[i], data);
		}
		private static bool AddSoulWithKey(short key, BaseSoul data)
		{
			try
			{
				MysticHunter.Instance.SoulDict.Add(key, data);
				return (true);
			}
			catch
			{
				MysticHunter.Instance.Logger.Warn("ID '" + data.soulNPC + "' - from class '" + data.GetType().ToString() + "' already exists under name '" + MysticHunter.Instance.SoulDict[data.soulNPC].SoulNPCName() + "'.");
				return (false);
			}
		}

		public static BaseSoul GetSoul(short soulNPC)
			=> MysticHunter.Instance.SoulDict.TryGetValue(soulNPC, out BaseSoul result) ? result : null;

		public static void ReloadSoulIndexUI()
			=> MysticHunter.Instance.soulIndexUI.soulIndexPanel.soulListPanel.soulList.ReloadList();
	}
}
