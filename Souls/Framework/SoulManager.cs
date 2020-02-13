using System;
using System.Linq;
using System.Collections.Generic;

using MysticHunter.Souls.Data;

namespace MysticHunter.Souls.Framework
{
	public class SoulManager
	{
		public static void SetupSouls()
		{
			MysticHunter.Instance.SoulDict = new Dictionary<short, ISoul>();

			// Pre-hardmode souls.
			AddNewSoul(new AntlionSwarmerSoul());
			AddNewSoul(new BabySlimeSoul());
			AddNewSoul(new BeeSoul());
			AddNewSoul(new BlackSlimeSoul());
			AddNewSoul(new CrabSoul());
			AddNewSoul(new DarkCasterSoul());
			AddNewSoul(new DemonSoul());
			AddNewSoul(new DoctorBonesSoul());
			AddNewSoul(new DungeonGuardianSoul());
			AddNewSoul(new DungeonSlimeSoul());
			AddNewSoul(new GoblinScoutSoul());
			AddNewSoul(new GraniteElementalSoul());
			AddNewSoul(new GraniteGolemSoul());
			AddNewSoul(new HarpySoul());
			AddNewSoul(new IceSlimeSoul());
			AddNewSoul(new MushiLadybugSoul());
			AddNewSoul(new NymphSoul());
			AddNewSoul(new SquidSoul());
			AddNewSoul(new TimSoul());
			AddNewSoul(new UndeadVikingSoul());
			AddNewSoul(new VoodooDemonSoul());

			// Hardmode souls.
			AddNewSoul(new ArmoredVikingSoul());

			// Boss souls.
			AddNewSoul(new KingSlimeSoul());
		}

		public static void ResetSoulAcquisition(List<short> acquiredSouls = null)
		{
			MysticHunter.Instance.SoulDict.Values.Select(v => v.acquired = false);

			if (acquiredSouls != null)
				acquiredSouls.ForEach(v => MysticHunter.Instance.SoulDict[v].acquired = true);
		}

		public static void RepopulateSoulIndexUI()
		{
			MysticHunter.Instance.soulIndexUI.soulIndexPanel.soulListPanel.soulList.RepopulateList();
		}

		private static void AddNewSoul(ISoul data)
		{
			if (MysticHunter.Instance.SoulDict.ContainsKey(data.soulNPC))
			{
				MysticHunter.Instance.Logger.Warn("ERROR: ID '" + data.soulNPC + "' - from class '" + data.GetType().ToString() + "' already exists under name '" + MysticHunter.Instance.SoulDict[data.soulNPC].soulName + "'.");
				return;
			}
			MysticHunter.Instance.SoulDict.Add(data.soulNPC, data);
		}
	}
}
