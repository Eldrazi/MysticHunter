#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.SolarEclipse
{
	public class PsychoSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Psycho;
		public override string soulDescription => "Invisibility and damage, at the cost of defense and speed.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			int selectedItemType = p.inventory[p.selectedItem].type;

			if (selectedItemType != ItemID.PsychoKnife)
			{
				if (p.itemAnimation > 0)
				{
					p.stealthTimer = 15;
					if (p.stealth > 0f)
					{
						p.stealth += 0.1f;
					}
				}
				else if (!p.mount.Active && Math.Abs(p.velocity.X) < 0.1f && Math.Abs(p.velocity.Y) < 0.1f)
				{
					if (p.stealthTimer == 0 && p.stealth > 0)
					{
						p.stealth -= 0.02f;
						if (p.stealth <= 0f)
						{
							p.stealth = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
							{
								NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, p.whoAmI);
							}
						}
					}
				}
				else
				{
					if (p.stealth > 0f)
					{
						p.stealth += 0.1f;
					}
					if (p.mount.Active)
					{
						p.stealth = 1f;
					}
				}

				if (p.stealthTimer > 0)
				{
					p.stealthTimer--;
				}
			}

			p.stealth = MathHelper.Clamp(p.stealth, 0f, 1f);
			
			p.aggro -= (int)((1f - p.stealth) * 100 * stack);

			p.GetDamage<Melee>() += (1f - p.stealth) * (0.5f * stack);
			p.GetCrit<Melee>() += (int)((1f - p.stealth) * 30f);
			if (p.GetCrit<Melee>() > 100)
			{
				p.GetCrit<Melee>().additive = 100;
			}

			return (true);
		}
	}
}
