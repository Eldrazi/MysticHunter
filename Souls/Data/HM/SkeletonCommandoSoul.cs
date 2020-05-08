using Terraria;
using Terraria.ID;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class SkeletonCommandoSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.SkeletonCommando;
		public override string soulDescription => "Fires a powerful rocket.";

		public override short cooldown => 600;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 120 + 10 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileID.RocketI, damage, 2, p.whoAmI);

			return (true);
		}
	}
}
