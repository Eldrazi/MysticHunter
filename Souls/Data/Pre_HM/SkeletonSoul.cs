using Terraria;
using Terraria.ID;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SkeletonSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Skeleton;
		public override string soulDescription => "Throw a bone.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 10;
			float size = 1;
			if (stack >= 5)
			{
				damage += 5;
				size += .2f;
			}
			if (stack >= 9)
			{
				damage += 9;
				size += .2f;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 7;
			int proj = Projectile.NewProjectile(p.Center, velocity, ProjectileID.Bone, damage, .1f, p.whoAmI);
			Main.projectile[proj].scale = size;
			Main.projectile[proj].netUpdate = true;
			Main.projectile[proj].noDropItem = true;
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.BigSkeleton, NPCID.SmallSkeleton };
	}
}
