﻿using System.IO;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;

using MysticHunter.Souls.UI;
using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;
using MysticHunter.API.Loading;

namespace MysticHunter
{
	public enum MysticHunterMessageType : byte
	{
		SyncStartSoulPlayer,
		SyncPlayerSouls
	}

	public class MysticHunter : Mod
	{
		// Singleton instance for the MysticHunter class, set in `Load`.
		public static MysticHunter Instance;

		// Two hotkeys for use with active souls.
		public ModHotKey RedSoulActive;
		public ModHotKey BlueSoulActive;

		// UI components required for the Soul Index UI.
		internal SoulIndexUI soulIndexUI;
		internal UserInterface siUserInterface;

		internal SoulIndexUIOpenClose soulIndexUIOpenClose;
		internal UserInterface siOpenButtonUserInterface;

		/// <summary>
		/// A dictionary that keeps track of all soul data.
		/// Set in the `Load` method via `SoulManager.LoadSouls`.
		/// </summary>
		private Dictionary<short, BaseSoul> _soulDict;
		public Dictionary<short, BaseSoul> SoulDict
		{
			get
			{
				if (_soulDict == null)
					_soulDict = new Dictionary<short, BaseSoul>();
				return _soulDict;
			}
			set
			{
				_soulDict = value;
			}
		}

		public override void Load()
		{
			Instance = this;

			// Initialize the loading funneler and subscribe this mod to the registry.
			LoadingFunneler.Load();
			RegistryLoader.RegisterMod(this);
		}
		public override void Unload()
		{
			LoadingFunneler.Unload();

			Instance = null;
		}

		public override void PostAddRecipes()
			=> LoadingFunneler.PostLoad();

		public override void UpdateUI(GameTime gameTime)
		{
			if (siUserInterface != null && SoulIndexUI.visible)
				siUserInterface.Update(gameTime);

			if (siOpenButtonUserInterface != null && SoulIndexUIOpenClose.visible)
				siOpenButtonUserInterface.Update(gameTime);
		}
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			Player P = Main.player[Main.myPlayer];

			int InventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (InventoryIndex != -1)
			{
				layers.Insert(InventoryIndex + 1, new LegacyGameInterfaceLayer(
					"Mystic Hunter: Soul Index UI",
					delegate
					{
						if (SoulIndexUI.visible)
							siUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(InventoryIndex + 1, new LegacyGameInterfaceLayer(
					"Mystic Hunter: Soul Index UI Open Close",
					delegate
					{
						if (SoulIndexUIOpenClose.visible)
							siOpenButtonUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		#region Network Package Handling

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MysticHunterMessageType msgType = (MysticHunterMessageType)reader.ReadByte();
			switch (msgType)
			{
				case MysticHunterMessageType.SyncPlayerSouls:
				case MysticHunterMessageType.SyncStartSoulPlayer:
					byte playerID = reader.ReadByte();
					SoulPlayer targetPlayer = Main.player[playerID].GetModPlayer<SoulPlayer>();
					
					targetPlayer.activeSouls[0].soulNPC = reader.ReadInt16();
					targetPlayer.activeSouls[1].soulNPC = reader.ReadInt16();
					targetPlayer.activeSouls[2].soulNPC = reader.ReadInt16();
					targetPlayer.activeSouls[0].stack = reader.ReadByte();
					targetPlayer.activeSouls[1].stack = reader.ReadByte();
					targetPlayer.activeSouls[2].stack = reader.ReadByte();

					if (msgType == MysticHunterMessageType.SyncPlayerSouls && Main.netMode == NetmodeID.Server)
					{
						var packet = GetPacket();
						packet.Write((byte)MysticHunterMessageType.SyncPlayerSouls);
						packet.Write(playerID);
						packet.Write(targetPlayer.activeSouls[0].soulNPC);
						packet.Write(targetPlayer.activeSouls[1].soulNPC);
						packet.Write(targetPlayer.activeSouls[2].soulNPC);
						packet.Write(targetPlayer.activeSouls[0].stack);
						packet.Write(targetPlayer.activeSouls[1].stack);
						packet.Write(targetPlayer.activeSouls[2].stack);
						packet.Send(-1, playerID);
					}

					if (msgType == MysticHunterMessageType.SyncStartSoulPlayer)
					{
						targetPlayer.lacBeetleSoul = reader.ReadBoolean();
						targetPlayer.cyanBeetleSoul = reader.ReadBoolean();
						targetPlayer.cochinealBeetleSoul = reader.ReadBoolean();

						targetPlayer.eocSoulDash = reader.ReadBoolean();
					}
					break;
			}
		}

		#endregion
	}
}
