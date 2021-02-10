#region Using directives

using Terraria;
using Terraria.ID;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class VortexianSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.VortexSoldier;
		public override string soulDescription => "Allows you to hover above the ground.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		private float heightModifier = 0;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.mount.Active)
			{
				return (false);
			}

			p.GetModPlayer<SoulPlayer>().vortexianSoul = true;

			if (p.controlUp)
			{
				heightModifier += 0.075f;
			}
			else if (p.controlDown)
			{
				heightModifier -= 0.075f;
			}
			heightModifier = MathHelper.Clamp(heightModifier, 1, stack * 1.5f);

			int minX = (int)MathHelper.Clamp(p.position.X / 16, 0, Main.maxTilesX);
			int maxX = (int)MathHelper.Clamp((p.position.X + p.width) / 16, 0, Main.maxTilesX);

			int minY, maxY;

			if (p.gravDir > 0)
			{
				minY = (int)MathHelper.Clamp((p.position.Y + p.height) / 16, 0, Main.maxTilesY);
				maxY = (int)MathHelper.Clamp((p.position.Y + p.height) / 16 + heightModifier, 0, Main.maxTilesY);
			}
			else
			{
				minY = (int)MathHelper.Clamp(p.position.Y / 16 - heightModifier, 0, Main.maxTilesY);
				maxY = (int)MathHelper.Clamp(p.position.Y / 16, 0, Main.maxTilesY);
			}

			bool canHover = false;

			for (int x = minX; x <= maxX; ++x)
			{
				for (int y = minY; y <= maxY; ++y)
				{
					Tile t = Framing.GetTileSafely(x, y);

					if (t.active() && Main.tileSolid[t.type])
					{
						canHover = true;
						break;
					}
				}
			}

			if (canHover)
			{
				if (p.velocity.Y > 0)
				{
					p.velocity.Y *= 0.98f;
				}
				p.velocity.Y -= p.gravity * p.gravDir * 1.25f;

				p.fallStart = (int)(p.position.Y / 16);
			}

			return (true);
		}
	}
}
