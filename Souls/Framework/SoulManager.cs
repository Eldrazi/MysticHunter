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
			AddSoul(new AngryBonesSoul());
			AddSoul(new AnomuraFungusSoul());
			AddSoul(new AntlionSoul());
			AddSoul(new AntlionChargerSoul());
			AddSoul(new AntlionSwarmerSoul());
			AddSoul(new BabySlimeSoul());
			AddSoul(new BeeSoul());
			AddSoul(new BlackSlimeSoul());
			AddSoul(new BloodCrawlerSoul());
			AddSoul(new BlueJellyfishSoul());
			AddSoul(new BlueSlimeSoul());
			AddSoul(new BoneSerpentSoul());
			AddSoul(new CaveBatSoul());
			AddSoul(new CochinealBeetleSoul());
			AddSoul(new CrabSoul());
			AddSoul(new CrawdadSoul());
			AddSoul(new CrimeraSoul());
			AddSoul(new CursedSkullSoul());
			AddSoul(new CyanBeetleSoul());
			AddSoul(new DarkCasterSoul());
			AddSoul(new DemonSoul());
			AddSoul(new DemonEyeSoul());
			AddSoul(new DevourerSoul());
			AddSoul(new DoctorBonesSoul());
			AddSoul(new DungeonGuardianSoul());
			AddSoul(new DungeonSlimeSoul());
			AddSoul(new EaterOfSoulsSoul());
			AddSoul(new FaceMonsterSoul());
			AddSoul(new FireImpSoul());
			AddSoul(new FungiBulbSoul());
			AddSoul(new GiantShellySoul());
			AddSoul(new GiantWormSoul());
			AddSoul(new GoblinScoutSoul());
			AddSoul(new GraniteElementalSoul());
			AddSoul(new GraniteGolemSoul());
			AddSoul(new GreenSlimeSoul());
			AddSoul(new HarpySoul());
			AddSoul(new HellbatSoul());
			AddSoul(new HopliteSoul());
			AddSoul(new HornetSoul());
			AddSoul(new IceBatSoul());
			AddSoul(new IceSlimeSoul());
			AddSoul(new JungleBatSoul());
			AddSoul(new JungleSlimeSoul());
			AddSoul(new LacBeetleSoul());
			AddSoul(new LavaSlimeSoul());
			AddSoul(new ManEaterSoul());
			AddSoul(new MeteorHeadSoul());
			AddSoul(new MotherSlimeSoul());
			AddSoul(new MushiLadybugSoul());
			AddSoul(new MushroomZombieSoul());
			AddSoul(new NymphSoul());
			AddSoul(new PinkySoul());
			AddSoul(new PinkJellyfishSoul());
			AddSoul(new PiranhaSoul());
			AddSoul(new PurpleSlimeSoul());
			AddSoul(new RavenSoul());
			AddSoul(new RedSlimeSoul());
			AddSoul(new SalamanderSoul());
			AddSoul(new SandSlimeSoul());
			AddSoul(new SeaSnailSoul());
			AddSoul(new SharkSoul());
			AddSoul(new SkeletonSoul());
			AddSoul(new SnatcherSoul());
			AddSoul(new SnowFlinxSoul());
			AddSoul(new SpikedIceSlimeSoul());
			AddSoul(new SpikedJungleSlimeSoul());
			AddSoul(new SquidSoul());
			AddSoul(new TimSoul());
			AddSoul(new TombCrawlerSoul());
			AddSoul(new UndeadMinerSoul());
			AddSoul(new UndeadVikingSoul());
			AddSoul(new VoodooDemonSoul());
			AddSoul(new YellowSlimeSoul());
			AddSoul(new VultureSoul());
			AddSoul(new WallCreeperSoul());
			AddSoul(new ZombieSoul());
			AddSoul(new ZombieEskimoSoul());

			// Hardmode souls.
			/*AddSoul(new AngryTrapperSoul());
			AddSoul(new ArmoredVikingSoul());
			AddSoul(new BasiliskSoul());
			AddSoul(new DreamerGhoulSoul());
			AddSoul(new FloatyGrossSoul());
			AddSoul(new HellArmoredBonesSoul());
			AddSoul(new IceElementalSoul());
			AddSoul(new NecromancerSoul());
			AddSoul(new RustyArmoredBonesSoul());
			AddSoul(new SandPoacherSoul());
			AddSoul(new TaintedGhoulSoul());
			AddSoul(new UnicornSoul());
			AddSoul(new VileGhoulSoul());
			AddSoul(new WyvernSoul());*/

			// Event souls.
			AddSoul(new GoblinArcherSoul());
			AddSoul(new GoblinPeonSoul());
			AddSoul(new GoblinSorcererSoul());
			AddSoul(new GoblinSummonerSoul());
			AddSoul(new GoblinThiefSoul());
			AddSoul(new GoblinWarriorSoul());

			// Boss souls.
			AddSoul(new BrainOfCthuluSoul());
			//AddSoul(new DarkMageSoul());
			//AddSoul(new DestroyerSoul());
			AddSoul(new EaterOfWorldsSoul());
			AddSoul(new EyeOfCthuluSoul());
			AddSoul(new KingSlimeSoul());
			AddSoul(new QueenBeeSoul());
			AddSoul(new SkeletronSoul());
			AddSoul(new WallOfFleshSoul());
		}

		public static void AddSoul(BaseSoul data)
		{
			if (MysticHunter.Instance.SoulDict.ContainsKey(data.soulNPC))
			{
				MysticHunter.Instance.Logger.Warn("ID '" + data.soulNPC + "' - from class '" + data.GetType().ToString() + "' already exists under name '" + MysticHunter.Instance.SoulDict[data.soulNPC].SoulNPCName() + "'.");
				return;
			}
			MysticHunter.Instance.SoulDict.Add(data.soulNPC, data);
		}
		public static BaseSoul GetSoul(short soulNPC) => MysticHunter.Instance.SoulDict.TryGetValue(soulNPC, out BaseSoul result) ? result : null;

		public static void ReloadSoulIndexUI()
		{
			MysticHunter.Instance.soulIndexUI.soulIndexPanel.soulListPanel.soulList.ReloadList();
		}

		public static void UnloadSouls()
		{
			MysticHunter.Instance.SoulDict.Clear();
		}
	}
}
