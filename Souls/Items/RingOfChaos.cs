using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Items
{
	public class RingOfChaos : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ring of Chaos");
			Tooltip.SetDefault("Grants spelunker, hunter and danger sense effects\n" +
				"Increased enemy spawnrate\n10% damage reduction\n" +
				"Grants night vision and shine effects\n" +
				"25% chance to inflict ichor and cursed fire on hit\n" +
				"Grants Well Fed buff\nGrants increased abilities in water\n" +
				"Grants immunity to gravity effects\n" +
				"Grants increases life regen\n" +
				"Inflict daybroken and venom on melee hit\n" +
				"20 % increased summon damage\n" +
				"27% damage reduction\n" +
				"[c/FF0000:Drastically ] increase the chance to drop souls\n" +
				"[c/00FF00:Grants infinite mana]");
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = ItemRarityID.Expert;

			item.value = Item.sellPrice(10, 0, 0, 0);

			item.defense = 5;
			item.material = true;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// Band of Double Sight
			player.dangerSense = true;
			player.findTreasure = true;
			player.detectCreature = true;

			// Cursed Hand
			player.endurance += .1f;
			player.ZoneWaterCandle = true;

			// Occular Charm
			player.nightVision = true;
			Lighting.AddLight((int)(player.position.X + (player.width / 2)) / 16, (int)(player.position.Y + (player.height / 2)) / 16, 0.8f, 0.95f, 1f);

			// Brace of Evil
			player.GetModPlayer<SoulPlayer>().BraceOfEvil = true;

			// Fishron Steak
			player.wellFed = true;
			player.accFlipper = true;
			player.waterWalk2 = true;
			player.ignoreWater = true;

			if (player.wet)
			{
				player.moveSpeed += .2f;
				player.allDamage += .1f;
			}

			// Golematic Boots
			player.buffImmune[BuffID.Gravitation] = true;
			player.buffImmune[BuffID.VortexDebuff] = true;

			// Living Gloves
			player.lifeRegen += 8;

			// Lunar Ritual
			player.GetModPlayer<SoulPlayer>().LunarRitual = true;

			// Mechanized Bone Necklace
			player.minionDamage += .2f;

			// Mechano Scarf
			player.endurance += .27f;

			// Queen Knuckle
			player.GetModPlayer<SoulPlayer>().QueenKnuckle = true;

			// Soul of the Damned
			if (!player.miscEquips.Any(x => x.type == ItemType<SoulOfTheDamned>()))
			{
				player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Red] += 0.05f;
				player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Blue] += 0.05f;
				player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Yellow] += 0.05f;
			}

			player.statMana = player.statManaMax2;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<ChaosStone>());
			recipe.AddIngredient(ItemType<LuminiteRing>());
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
