#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class DrakomireRiderSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SolarDrakomireRider;
		public override string soulDescription => "Toss a solar javelin.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 180 + 20 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileID.Daybreak, damage, 1f, p.whoAmI);

			return (true);
		}
	}
}
