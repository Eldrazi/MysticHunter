#region Using directives

using System.IO;
using System.Collections.Generic;

using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

#endregion

namespace MysticHunter.Common.GlobalItems
{
	public class SoulItem : GlobalItem
	{
		public byte soulDropModifier;

		public override bool InstancePerEntity => true;

		public SoulItem()
		{
			soulDropModifier = 0;
		}

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			SoulItem myClone = (SoulItem)base.Clone(item, itemClone);
			myClone.soulDropModifier = soulDropModifier;
			return (myClone);
		}

		public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			if (item.accessory && item.maxStack == 1 && rand.NextBool(30))
			{
				/*switch (rand.Next(3))
				{
					case 0:
						return Mod.PrefixType("SoulSiphon");
					case 1:
						return mod.PrefixType("SoulStealing");
					default:
						return mod.PrefixType("SoulReaping");
				}*/
			}
			return -1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (!item.social && item.prefix > 0)
			{
				if (soulDropModifier > 0)
				{
					TooltipLine line = new TooltipLine(Mod, "PrefixSoulDropPower", "+" + soulDropModifier + "% soul drops")
					{
						isModifier = true
					};
					tooltips.Add(line);
				}
			}
		}

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			SoulPlayer sp;
			if (soulDropModifier > 0)
			{
				sp = player.GetModPlayer<SoulPlayer>();
				sp.soulDropModifier[0] += soulDropModifier / 100f;
				sp.soulDropModifier[1] += soulDropModifier / 100f;
				sp.soulDropModifier[2] += soulDropModifier / 100f;
			}
		}

		public override void ExtractinatorUse(int extractType, ref int resultType, ref int resultStack)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			if (sp.undeadMinerSoul)
			{
				int amount = 1;
				int stack = sp.UnlockedSouls[sp.YellowSoul.soulNPC];

				if (stack >= 5)
					amount++;
				if (stack >= 9)
					amount++;

				resultStack += amount;
			}
		}

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			if (sp.poltergeistSoul == true)
			{
				grabRange += sp.YellowSoulNet.stack * 3;
			}
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(soulDropModifier);
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			soulDropModifier = reader.ReadByte();
		}
	}
}
