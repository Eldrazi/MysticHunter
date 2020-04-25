namespace MysticHunter.Souls.Framework
{
	public class SoulManager
	{
		/// <summary>
		/// Tries to add the <paramref name="data"/> to the <see cref="MysticHunter.SoulDict"/> dictionary.
		/// </summary>
		/// <param name="data">The <see cref="BaseSoul"/> to add to the dictionary.</param>
		/// <param name="alternateKeys">If set will try to add more keys to the same <paramref name="data"/> value.</param>
		public static void AddSoul(BaseSoul data)
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
