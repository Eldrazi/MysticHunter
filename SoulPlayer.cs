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
		public NetSoulData[,] activeSouls;
		public int activeSoulConfig = 0;

		public BaseSoul RedSoul
		{
			get { return RedSoulNet.soulNPC == 0 ? null : SoulManager.GetSoul(RedSoulNet.soulNPC); }
			set
			{
				if (value == null)
					RedSoulNet.soulNPC = 0;
				else
				{
					RedSoulNet.soulNPC = value.soulNPC;
					RedSoulNet.stack = UnlockedSouls[value.soulNPC];
				}
			}
		}
		public BaseSoul BlueSoul
		{
			get { return BlueSoulNet.soulNPC == 0 ? null : SoulManager.GetSoul(BlueSoulNet.soulNPC); }
			set
			{
				if (value == null)
					BlueSoulNet.soulNPC = 0;
				else
				{
					BlueSoulNet.soulNPC = value.soulNPC;
					BlueSoulNet.stack = UnlockedSouls[value.soulNPC];
				}
			}
		}
		public BaseSoul YellowSoul
		{
			get { return YellowSoulNet.soulNPC == 0 ? null : SoulManager.GetSoul(YellowSoulNet.soulNPC); }
			set
			{
				if (value == null)
					YellowSoulNet.soulNPC = 0;
				else
				{
					YellowSoulNet.soulNPC = value.soulNPC;
					YellowSoulNet.stack = UnlockedSouls[value.soulNPC];
				}
			}
		}

		public NetSoulData RedSoulNet
		{
			get { return activeSouls[activeSoulConfig, (int)SoulType.Red]; }
		}
		public NetSoulData BlueSoulNet
		{
			get { return activeSouls[activeSoulConfig, (int)SoulType.Blue]; }
		}
		public NetSoulData YellowSoulNet
		{
			get { return activeSouls[activeSoulConfig, (int)SoulType.Yellow]; }
		}

		public float[] soulDropModifier;
		public readonly float[] DefinedSoulDropModifier = new float[3] { 0.015f, 0.015f, 0.015f };

		public PreHurtModifier preHurtModifier = null;

		// Soul accessory variables.
		public bool ChaosStone = false;
		public bool BraceOfEvil = false;
		public bool LunarRitual = false;
		public bool QueenKnuckle = false;

		// Yellow soul booleans.
		public bool yetiSoul = false;
		public bool pinkySoul = false;
		public bool lamiaSoul = false;
		public bool ghostSoul = false;
		public bool seaSnailSoul = false;
		public bool poltergeistSoul = false;
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
			{
				this.activeSouls = new NetSoulData[3, 3] {
					{ new NetSoulData(), new NetSoulData(), new NetSoulData() },
					{ new NetSoulData(), new NetSoulData(), new NetSoulData() },
					{ new NetSoulData(), new NetSoulData(), new NetSoulData() }
				};
			}

			this.soulDropModifier = new float[3] { DefinedSoulDropModifier[0], DefinedSoulDropModifier[1], DefinedSoulDropModifier[2] };
		}

		public override void ResetEffects()
		{
			preHurtModifier = null;

			ChaosStone = false;
			BraceOfEvil = false;
			LunarRitual = false;
			QueenKnuckle = false;

			yetiSoul = false;
			pinkySoul = false;
			lamiaSoul = false;
			ghostSoul = false;
			seaSnailSoul = false;
			poltergeistSoul = false;
			undeadMinerSoul = false;
			torturedSoulSoul = false;
			possessedArmorSoul = false;
			tacticalSkeletonSoul = false;

			if (BlueSoulNet.soulNPC != NPCID.LacBeetle)
				lacBeetleSoul = false;
			if (BlueSoulNet.soulNPC != NPCID.CyanBeetle)
				cyanBeetleSoul = false;
			if (BlueSoulNet.soulNPC != NPCID.CochinealBeetle)
				cochinealBeetleSoul = false;

			if (BlueSoulNet.soulNPC != NPCID.EyeofCthulhu)
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
			if (YellowSoulNet.soulNPC != 0 && !player.HasBuff(BuffType<YellowSoulDebuff>()))
			{
				BaseSoul soulReference = YellowSoul;
				soulReference.SoulUpdate(player, YellowSoulNet.stack);

				if (soulReference.cooldown > 0)
				{
					player.AddBuff(BuffType<YellowSoulDebuff>(), soulReference.cooldown);
				}
			}

			RedSoul?.PostUpdate(player);
			BlueSoul?.PostUpdate(player);
			YellowSoul?.PostUpdate(player);
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (lacBeetleSoul)
				LacBeetleSoul.ModifyHit(player, ref damage, damageSource, BlueSoulNet.stack);
			if (cyanBeetleSoul)
				CyanBeetleSoul.ModifyHit(player, ref damage, damageSource, BlueSoulNet.stack);
			if (cochinealBeetleSoul)
				CochinealBeetleSoul.ModifyHit(player, ref damage, damageSource, BlueSoulNet.stack);
			if (iceTortoiseSoul)
				IceTortoiseSoul.ModifyHit(player, ref damage, damageSource, BlueSoulNet.stack);

			return (preHurtModifier?.Invoke(player, ref damage, damageSource, YellowSoulNet.stack) ?? true);
		}

		public override bool? CanHitNPC(Item item, NPC target)
		{
			if (target.friendly)
				return (this.torturedSoulSoul);
			return (null);
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
			=> YellowSoul?.OnHitNPC(player, target, item, ref damage, YellowSoulNet.stack);
		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
			=> YellowSoul?.OnHitNPC(player, target, proj, ref damage, YellowSoulNet.stack);

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			if ((this.BraceOfEvil && Main.rand.Next(100) < 15) || this.ChaosStone)
			{
				if (this.ChaosStone)
				{
					target.AddBuff(BuffID.Ichor, 120);
					target.AddBuff(BuffID.CursedInferno, 120);
				}
				else if (WorldGen.crimson)
					target.AddBuff(BuffID.Ichor, 120);
				else
					target.AddBuff(BuffID.CursedInferno, 120);
			}
			if (this.QueenKnuckle)
			{
				if (!this.ChaosStone)
					target.AddBuff(BuffID.Poisoned, 180);
				if (this.ChaosStone || Main.rand.Next(100) < 15)
					target.AddBuff(BuffID.Venom, 120);
			}
			if (this.LunarRitual)
			{
				target.AddBuff(BuffID.Daybreak, 120);
			}
		}

		/// <summary>
		/// Used to process Red and Blue soul active triggers/hotkeys, if available.
		/// </summary>
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			// Using a temporary BaseSoul reference to diminish the amount of times we have to call 
			BaseSoul tmpSoulRef;

			if (RedSoulNet.soulNPC != 0 && !player.HasBuff(BuffType<RedSoulDebuff>()) && MysticHunter.Instance.RedSoulActive.Current)
			{
				tmpSoulRef = RedSoul;
				if (CheckSoulMana(tmpSoulRef.ManaCost(player, RedSoulNet.stack), false) &&
					tmpSoulRef.SoulUpdate(player, RedSoulNet.stack))
				{
					player.manaRegenDelay = (int)player.maxRegenDelay;
					player.AddBuff(BuffType<RedSoulDebuff>(), tmpSoulRef.cooldown);
					CheckSoulMana(tmpSoulRef.ManaCost(player, RedSoulNet.stack), true);
				}
			}

			if (BlueSoulNet.soulNPC != 0 && !player.HasBuff(BuffType<BlueSoulDebuff>()) && MysticHunter.Instance.BlueSoulActive.Current)
			{
				tmpSoulRef = BlueSoul;
				if (CheckSoulMana(tmpSoulRef.ManaCost(player, BlueSoulNet.stack), false) &&
					tmpSoulRef.SoulUpdate(player, BlueSoulNet.stack))
				{
					player.manaRegenDelay = (int)player.maxRegenDelay;
					player.AddBuff(BuffType<BlueSoulDebuff>(), tmpSoulRef.cooldown);
					CheckSoulMana(tmpSoulRef.ManaCost(player, BlueSoulNet.stack), true);
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
				player.head = Mod.GetEquipSlot("PossessedArmorHead", EquipType.Head);
				player.body = Mod.GetEquipSlot("PossessedArmorBody", EquipType.Body);
				player.legs = Mod.GetEquipSlot("PossessedArmorLegs", EquipType.Legs);
			}
			
			if (this.iceTortoiseSoul)
				player.shield = (sbyte)Mod.GetEquipSlot("IceTortoiseShield", EquipType.Shield);
		}

		public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (this.tacticalSkeletonSoul && item.useAmmo == AmmoID.Bullet)
			{
				int amount = 1;
				if (YellowSoulNet.stack >= 5)
					amount++;
				if (YellowSoulNet.stack >= 9)
					amount++;

				for (int i = 0; i < amount; ++i)
				{
					Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY).RotatedByRandom(.2f), type, damage / 2, knockBack, player.whoAmI);
				}
			}
			return (true);
		}

		// TODO: Eldrazi - No longer a proper hook, needs to be ported to layer objects.
		/*public override void ModifyDrawLayers(PlayerDrawSet drawInfo)
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
		}*/

		public override void OnEnterWorld(Player player)
			=> SoulManager.ReloadSoulIndexUI();

		/// <summary>
		/// Saving and loading for soul data.
		/// </summary>
		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound
			{
				{ "acquiredSoulsKeys", UnlockedSouls.Keys.ToList() },
				{ "acquiredSoulsValues", UnlockedSouls.Values.ToList() }
			};

			for (int i = 0; i < activeSouls.GetLength(0); ++i)
			{
				for (int j = 0; j < activeSouls.GetLength(1); ++j)
				{
					tag.Add("soul" + i + ":" + j, activeSouls[i, j].soulNPC);
				}
			}
			return tag;
		}
		public override void Load(TagCompound tag)
		{
			try
			{
				var keys = tag.Get<List<short>>("acquiredSoulsKeys");
				var values = tag.Get<List<byte>>("acquiredSoulsValues");
				UnlockedSouls = keys.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);

				activeSouls = new NetSoulData[3, 3];
				for (int i = 0; i < activeSouls.GetLength(0); i++)
				{
					for (int j = 0; j < activeSouls.GetLength(1); j++)
					{
						short soulNPC = tag.GetShort("soul" + i + ":" + j);

						activeSouls[i, j] = new NetSoulData();
						if (SoulManager.GetSoul(soulNPC) != null)
						{
							if (UnlockedSouls.TryGetValue(soulNPC, out byte result))
							{
								activeSouls[i, j].stack = result;
								activeSouls[i, j].soulNPC = soulNPC;
							}
						}
					}
				}
			}
			catch
			{
				UnlockedSouls = new Dictionary<short, byte>();
				activeSouls = new NetSoulData[3, 3] {
					{ new NetSoulData(), new NetSoulData(), new NetSoulData() },
					{ new NetSoulData(), new NetSoulData(), new NetSoulData() },
					{ new NetSoulData(), new NetSoulData(), new NetSoulData() },
				};
			}
		}

		#region Player Syncing & Networking

		public override void clientClone(ModPlayer clientClone)
		{
			SoulPlayer clone = clientClone as SoulPlayer;

			for (int i = 0; i < this.activeSouls.GetLength(0); ++i)
			{
				for (int j = 0; j < this.activeSouls.GetLength(1); ++j)
				{
					if (this.activeSouls[i, j] == null)
						this.activeSouls[i, j] = new NetSoulData();

					clone.activeSouls[i, j].stack = this.activeSouls[i, j].stack;
					clone.activeSouls[i, j].soulNPC = this.activeSouls[i, j].soulNPC;
				}
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
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)MysticHunterMessageType.SyncStartSoulPlayer);
			packet.Write((byte)player.whoAmI);

			packet.Write(RedSoulNet.soulNPC);
			packet.Write(BlueSoulNet.soulNPC);
			packet.Write(YellowSoulNet.soulNPC);

			packet.Write(RedSoulNet.stack);
			packet.Write(BlueSoulNet.stack);
			packet.Write(YellowSoulNet.stack);

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

			if (clone.activeSouls[activeSoulConfig, (int)SoulType.Red] != this.activeSouls[activeSoulConfig, (int)SoulType.Red] ||
				clone.activeSouls[activeSoulConfig, (int)SoulType.Blue] != this.activeSouls[activeSoulConfig, (int)SoulType.Blue] ||
				clone.activeSouls[activeSoulConfig, (int)SoulType.Yellow] != this.activeSouls[activeSoulConfig, (int)SoulType.Yellow])
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)MysticHunterMessageType.SyncPlayerSouls);
				packet.Write((byte)player.whoAmI);

				packet.Write(RedSoulNet.soulNPC);
				packet.Write(BlueSoulNet.soulNPC);
				packet.Write(YellowSoulNet.soulNPC);

				packet.Write(RedSoulNet.stack);
				packet.Write(BlueSoulNet.stack);
				packet.Write(YellowSoulNet.stack);
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
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)MysticHunterMessageType.SyncPlayerSoulExtras);
					packet.Write((byte)player.whoAmI);

					packet.Write(seaSnailSoul);
					packet.Write(lacBeetleSoul);
					packet.Write(cyanBeetleSoul);
					packet.Write(cochinealBeetleSoul);
					packet.Write(iceTortoiseSoul);

					packet.Write(eocSoulDash);

					packet.Send();
				}
			}
		}

		#endregion

		#region Soul Util Functions

		public void UpdateActiveSoulData()
		{
			for (int i = 0; i < this.activeSouls.GetLength(1); ++i)
			{
				if (this.activeSouls[activeSoulConfig, i].soulNPC != 0)
				{
					this.activeSouls[activeSoulConfig, i].stack = this.UnlockedSouls[this.activeSouls[activeSoulConfig, i].soulNPC];
				}
			}
		}

		public bool HasMaxSouls(short npcType)
		{
			if (!UnlockedSouls.ContainsKey(npcType))
			{
				return (false);
			}

			return (UnlockedSouls[npcType] >= 9);
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
