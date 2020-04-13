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
		public NetSoulData[] activeSouls;

		public BaseSoul RedSoul
		{
			get { return activeSouls[(int)SoulType.Red].soulNPC == 0 ? null : SoulManager.GetSoul(activeSouls[(int)SoulType.Red].soulNPC); }
			set
			{
				if (value == null)
					activeSouls[(int)SoulType.Red].soulNPC = 0;
				else
				{
					activeSouls[(int)SoulType.Red].soulNPC = value.soulNPC;
					activeSouls[(int)SoulType.Red].stack = UnlockedSouls[value.soulNPC];
				}
			}
		}
		public BaseSoul BlueSoul
		{
			get { return activeSouls[(int)SoulType.Blue].soulNPC == 0 ? null : SoulManager.GetSoul(activeSouls[(int)SoulType.Blue].soulNPC); }
			set
			{
				if (value == null)
					activeSouls[(int)SoulType.Blue].soulNPC = 0;
				else
				{
					activeSouls[(int)SoulType.Blue].soulNPC = value.soulNPC;
					activeSouls[(int)SoulType.Blue].stack = UnlockedSouls[value.soulNPC];
				}
			}
		}
		public BaseSoul YellowSoul
		{
			get { return activeSouls[(int)SoulType.Yellow].soulNPC == 0 ? null : SoulManager.GetSoul(activeSouls[(int)SoulType.Yellow].soulNPC); }
			set
			{
				if (value == null)
					activeSouls[(int)SoulType.Yellow].soulNPC = 0;
				else
				{
					activeSouls[(int)SoulType.Yellow].soulNPC = value.soulNPC;
					activeSouls[(int)SoulType.Yellow].stack = UnlockedSouls[value.soulNPC];
				}
			}
		}

		public float[] soulDropModifier;
		public readonly float[] DefinedSoulDropModifier = new float[3] { .015f, .015f, .015f };

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
			if (this.activeSouls == null)
				this.activeSouls = new NetSoulData[3] { new NetSoulData(), new NetSoulData(), new NetSoulData() };

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
			if (activeSouls[(int)SoulType.Yellow].soulNPC != 0 && yellowSoulCooldown == 0)
			{
				yellowSoulCooldown = YellowSoul.cooldown;
				YellowSoul.SoulUpdate(player, activeSouls[(int)SoulType.Yellow].stack);
			}

			RedSoul?.PostUpdate(player);
			BlueSoul?.PostUpdate(player);
			YellowSoul?.PostUpdate(player);

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
				LacBeetleSoul.ModifyHit(player, ref damage, damageSource, activeSouls[(int)SoulType.Blue].stack);
			if (cyanBeetleSoul)
				CyanBeetleSoul.ModifyHit(player, ref damage, damageSource, activeSouls[(int)SoulType.Blue].stack);
			if (cochinealBeetleSoul)
				CochinealBeetleSoul.ModifyHit(player, ref damage, damageSource, activeSouls[(int)SoulType.Blue].stack);

			preHurtModifier?.Invoke(player, ref damage, damageSource, activeSouls[(int)SoulType.Yellow].stack);

			return (true);
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			YellowSoul?.OnHitNPC(player, target, item, activeSouls[(int)SoulType.Yellow].stack);
		}
		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			YellowSoul?.OnHitNPC(player, target, proj, activeSouls[(int)SoulType.Yellow].stack);
		}

		/// <summary>
		/// Used to process Red and Blue soul active triggers/hotkeys, if available.
		/// </summary>
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (activeSouls[(int)SoulType.Red].soulNPC != 0 && redSoulCooldown <= 0 && MysticHunter.Instance.RedSoulActive.Current)
			{
				if (player.CheckMana(RedSoul.ManaCost(player, UnlockedSouls[RedSoul.soulNPC]), true, false) && RedSoul.SoulUpdate(player, UnlockedSouls[RedSoul.soulNPC]))
				{
					redSoulCooldown = RedSoul.cooldown;
					player.manaRegenDelay = (int)player.maxRegenDelay;
				}
			}

			if (activeSouls[(int)SoulType.Blue].soulNPC != 0 && blueSoulCooldown <= 0 && MysticHunter.Instance.BlueSoulActive.Current)
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

			tag.Add("acquiredSoulsKeys", UnlockedSouls.Keys.ToList());
			tag.Add("acquiredSoulsValues", UnlockedSouls.Values.ToList());

			for (int i = 0; i < activeSouls.Length; ++i)
				tag.Add("soul" + i, activeSouls[i].soulNPC);

			return tag;
		}
		public override void Load(TagCompound tag)
		{
			try
			{
				var keys = tag.Get<List<short>>("acquiredSoulsKeys");
				var values = tag.Get<List<byte>>("acquiredSoulsValues");
				UnlockedSouls = keys.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);

				activeSouls = new NetSoulData[3];
				for (int i = 0; i < activeSouls.Length; i++)
				{
					short soulNPC = tag.GetShort("soul" + i);
					activeSouls[i] = new NetSoulData(soulNPC, 
						UnlockedSouls.TryGetValue(soulNPC, out byte result) ? result : (byte)0);
				}
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

			for (int i = 0; i < activeSouls.Length; ++i)
			{
				if (activeSouls[i] == null)
					activeSouls[i] = new NetSoulData();

				clone.activeSouls[i].soulNPC = this.activeSouls[i].soulNPC;
				clone.activeSouls[i].stack = this.activeSouls[i].stack;
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
			packet.Write(activeSouls[0].soulNPC);
			packet.Write(activeSouls[1].soulNPC);
			packet.Write(activeSouls[2].soulNPC);
			packet.Write(activeSouls[0].stack);
			packet.Write(activeSouls[1].stack);
			packet.Write(activeSouls[2].stack);

			packet.Write(lacBeetleSoul);
			packet.Write(cyanBeetleSoul);
			packet.Write(cochinealBeetleSoul);

			packet.Write(eocSoulDash);

			packet.Send(toWho, fromWho);
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			SoulPlayer clone = clientPlayer as SoulPlayer;

			if (clone.activeSouls[(int)SoulType.Red] != this.activeSouls[(int)SoulType.Red] ||
				clone.activeSouls[(int)SoulType.Blue] != this.activeSouls[(int)SoulType.Blue] ||
				clone.activeSouls[(int)SoulType.Yellow] != this.activeSouls[(int)SoulType.Yellow])
			{
				ModPacket packet = mod.GetPacket();
				packet.Write((byte)MysticHunterMessageType.SyncPlayerSouls);
				packet.Write((byte)player.whoAmI);
				packet.Write(activeSouls[0].soulNPC);
				packet.Write(activeSouls[1].soulNPC);
				packet.Write(activeSouls[2].soulNPC);
				packet.Write(activeSouls[0].stack);
				packet.Write(activeSouls[1].stack);
				packet.Write(activeSouls[2].stack);
				packet.Send();
			}
		}

		#endregion

		#region Soul Util Functions

		public void UpdateActiveSoulData()
		{
			for (int i = 0; i < this.activeSouls.Length; ++i)
			{
				if (this.activeSouls[i].soulNPC != 0)
					this.activeSouls[i].stack = this.UnlockedSouls[this.activeSouls[i].soulNPC];
			}
		}

		#endregion
	}
}
