#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.DataStructures;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class EvolutionBeastSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.NebulaBeast;
		public override string soulDescription => "Increases stats while not hit.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		private int modifier = 0;
		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (++modifier >= 0)
			{
				modifier = (int)MathHelper.Clamp(modifier, 0, 180);

				p.allDamage += (modifier / 240);
				p.statDefense += (modifier / 8);
			}

			p.GetModPlayer<SoulPlayer>().preHurtModifier += OnHitModifier;
			return (true);
		}

		private bool OnHitModifier(Player player, ref int damage, PlayerDeathReason damageSource, byte soulStack)
		{
			modifier = -60;
			return (true);
		}
	}
}
