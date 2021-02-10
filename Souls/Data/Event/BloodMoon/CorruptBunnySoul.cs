#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.BloodMoon
{
	public class CorruptBunnySoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.CorruptBunny;
		public override string soulDescription => "Increases jump height.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.jumpBoost = true;
			p.jumpSpeedBoost += stack * 0.6f;

			p.extraFall += 20;

			if (p.velocity.Y != 0 && (int)Main.time % 10 == 0)
			{
				Projectile.NewProjectile(p.MountedCenter, Vector2.Zero, ModContent.ProjectileType<Projectiles.VileTrail>(), (int)p.jumpSpeedBoost, 0f, p.whoAmI);
			}

			return (true);
		}
	}
}
