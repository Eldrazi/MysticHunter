#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class SkeletonSniperSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.SkeletonSniper;
		public override string soulDescription => "Fire an immensely powerful bullet.";

		public override short cooldown => 3000;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 50;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 300 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileID.BulletHighVelocity, damage, 2.5f, p.whoAmI);
			SoundEngine.PlaySound(SoundID.Item41, p.Center);
			return (true);
		}
	}
}
