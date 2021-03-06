﻿#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using MysticHunter.Souls.Buffs;
using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class WraithSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Wraith;
		public override string soulDescription => "Allows you to pass through enemies.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().preHurtModifier += OnHitModifier;
			return (true);
		}

		private bool OnHitModifier(Player player, ref int damage, PlayerDeathReason damageSource, byte soulStack)
		{
			if (!player.HasBuff(ModContent.BuffType<YellowSoulDebuff>()) && player.CheckMana(10, true))
			{
				player.immune = true;
				player.immuneTime = 10 + 5 * soulStack;
				player.manaRegenDelay = (int)player.maxRegenDelay;

				player.AddBuff(ModContent.BuffType<YellowSoulDebuff>(), 600);
				return (false);
			}
			return (true);
		}
	}
}
