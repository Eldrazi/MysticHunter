using System.Linq;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Buffs;
using MysticHunter.Souls.Data.HM;
using MysticHunter.Souls.Framework;
using MysticHunter.Souls.Data.Bosses;
using MysticHunter.Souls.Data.Pre_HM;

namespace MysticHunter
{
	public delegate bool PreHurtModifier(Player player, ref int damage, PlayerDeathReason damageSource, byte soulStack);

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
		public readonly float[] DefinedSoulDropModifier = new float[3] { 0.015f, 0.015f, 0.015f };

		public PreHurtModifier preHurtModifier = null;

		// Yellow soul booleans.
		public bool pinkySoul = false;
		public bool lamiaSoul = false;
		public bool ghostSoul = false;
		public bool seaSnailSoul = false;
		public bool undeadMinerSoul = false;
		public bool torturedSoulSoul = false;
		public bool possessedArmorSoul = false;
		public bool tacticalSkeletonSoul = false;

		// Blue soul booleans.
		public bool lacBeetleSoul = false;
		public bool cyanBeetleSoul = false;
		public bool cochinealBeetleSoul = false;
		public bool iceTortoiseSoul = false;

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
			lamiaSoul = false;
			ghostSoul = false;
			seaSnailSoul = false;
			undeadMinerSoul = false;
			torturedSoulSoul = false;
			possessedArmorSoul = false;
			tacticalSkeletonSoul = false;

			if (activeSouls[(int)SoulType.Blue].soulNPC != NPCID.LacBeetle)
				lacBeetleSoul = false;
			if (activeSouls[(int)SoulType.Blue].soulNPC != NPCID.CyanBeetle)
				cyanBeetleSoul = false;
			if (activeSouls[(int)SoulType.Blue].soulNPC != NPCID.CochinealBeetle)
				cochinealBeetleSoul = false;

			if (activeSouls[(int)SoulType.Blue].soulNPC != NPCID.EyeofCthulhu)
				eocSoulDash = false;

