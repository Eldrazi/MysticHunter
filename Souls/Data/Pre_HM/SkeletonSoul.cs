using Terraria;
using Terraria.ID;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SkeletonSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Skeleton;
		public string soulDescription => "Fires a small horde of bees.";

		public short cooldown => 180;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => 10;
		public bool SoulUpdate(Player p, short stack)
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
			return (true);
		}
	}
}
