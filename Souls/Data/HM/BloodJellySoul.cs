using Terraria;
using Terraria.ID;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class BloodJellySoul : PostHMSoul
	{
		public override short soulNPC => NPCID.BloodJelly;
		public override string soulDescription => "Occasionally suck blood from enemies.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			int maxNpcAmount = 2;
			if (stack >= 5)
				maxNpcAmount++;
			if (stack >= 9)
				maxNpcAmount++;

			int maxDistance = 160;

			if (p.whoAmI != Main.myPlayer)
				return (false);

			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				NPC npc = Main.npc[i];

				if (npc.active && !npc.friendly && !npc.dontTakeDamage && !npc.immortal)
				{
					if (p.Distance(npc.Center) > maxDistance)
						continue;

					int healAmount = (int)System.Math.Max(1, stack * .8f);

					npc.netUpdate = true;
					npc.life -= healAmount;
					npc.HitEffect(0, healAmount);

					p.statLife += healAmount;
					p.HealEffect(healAmount);

					for (int j = 0; j < 5; ++j)
					{
						Dust d = Main.dust[Dust.NewDust(npc.Center, npc.width, npc.height, 235)];
						d.velocity *= 0f;
						d.scale = Main.rand.Next(70, 85) * .01f;
						d.fadeIn = p.whoAmI + 1;
					}

					if (--maxNpcAmount <= 0)
						break;
				}
			}

			return (true);
		}
	}
}
