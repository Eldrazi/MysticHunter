#region Using directives

using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.DataStructures;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class MeteorHeadSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.MeteorHead;
		public override string soulDescription => "Reduces damage from fire attacks.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			SoulPlayer sp = p.GetModPlayer<SoulPlayer>();
			sp.preHurtModifier += ModifyHit;
			return (true);
		}

		private readonly int[] fireProjectiles = new int []
		{
			ProjectileID.FireArrow, ProjectileID.BallofFire,
			ProjectileID.Flamelash, ProjectileID.Sunfury,
			ProjectileID.HellfireArrow, ProjectileID.FlamingArrow,
			ProjectileID.Flames, ProjectileID.FlamethrowerTrap,
			ProjectileID.FlamesTrap, ProjectileID.Fireball,
			ProjectileID.InfernoHostileBolt, ProjectileID.InfernoHostileBlast,
			ProjectileID.InfernoFriendlyBolt, ProjectileID.InfernoFriendlyBlast,
			ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3,
			ProjectileID.FlamingScythe, ProjectileID.ImpFireball,
			ProjectileID.MolotovFire, ProjectileID.MolotovFire2, ProjectileID.MolotovFire3,
			ProjectileID.CultistBossFireBall, ProjectileID.CultistBossFireBallClone,
			ProjectileID.HelFire, ProjectileID.Daybreak,
			ProjectileID.GeyserTrap, ProjectileID.SpiritFlame,
			ProjectileID.DD2BetsyFireball, ProjectileID.DD2BetsyFlameBreath,
			ProjectileID.MonkStaffT2Ghast, ProjectileID.DD2PhoenixBowShot
		};

		private readonly int[] fireNPCs = new int[]
		{
			NPCID.MeteorHead, NPCID.FireImp, NPCID.BurningSphere,
			NPCID.LavaSlime, NPCID.Hellbat, NPCID.BlazingWheel,
			NPCID.Lavabat, NPCID.Pumpking, NPCID.PumpkingBlade,
			NPCID.SolarCrawltipedeHead, NPCID.SolarCrawltipedeBody,
			NPCID.SolarCrawltipedeTail, NPCID.SolarDrakomire,
			NPCID.SolarDrakomireRider, NPCID.SolarSroller,
			NPCID.SolarCorite, NPCID.SolarSolenian,
			NPCID.SolarFlare, NPCID.SolarSpearman, NPCID.SolarGoop
		};

		private readonly int[] fireItems = new int[]
		{
			ItemID.Flamarang, ItemID.FieryGreatsword, ItemID.MoltenPickaxe,
			ItemID.MoltenHamaxe, ItemID.DD2SquireBetsySword,
			ItemID.MonkStaffT2
		};

		private bool ModifyHit(Player p, ref int damage, PlayerDeathReason damageSource, byte soulStack)
		{
			int damageReduction = 5 * soulStack;

			if ((damageSource.SourceProjectileType != 0 && fireProjectiles.Contains((int)damageSource.SourceProjectileType)) ||
				 (damageSource.SourceNPCIndex != 0 && fireNPCs.Contains(Main.npc[damageSource.SourceNPCIndex].type)) ||
				 (damageSource.SourceItemType != 0 && fireItems.Contains(damageSource.SourceItemType)))
				damage -= damageReduction;
			return (true);
		}
	}
}
