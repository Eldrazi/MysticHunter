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
		// A dictionary that keeps track of which souls the player has unlocked.
		public Dictionary<short, byte> UnlockedSouls = new Dictionary<short, byte>();

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

		public float[] soulDropModifier;
		public readonly float[] DefinedSoulDropModifier = new float[3] { .1f, .1f, .1f };

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
				YellowSoul.SoulUpdate(player, UnlockedSouls[YellowSoul.soulNPC]);

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
				LacBeetleSoul.ModifyHit(player, ref damage, damageSource, UnlockedSouls[BlueSoul.soulNPC]);
			if (cyanBeetleSoul)
				CyanBeetleSoul.ModifyHit(player, ref damage, damageSource, UnlockedSouls[BlueSoul.soulNPC]);
			if (cochinealBeetleSoul)
				CochinealBeetleSoul.ModifyHit(player, ref damage, damageSource, UnlockedSouls[BlueSoul.soulNPC]);

			preHurtModifier?.Invoke(player, ref damage, damageSource, UnlockedSouls[YellowSoul.soulNPC]);

			return (true);
		}

		/// <summary>
		/// Used to process Red and Blue soul active triggers/hotkeys, if available.
		/// </summary>
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (RedSoul != null && redSoulCooldown <= 0 && MysticHunter.Instance.RedSoulActive.Current)
			{
				if (player.CheckMana(RedSoul.ManaCost(player, UnlockedSouls[RedSoul.soulNPC]), true, false) && RedSoul.SoulUpdate(player, UnlockedSouls[RedSoul.soulNPC]))
				{
					redSoulCooldown = RedSoul.cooldown;
					player.manaRegenDelay = (int)player.maxRegenDelay;
				}
			}

			if (BlueSoul != null && blueSoulCooldown <= 0 && MysticHunter.Instance.BlueSoulActive.Current)
			{
				if (player.CheckMana(BlueSoul.ManaCost(player, UnlockedSouls[BlueSoul.soulNPC]), true, false) && BlueSoul.SoulUpdate(player, UnlockedSouls[BlueSoul.soulNPC]))
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

		public override void OnEnterWorld(Player player)
		{
			SoulManager.ReloadSoulIndexUI();
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

			tag.Add("acquiredSoulsKeys", UnlockedSouls.Keys.ToList());
			tag.Add("acquiredSoulsValues", UnlockedSouls.Values.ToList());

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
				UnlockedSouls = keys.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
			}
			catch 
			{
				UnlockedSouls = new Dictionary<short, byte>();
			}
		}

		#region Player Syncing & Networking

		public override void clientClone(ModPlayer clientClone)
		{
			SoulPlayer clone = clientClone as SoulPlayer;

			for (int i = 0; i < souls.Length; ++i)
			{
				if (souls[i] == null)
					clone.souls[i] = null;
				else
					clone.souls[i] = MysticHunter.Instance.SoulDict[souls[i].soulNPC];
			}

			clone.lacBeetleSoul = lacBeetleSoul;
			clone.cyanBeetleSoul = cyanBeetleSoul;
			clone.cochinealBeetleSoul = cochinealBeetleSoul;

			clone.eocSoulDash = eocSoulDash;
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			ModPacket packet = mod.GetPacket();
			packet.Write((byte)MysticHunterMessageType.SyncStartSoulPlayer);
			packet.Write((byte)player.whoAmI);
			packet.Write((RedSoul == null) ? 0 : RedSoul.soulNPC);
			packet.Write((BlueSoul == null) ? 0 : BlueSoul.soulNPC);
			packet.Write((YellowSoul == null) ? 0 : YellowSoul.soulNPC);

			packet.Write(lacBeetleSoul);
			packet.Write(cyanBeetleSoul);
			packet.Write(cochinealBeetleSoul);

			packet.Write(eocSoulDash);

			packet.Send(toWho, fromWho);
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			SoulPlayer clone = clientPlayer as SoulPlayer;

			if (IsSoulDifferent(clone.RedSoul, RedSoul) || IsSoulDifferent(clone.BlueSoul, BlueSoul) || IsSoulDifferent(clone.YellowSoul, YellowSoul))
			{
				ModPacket packet = mod.GetPacket();
				packet.Write((byte)MysticHunterMessageType.SyncPlayerSouls);
				packet.Write((byte)player.whoAmI);
				packet.Write((RedSoul == null) ? 0 : RedSoul.soulNPC);
				packet.Write((BlueSoul == null) ? 0 : BlueSoul.soulNPC);
				packet.Write((YellowSoul == null) ? 0 : YellowSoul.soulNPC);
				packet.Send();
			}
		}

		private bool IsSoulDifferent(BaseSoul soul, BaseSoul otherSoul)
		{
			if (soul == null || otherSoul == null)
			{
				return !(soul == null && otherSoul == null);
			}
			else
				return (soul.soulNPC != otherSoul.soulNPC);
		}

		#endregion
	}
}
