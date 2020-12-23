#region Using directives

using Terraria;
using Terraria.ID;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	internal sealed class ElfArcherSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.ElfArcher;
		public override string soulDescription => "Shoot an elf arrow.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60;
			float modifier = 1f;

			if (stack >= 5)
			{
				damage += 20;
				modifier += 0.5f;
			}
			if (stack >= 9)
			{
				damage += 20;
				modifier += 0.5f;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8;
			int newProj = Projectile.NewProjectile(p.Center, velocity, ProjectileID.FlamingArrow, damage, 1, p.whoAmI);
			Main.projectile[newProj].scale = modifier;
			Main.projectile[newProj].netUpdate = true;

			return (true);
		}
	}
}
