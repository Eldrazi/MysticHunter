using System.Linq;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using MysticHunter.Souls.Items;
using MysticHunter.Souls.Data.Pre_HM;
using MysticHunter.Souls.Data.Bosses;

namespace MysticHunter
{
	public delegate void PreHurtModifier(Player player, ref int damage, PlayerDeathReason damageSource, int soulStack);

	/// <summary>
	/// ModPlayer class for handling everything soul related (active/passive updates).
	/// </summary>
	public class SoulPlayer : ModPlayer
	{
		/// <summary>
		/// An array that keeps track of soul items.
		/// Red, Blue and Yellow souls are stacked in the array in that order (0, 1, 2).
		/// </summary>
		public BaseSoul[] souls;
		public BaseSoul RedSoul
		{
			get { return souls[(int)SoulType.Red]; }
			set { souls[(int)SoulType.Red] = value; }
		}
		public BaseSoul BlueSoul
		{
			get { return souls[(int)SoulType.Blue]; }
			set { souls[(int)SoulType.Blue] = value; }
		}
		public BaseSoul YellowSoul
		{
			get { return souls[(int)SoulType.Yellow]; }
			set { souls[(int)SoulType.Yellow] = value; }
		}

		public readonly float[] DefinedSoulDropModifier = new float[3] { .1f, .1f, .1f };
		public float[] soulDropModifier;

		private short redSoulCooldown, blueSoulCooldown, yellowSoulCooldown;

		public PreHurtModifier preHurtModifier = null;

		// Yellow soul booleans.
		public bool pinkySoul = false;
		public bool seaSnailSoul = false;
		public bool undeadMinerSoul = false;

		// Blue soul booleans.
		public bool lacBeetleSoul = false;
		public bool cyanBeetleSoul = false;
		public bool cochinealBeetleSoul = false;

		// Additional soul variables.
		public bool eocSoulDash = false;
		public int seaSnailAnimationCounter = 0;

		/// <summary>
		/// Initializes the `souls` array.
		/// Done in this function so every player has his/her own instance.
		/// </summary>
		public override void Initialize()
		{
			this.souls = new BaseSoul[3];

			this.soulDropModifier = new float[3] { DefinedSoulDropModifier[0], DefinedSoulDropModifier[1], DefinedSoulDropModifier[2] };
		}

		public override void ResetEffects()
		{
			preHurtModifier = null;

			pinkySoul = false;
			seaSnailSoul = false;
			undeadMinerSoul = false;

			if (BlueSoul == null || BlueSoul.soulNPC != NPCID.LacBeetle)
				lacBeetleSoul = false;
			if (BlueSoul == null || BlueSoul.soulNPC != NPCID.CyanBeetle)
				cyanBeetleSoul = false;
			if (BlueSoul == null || BlueSoul.soulNPC != NPCID.CochinealBeetle)
				cochinealBeetleSoul = false;

			if (BlueSoul == null || BlueSoul.soulNPC != NPCID.EyeofCthulhu)
				eocSoulDash = false;

			this.soulDropModifier[0] = DefinedSoulDropModifier[0];
			this.soulDropModifier[1] = DefinedSoulDropModifier[1];
			this.soulDropModifier[2] = DefinedSoulDropModifier[2];
		}

		/// <summary>
		/// Used to add the Soul Index item to starting characters.
		/// </summary>
		public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
		{
			Item item = new Item();
			item.SetDefaults(ItemType<SoulIndex>());
			items.Add(item);
		}

