using System.Collections.Generic;

using Terraria.ID;

using MysticHunter.Souls.Data.HM;
using MysticHunter.Souls.Data.Event;
using MysticHunter.Souls.Data.Pre_HM;
using MysticHunter.Souls.Data.Bosses;

namespace MysticHunter.Souls.Framework
{
	public class SoulManager
	{
		public static void LoadSouls()
		{
			MysticHunter.Instance.SoulDict = new Dictionary<short, BaseSoul>();

			// Pre-hardmode souls.
			AddSoul(new AngryBonesSoul(), new short[] { NPCID.AngryBonesBig, NPCID.AngryBonesBigHelmet, NPCID.AngryBonesBigMuscle });
			AddSoul(new AnomuraFungusSoul());
			AddSoul(new AntlionSoul());
			AddSoul(new AntlionChargerSoul());
			AddSoul(new AntlionSwarmerSoul());
			AddSoul(new BabySlimeSoul());
			AddSoul(new BeeSoul(), new short[] { NPCID.BeeSmall });
			AddSoul(new BlackSlimeSoul());
			AddSoul(new BloodCrawlerSoul());
			AddSoul(new BlueJellyfishSoul());
			AddSoul(new BlueSlimeSoul());
			AddSoul(new BoneSerpentSoul());
			AddSoul(new CaveBatSoul());
			AddSoul(new CochinealBeetleSoul());
			AddSoul(new CrabSoul());
			AddSoul(new CrawdadSoul(), new short[] { NPCID.Crawdad2 });
			AddSoul(new CrimeraSoul(), new short[] { NPCID.LittleCrimera, NPCID.BigCrimera });
			AddSoul(new CursedSkullSoul());
			AddSoul(new CyanBeetleSoul());
			AddSoul(new DarkCasterSoul());
			AddSoul(new DemonSoul());
			AddSoul(new DemonEyeSoul(), new short[] { NPCID.DemonEye2, NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship });
			AddSoul(new DevourerSoul());
			AddSoul(new DoctorBonesSoul());
			AddSoul(new DungeonGuardianSoul());
			AddSoul(new DungeonSlimeSoul());
			AddSoul(new EaterOfSoulsSoul(), new short[] { NPCID.BigEater, NPCID.LittleEater });
			AddSoul(new FaceMonsterSoul());
			AddSoul(new FireImpSoul());
			AddSoul(new FungiBulbSoul());
			AddSoul(new GiantShellySoul(), new short[] { NPCID.GiantShelly2 });
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
			AddSoul(new SalamanderSoul(), new short[] { NPCID.Salamander2, NPCID.Salamander3, NPCID.Salamander4 });
			AddSoul(new SandSlimeSoul());
			AddSoul(new SeaSnailSoul());
			AddSoul(new SharkSoul());
			AddSoul(new SkeletonSoul(), new short[] { NPCID.BigSkeleton, NPCID.SmallSkeleton });
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
			AddSoul(new ZombieSoul(), new short[] { NPCID.SmallZombie, NPCID.BigZombie });
			AddSoul(new ZombieEskimoSoul());

			// Hardmode souls.
			/*AddSoul(new AngryTrapperSoul());
			AddSoul(new ArmoredVikingSoul());
			AddSoul(new BasiliskSoul());
			AddSoul(new BloodJellySoul());
			AddSoul(new BlueArmoredBonesSoul(), new short[] { NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesNoPants, NPCID.BlueArmoredBonesSword });
			AddSoul(new BigMimicSoul(), new short[] { NPCID.BigMimicCrimson, NPCID.BigMimicHallow, NPCID.BigMimicJungle });
			AddSoul(new DreamerGhoulSoul());
			AddSoul(new FloatyGrossSoul());
			AddSoul(new HellArmoredBonesSoul(), new short[] { NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword });
			AddSoul(new IceElementalSoul());
			AddSoul(new LihzahrdSoul());
			AddSoul(new MimicSoul());
			AddSoul(new NecromancerSoul());
			AddSoul(new RustyArmoredBonesSoul(), new short[] { NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.RustyArmoredBonesSwordNoArmor });
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
		public static void UnloadSouls()
		{
			MysticHunter.Instance.SoulDict.Clear();
		}

		/// <summary>
		/// Tries to add the <paramref name="data"/> to the <see cref="MysticHunter.SoulDict"/> dictionary.
		/// </summary>
		/// <param name="data">The <see cref="BaseSoul"/> to add to the dictionary.</param>
		/// <param name="alternateKeys">If set will try to add more keys to the same <paramref name="data"/> value.</param>
		public static void AddSoul(BaseSoul data, short[] alternateKeys = null)
		{
			AddSoulWithKey(data.soulNPC, data);

			if (alternateKeys != null)
			{
				for (int i = 0; i < alternateKeys.Length; ++i)
					AddSoulWithKey(alternateKeys[i], data);
			}
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

		public static BaseSoul GetSoul(short soulNPC) => MysticHunter.Instance.SoulDict.TryGetValue(soulNPC, out BaseSoul result) ? result : null;

		public static void ReloadSoulIndexUI()
		{
			MysticHunter.Instance.soulIndexUI.soulIndexPanel.soulListPanel.soulList.ReloadList();
		}
	}
}