			this.soulDropModifier[0] = DefinedSoulDropModifier[0];
			this.soulDropModifier[1] = DefinedSoulDropModifier[1];
			this.soulDropModifier[2] = DefinedSoulDropModifier[2];
		}

		/// <summary>
		/// Used to process Yellow soul passives, if available.
		/// </summary>
		public override void PostUpdateEquips()
		{
			if (activeSouls[(int)SoulType.Yellow].soulNPC != 0)
			{
				BaseSoul soulReference = YellowSoul;
				soulReference.SoulUpdate(player, activeSouls[(int)SoulType.Yellow].stack);
			}

			RedSoul?.PostUpdate(player);
			BlueSoul?.PostUpdate(player);
			YellowSoul?.PostUpdate(player);
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (lacBeetleSoul)
				LacBeetleSoul.ModifyHit(player, ref damage, damageSource, activeSouls[(int)SoulType.Blue].stack);
			if (cyanBeetleSoul)
				CyanBeetleSoul.ModifyHit(player, ref damage, damageSource, activeSouls[(int)SoulType.Blue].stack);
			if (cochinealBeetleSoul)
				CochinealBeetleSoul.ModifyHit(player, ref damage, damageSource, activeSouls[(int)SoulType.Blue].stack);
			if (iceTortoiseSoul)
				IceTortoiseSoul.ModifyHit(player, ref damage, damageSource, activeSouls[(int)SoulType.Blue].stack);

			return (preHurtModifier?.Invoke(player, ref damage, damageSource, activeSouls[(int)SoulType.Yellow].stack) ?? true);
		}

		public override bool? CanHitNPC(Item item, NPC target)
		{
			if (target.friendly)
				return (this.torturedSoulSoul);
			return (null);
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
			=> YellowSoul?.OnHitNPC(player, target, item, ref damage, activeSouls[(int)SoulType.Yellow].stack);
		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
			=> YellowSoul?.OnHitNPC(player, target, proj, ref damage, activeSouls[(int)SoulType.Yellow].stack);

		/// <summary>
		/// Used to process Red and Blue soul active triggers/hotkeys, if available.
		/// </summary>
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			// Using a temporary BaseSoul reference to diminish the amount of times we have to call 
			BaseSoul tmpSoulRef;

			if (activeSouls[(int)SoulType.Red].soulNPC != 0 && !player.HasBuff(BuffType<RedSoulDebuff>()) && MysticHunter.Instance.RedSoulActive.Current)
			{
				tmpSoulRef = RedSoul;
				if (CheckSoulMana(tmpSoulRef.ManaCost(player, UnlockedSouls[activeSouls[(int)SoulType.Red].soulNPC]), true) &&
					tmpSoulRef.SoulUpdate(player, UnlockedSouls[activeSouls[(int)SoulType.Red].soulNPC]))
				{
					player.manaRegenDelay = (int)player.maxRegenDelay;
					player.AddBuff(BuffType<RedSoulDebuff>(), tmpSoulRef.cooldown);
				}
			}

			if (activeSouls[(int)SoulType.Blue].soulNPC != 0 && !player.HasBuff(BuffType<BlueSoulDebuff>()) && MysticHunter.Instance.BlueSoulActive.Current)
			{
				tmpSoulRef = BlueSoul;
				if (CheckSoulMana(tmpSoulRef.ManaCost(player, UnlockedSouls[activeSouls[(int)SoulType.Blue].soulNPC]), true) &&
					tmpSoulRef.SoulUpdate(player, UnlockedSouls[activeSouls[(int)SoulType.Blue].soulNPC]))
				{
					player.manaRegenDelay = (int)player.maxRegenDelay;
					player.AddBuff(BuffType<BlueSoulDebuff>(), tmpSoulRef.cooldown);
				}
			}
		}

		public override void FrameEffects()
		{
			if (this.lamiaSoul)
			{
				if (player.Male)
					player.head = ArmorIDs.Head.LamiaMale;
				else
					player.head = ArmorIDs.Head.LamiaFemale;

				player.body = ArmorIDs.Body.Lamia;
				player.legs = ArmorIDs.Legs.Lamia;
			}
			else if (this.possessedArmorSoul)
			{
				player.head = mod.GetEquipSlot("PossessedArmorHead", EquipType.Head);
				player.body = mod.GetEquipSlot("PossessedArmorBody", EquipType.Body);
				player.legs = mod.GetEquipSlot("PossessedArmorLegs", EquipType.Legs);
			}
			
			if (this.iceTortoiseSoul)
				player.shield = (sbyte)mod.GetEquipSlot("IceTortoiseShield", EquipType.Shield);
		}

		public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (this.tacticalSkeletonSoul && item.useAmmo == AmmoID.Bullet)
			{
				int amount = 1;
				if (this.activeSouls[(int)SoulType.Yellow].stack >= 5)
					amount++;
				if (this.activeSouls[(int)SoulType.Yellow].stack >= 9)
					amount++;

				for (int i = 0; i < amount; ++i)
				{
					Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY).RotatedByRandom(.2f), type, damage / 2, knockBack, player.whoAmI);
				}
			}
			return (true);
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
			=> SoulManager.ReloadSoulIndexUI();

		/// <summary>
		/// Saving and loading for soul items.
		/// </summary>
		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound
			{
				{ "acquiredSoulsKeys", UnlockedSouls.Keys.ToList() },
				{ "acquiredSoulsValues", UnlockedSouls.Values.ToList() }
			};

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

					activeSouls[i] = new NetSoulData();
					if (SoulManager.GetSoul(soulNPC) != null)
					{
						if (UnlockedSouls.TryGetValue(soulNPC, out byte result))
						{
							activeSouls[i].stack = result;
							activeSouls[i].soulNPC = soulNPC;
						}
					}
				}
			}
			catch
			{
				UnlockedSouls = new Dictionary<short, byte>();
				activeSouls = new NetSoulData[3] { new NetSoulData(), new NetSoulData(), new NetSoulData() };
			}
		}

		#region Player Syncing & Networking

		public override void clientClone(ModPlayer clientClone)
		{
			SoulPlayer clone = clientClone as SoulPlayer;

			for (int i = 0; i < this.activeSouls.Length; ++i)
			{
				if (this.activeSouls[i] == null)
					this.activeSouls[i] = new NetSoulData();

				clone.activeSouls[i].stack = this.activeSouls[i].stack;
				clone.activeSouls[i].soulNPC = this.activeSouls[i].soulNPC;
			}

			clone.seaSnailSoul = seaSnailSoul;
			clone.lacBeetleSoul = lacBeetleSoul;
			clone.cyanBeetleSoul = cyanBeetleSoul;
			clone.cochinealBeetleSoul = cochinealBeetleSoul;
			clone.iceTortoiseSoul = iceTortoiseSoul;

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

			packet.Write(seaSnailSoul);
			packet.Write(lacBeetleSoul);
			packet.Write(cyanBeetleSoul);
			packet.Write(cochinealBeetleSoul);
			packet.Write(iceTortoiseSoul);

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
			else
			{
				if (clone.seaSnailSoul != this.seaSnailSoul ||
					clone.lacBeetleSoul != this.lacBeetleSoul ||
					clone.cyanBeetleSoul != this.cyanBeetleSoul ||
					clone.cochinealBeetleSoul != this.cochinealBeetleSoul ||
					clone.iceTortoiseSoul != this.iceTortoiseSoul)
				{
					ModPacket packet = mod.GetPacket();
					packet.Write((byte)MysticHunterMessageType.SyncPlayerSouls);
					packet.Write((byte)player.whoAmI);
					packet.Write(seaSnailSoul);
					packet.Write(lacBeetleSoul);
					packet.Write(cyanBeetleSoul);
					packet.Write(cochinealBeetleSoul);
					packet.Write(iceTortoiseSoul);
					packet.Send();
				}
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

		private bool CheckSoulMana(int amount, bool pay = false, bool blockQuickMana = false)
		{
			if (player.statMana >= amount)
			{
				if (pay)
					player.statMana -= amount;
				return (true);
			}
			if (!player.manaFlower || blockQuickMana)
				return (false);
			player.QuickMana();
			return CheckSoulMana(amount, pay, true);
		}

		#endregion
	}
}
