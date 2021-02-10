#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.SolarEclipse
{
	public class BloodJellySoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.NebulaHeadcrab;
		public override string soulDescription => "Siphon health from enemies around you.";

		public override short cooldown => 10;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 1;
		public override bool SoulUpdate(Player p, short stack)
		{
			int healAmount = 1;
			int maxDistance = 80;
			int maxNpcAmount = stack;

			if (p.whoAmI != Main.myPlayer)
				return (false);

			for (int i = 0; i < Main.maxNPCs && maxNpcAmount > 0; ++i)
			{
				NPC npc = Main.npc[i];

				if (npc.active && !npc.friendly && !npc.dontTakeDamage && !npc.immortal)
				{
					if (p.Distance(npc.Center) > maxDistance)
						continue;

					maxNpcAmount--;

					npc.netUpdate = true;
					npc.life -= healAmount;
					npc.HitEffect(0, healAmount);

					p.statLife += healAmount;
					p.HealEffect(healAmount);

					for (int j = 0; j < 5; ++j)
					{
						Dust d = Main.dust[Dust.NewDust(npc.Center, npc.width, npc.height, 235)];
						d.velocity *= 0f;
						d.fadeIn = p.whoAmI + 1;
						d.scale = Main.rand.Next(70, 85) * .01f;
					}
				}
			}

			return (true);
		}
	}
}
