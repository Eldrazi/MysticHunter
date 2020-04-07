using System.Linq;
using System.Collections.Generic;

using MysticHunter.Souls.Data.HM;
using MysticHunter.Souls.Data.Event;
using MysticHunter.Souls.Data.Pre_HM;
using MysticHunter.Souls.Data.Bosses;

namespace MysticHunter.Souls.Framework
{
	public class SoulManager
	{
		public static void SetupSouls()
		{
			MysticHunter.Instance.SoulDict = new Dictionary<short, BaseSoul>();

			// Pre-hardmode souls.
			AddNewSoul(new AngryBonesSoul());
			AddNewSoul(new AnomuraFungusSoul());
			AddNewSoul(new AntlionSoul());
			AddNewSoul(new AntlionChargerSoul());
			AddNewSoul(new AntlionSwarmerSoul());
			AddNewSoul(new BabySlimeSoul());
			AddNewSoul(new BeeSoul());
			AddNewSoul(new BlackSlimeSoul());
			AddNewSoul(new BloodCrawlerSoul());
			AddNewSoul(new BlueJellyfishSoul());
			AddNewSoul(new BlueSlimeSoul());
			AddNewSoul(new BoneSerpentSoul());
			AddNewSoul(new CaveBatSoul());
			AddNewSoul(new CochinealBeetleSoul());
			AddNewSoul(new CrabSoul());
			AddNewSoul(new CrawdadSoul());
			AddNewSoul(new CrimeraSoul());
			AddNewSoul(new CursedSkullSoul());
			AddNewSoul(new CyanBeetleSoul());
			AddNewSoul(new DarkCasterSoul());
			AddNewSoul(new DemonSoul());
			AddNewSoul(new DemonEyeSoul());
			AddNewSoul(new DevourerSoul());
			AddNewSoul(new DoctorBonesSoul());
			AddNewSoul(new DungeonGuardianSoul());
			AddNewSoul(new DungeonSlimeSoul());
			AddNewSoul(new EaterOfSoulsSoul());
			AddNewSoul(new FaceMonsterSoul());
			AddNewSoul(new FireImpSoul());
			AddNewSoul(new FungiBulbSoul());
			AddNewSoul(new GiantShellySoul());
			AddNewSoul(new GiantWormSoul());
			AddNewSoul(new GoblinScoutSoul());
			AddNewSoul(new GraniteElementalSoul());
			AddNewSoul(new GraniteGolemSoul());
			AddNewSoul(new GreenSlimeSoul());
			AddNewSoul(new HarpySoul());
			AddNewSoul(new HellbatSoul());
			AddNewSoul(new HopliteSoul());
			AddNewSoul(new HornetSoul());
			AddNewSoul(new IceBatSoul());
			AddNewSoul(new IceSlimeSoul());
			AddNewSoul(new JungleBatSoul());
			AddNewSoul(new JungleSlimeSoul());
			AddNewSoul(new LacBeetleSoul());
			AddNewSoul(new LavaSlimeSoul());
			AddNewSoul(new ManEaterSoul());
			AddNewSoul(new MeteorHeadSoul());
			AddNewSoul(new MotherSlimeSoul());
			AddNewSoul(new MushiLadybugSoul());
			AddNewSoul(new MushroomZombieSoul());
			AddNewSoul(new NymphSoul());
			AddNewSoul(new PinkySoul());
			AddNewSoul(new PinkJellyfishSoul());
			AddNewSoul(new PiranhaSoul());
			AddNewSoul(new PurpleSlimeSoul());
			AddNewSoul(new RavenSoul());
			AddNewSoul(new RedSlimeSoul());
			AddNewSoul(new SalamanderSoul());
			AddNewSoul(new SandSlimeSoul());
			AddNewSoul(new SeaSnailSoul());
			AddNewSoul(new SharkSoul());
			AddNewSoul(new SkeletonSoul());
			AddNewSoul(new SnatcherSoul());
			AddNewSoul(new SnowFlinxSoul());
			AddNewSoul(new SpikedIceSlimeSoul());
			AddNewSoul(new SpikedJungleSlimeSoul());
			AddNewSoul(new SquidSoul());
			AddNewSoul(new TimSoul());
			AddNewSoul(new TombCrawlerSoul());
			AddNewSoul(new UndeadMinerSoul());
			AddNewSoul(new UndeadVikingSoul());
			AddNewSoul(new VoodooDemonSoul());
			AddNewSoul(new YellowSlimeSoul());
			AddNewSoul(new VultureSoul());
			AddNewSoul(new WallCreeperSoul());
			AddNewSoul(new ZombieSoul());
			AddNewSoul(new ZombieEskimoSoul());

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
			AddNewSoul(new SandPoacherSoul());
			AddNewSoul(new TaintedGhoulSoul());
			AddNewSoul(new UnicornSoul());
			AddNewSoul(new VileGhoulSoul());
			AddNewSoul(new WyvernSoul());

			// Event souls.
			AddNewSoul(new GoblinArcherSoul());
			AddNewSoul(new GoblinPeonSoul());
			AddNewSoul(new GoblinSorcererSoul());
			AddNewSoul(new GoblinSummonerSoul());
			AddNewSoul(new GoblinThiefSoul());
			AddNewSoul(new GoblinWarriorSoul());

			// Boss souls.
			AddNewSoul(new BrainOfCthuluSoul());
			AddNewSoul(new DarkMageSoul());
			AddNewSoul(new DestroyerSoul());
			AddNewSoul(new KingSlimeSoul());
		}

		public static void ResetSoulAcquisition(Dictionary<short, byte> acquiredSouls)
		{
			MysticHunter.Instance.SoulDict.Values.Select(v => v.acquired = false);

			if (acquiredSouls != null)
			{
				foreach (var kvp in acquiredSouls)
					MysticHunter.Instance.SoulDict[kvp.Key].stack = kvp.Value;
			}
		}

		public static void ReloadSoulIndexUI()
		{
			MysticHunter.Instance.soulIndexUI.soulIndexPanel.soulListPanel.soulList.ReloadList();
		}

		private static void AddNewSoul(BaseSoul data)
		{
			if (MysticHunter.Instance.SoulDict.ContainsKey(data.soulNPC))
			{
				MysticHunter.Instance.Logger.Warn("ID '" + data.soulNPC + "' - from class '" + data.GetType().ToString() + "' already exists under name '" + MysticHunter.Instance.SoulDict[data.soulNPC].SoulNPCName() + "'.");
				return;
			}
			MysticHunter.Instance.SoulDict.Add(data.soulNPC, data);
		}

		public static void UnloadSouls()
		{
			MysticHunter.Instance.SoulDict.Clear();
		}
	}
}
