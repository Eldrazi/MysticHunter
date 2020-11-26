using Terraria;
using Terraria.ID;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	public class SnowBallaSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SnowBalla;
		public override string soulDescription => "Toss a snowball.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(4 + 1 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 10 + 5 * stack;
			float knockback = .4f + .1f * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8;
			Projectile.NewProjectile(p.Center, velocity, ProjectileID.SnowBallFriendly, damage, knockback, p.whoAmI);

			return (true);
		}
	}
}
