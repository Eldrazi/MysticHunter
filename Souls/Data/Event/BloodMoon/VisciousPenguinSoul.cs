#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.BloodMoon
{
	public class VisciousPenguinSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.CrimsonPenguin;
		public override string soulDescription => "Increases move speed.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.maxRunSpeed += 0.3f * stack;
			p.runAcceleration *= 1 + (0.25f * stack);

			if (p.velocity.Y == 0 && p.velocity.X != 0 && (int)Main.time % 10 == 0)
			{
				Projectile.NewProjectile(p.MountedCenter, Vector2.Zero, ModContent.ProjectileType<Projectiles.BloodTrail>(), (int)p.maxRunSpeed, 0f, p.whoAmI);
			}

			return (true);
		}
	}
}