		/// <summary>
		/// Used to process Yellow soul passives, if available.
		/// </summary>
		public override void PostUpdateEquips()
		{
			if (YellowSoul != null && yellowSoulCooldown == 0)
				YellowSoul.SoulUpdate(player, YellowSoul.stack);

			for (int i = 0; i < souls.Length; ++i)
				if (souls[i] != null)
					souls[i].PostUpdate(player);

			if (redSoulCooldown > 0)
				redSoulCooldown--;
			if (blueSoulCooldown > 0)
				blueSoulCooldown--;
			if (yellowSoulCooldown > 0)
				yellowSoulCooldown--;
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (lacBeetleSoul)
				LacBeetleSoul.ModifyHit(player, ref damage, damageSource, BlueSoul.stack);
			if (cyanBeetleSoul)
				CyanBeetleSoul.ModifyHit(player, ref damage, damageSource, BlueSoul.stack);
			if (cochinealBeetleSoul)
				CochinealBeetleSoul.ModifyHit(player, ref damage, damageSource, BlueSoul.stack);

			preHurtModifier?.Invoke(player, ref damage, damageSource, YellowSoul.stack);

			return (true);
		}

		/// <summary>
		/// Used to process Red and Blue soul active triggers/hotkeys, if available.
		/// </summary>
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (RedSoul != null && redSoulCooldown <= 0 && MysticHunter.Instance.RedSoulActive.Current)
			{
				if (player.CheckMana(RedSoul.ManaCost(player, RedSoul.stack), true, false) && RedSoul.SoulUpdate(player, RedSoul.stack))
				{
					redSoulCooldown = RedSoul.cooldown;
					player.manaRegenDelay = (int)player.maxRegenDelay;
				}
			}

			if (BlueSoul != null && blueSoulCooldown <= 0 && MysticHunter.Instance.BlueSoulActive.Current)
			{
				if (player.CheckMana(BlueSoul.ManaCost(player, BlueSoul.stack), true, false) && BlueSoul.SoulUpdate(player, BlueSoul.stack))
				{
					blueSoulCooldown = BlueSoul.cooldown;
					player.manaRegenDelay = (int)player.maxRegenDelay;
				}
			}
		}

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			if (this.seaSnailSoul)
			{
				layers.Find(x => x.Name == "Head").visible = false;
				SeaSnailSoul.DrawLayer.visible = true;
				layers.Add(SeaSnailSoul.DrawLayer);
			}
			else
				seaSnailAnimationCounter = 0;

			if (this.lacBeetleSoul)
			{
				LacBeetleSoul.DrawLayer.visible = true;
				layers.Add(LacBeetleSoul.DrawLayer);
			}
			if (this.cyanBeetleSoul)
			{
				CyanBeetleSoul.DrawLayer.visible = true;
				layers.Add(CyanBeetleSoul.DrawLayer);
			}
			if (this.cochinealBeetleSoul)
			{
				CochinealBeetleSoul.DrawLayer.visible = true;
				layers.Add(CochinealBeetleSoul.DrawLayer);
			}

			if (this.eocSoulDash)
			{
				EyeOfCthuluSoul.DrawLayer.visible = true;
				layers.Add(EyeOfCthuluSoul.DrawLayer);
			}
		}

		/// <summary>
		/// Saving and loading for soul items.
		/// </summary>
		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			for (int i = 0; i < souls.Length; ++i)
			{
				short id = 0;
				if (souls[i] != null)
					id = souls[i].soulNPC;
				tag.Add("soul" + i, id);
			}

			// A simple query to get every soul that the player has unlocked and store it in a list for saving purposed.
			Dictionary<short, byte> acquiredSouls = MysticHunter.Instance.SoulDict.Values
				.Where(v => v.acquired == true)
				.ToDictionary(v => v.soulNPC, v => v.stack);

			tag.Add("acquiredSoulsKeys", acquiredSouls.Keys.ToList());
			tag.Add("acquiredSoulsValues", acquiredSouls.Values.ToList());

			return tag;
		}
		public override void Load(TagCompound tag)
		{
			try
			{
				souls = new BaseSoul[3];
				for (int i = 0; i < souls.Length; i++)
				{
					short id = tag.GetShort("soul" + i);
					if (id != 0)
						souls[i] = MysticHunter.Instance.SoulDict[id];
				}

				var keys = tag.Get<List<short>>("acquiredSoulsKeys");
				var values = tag.Get<List<byte>>("acquiredSoulsValues");
				SoulManager.ResetSoulAcquisition(keys.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value));
				SoulManager.ReloadSoulIndexUI();
			}
			catch { }
		}
	}
}
