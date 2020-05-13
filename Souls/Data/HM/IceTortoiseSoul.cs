using Terraria;
using Terraria.ID;
using Terraria.DataStructures;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class IceTortoiseSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.IceTortoise;
		public override string soulDescription => "Create a shield that freezes enemies.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => (short)(p.GetModPlayer<SoulPlayer>().iceTortoiseSoul ? 0 : 15);
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().iceTortoiseSoul = !p.GetModPlayer<SoulPlayer>().iceTortoiseSoul;
			return (true);
		}

		public override void PostUpdate(Player player)
		{
			if (player.GetModPlayer<SoulPlayer>().iceTortoiseSoul)
			{
				player.statDefense += 5;
				player.noKnockback = true;
				player.manaRegenDelay = 10;
			}
		}

		public static void ModifyHit(Player p, ref int damage, PlayerDeathReason damageSource, int stack)
		{
			if (damageSource.SourceNPCIndex != 0)
			{
				NPC hitBy = Main.npc[damageSource.SourceNPCIndex];

				if (hitBy.active && !hitBy.boss)
					hitBy.AddBuff(BuffID.Frostburn, 120 + 30 * stack);
			}
		}
	}
}
