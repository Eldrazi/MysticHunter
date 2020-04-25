using System.Linq;

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class DiabolistSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DiabolistRed;
		public override string soulDescription => "Increases fire damage.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack) => true;

		private readonly int[] fireProjectiles = new int[] {
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

		private readonly int[] fireItems = new int[] {
			ItemID.Flamarang, ItemID.FieryGreatsword, ItemID.MoltenPickaxe,
			ItemID.MoltenHamaxe, ItemID.DD2SquireBetsySword,
			ItemID.MonkStaffT2
		};

		public override void OnHitNPC(Player player, NPC npc, Entity hitEntity, ref int damage, byte stack)
		{
			int damageModifier = 10 * stack;

			if (hitEntity is Item item && fireItems.Contains(item.type))
				damage += damageModifier;
			else if (hitEntity is Projectile projectile && fireProjectiles.Contains(projectile.type))
				damage += damageModifier;
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.DiabolistWhite };
	}
}
