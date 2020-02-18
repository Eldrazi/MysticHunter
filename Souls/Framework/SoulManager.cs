﻿using System;
using System.Linq;
using System.Collections.Generic;

using MysticHunter.Souls.Data.HM;
using MysticHunter.Souls.Data.Pre_HM;

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
			AddNewSoul(new BlueSlimeSoul());
			AddNewSoul(new BoneSerpentSoul());
			AddNewSoul(new CochinealBeetleSoul());
			AddNewSoul(new CrabSoul());
			AddNewSoul(new CursedSkullSoul());
			AddNewSoul(new CyanBeetleSoul());
			AddNewSoul(new DarkCasterSoul());
			AddNewSoul(new DemonSoul());
			AddNewSoul(new DemonEyeSoul());
			AddNewSoul(new DevourerSoul());
			AddNewSoul(new DoctorBonesSoul());
			AddNewSoul(new DungeonGuardianSoul());
			AddNewSoul(new DungeonSlimeSoul());
			AddNewSoul(new FireImpSoul());
			AddNewSoul(new FungiBulbSoul());
			AddNewSoul(new GiantWormSoul());
			AddNewSoul(new GoblinScoutSoul());
			AddNewSoul(new GraniteElementalSoul());
			AddNewSoul(new GraniteGolemSoul());
			AddNewSoul(new HarpySoul());
			AddNewSoul(new HellbatSoul());
			AddNewSoul(new HopliteSoul());
			AddNewSoul(new IceBatSoul());
			AddNewSoul(new IceSlimeSoul());
			AddNewSoul(new JungleBatSoul());
			AddNewSoul(new JungleSlimeSoul());
			AddNewSoul(new LacBeetleSoul());
			AddNewSoul(new LavaSlimeSoul());
			AddNewSoul(new ManEaterSoul());
			AddNewSoul(new MushiLadybugSoul());
			AddNewSoul(new SandSlimeSoul());
			AddNewSoul(new NymphSoul());
			AddNewSoul(new PinkySoul());
			AddNewSoul(new PurpleSlimeSoul());
			AddNewSoul(new SquidSoul());
			AddNewSoul(new TimSoul());
			AddNewSoul(new TombCrawlerSoul());
			AddNewSoul(new UndeadVikingSoul());
			AddNewSoul(new VoodooDemonSoul());

			// Hardmode souls.
			AddNewSoul(new AngryTrapperSoul());
			AddNewSoul(new ArmoredVikingSoul());
			AddNewSoul(new BasiliskSoul());
			AddNewSoul(new DreamerGhoulSoul());
			AddNewSoul(new FloatyGrossSoul());
			AddNewSoul(new HellArmoredBonesSoul());
			AddNewSoul(new IceElementalSoul());
			AddNewSoul(new NecromancerSoul());
			AddNewSoul(new RustyArmoredBonesSoul());
			AddNewSoul(new TaintedGhoulSoul());
			AddNewSoul(new UnicornSoul());
			AddNewSoul(new VileGhoulSoul());
			AddNewSoul(new WyvernSoul());

			// Boss souls.
			AddNewSoul(new KingSlimeSoul());
			AddNewSoul(new DarkMageSoul());
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
				MysticHunter.Instance.Logger.Warn("ID '" + data.soulNPC + "' - from class '" + data.GetType().ToString() + "' already exists under name '" + MysticHunter.Instance.SoulDict[data.soulNPC].SoulNPCName() + "'.");
				return;
			}
			MysticHunter.Instance.SoulDict.Add(data.soulNPC, data);
		}
	}
}
