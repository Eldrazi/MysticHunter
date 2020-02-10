using System.Collections.Generic;

using Terraria;
using Terraria.UI;
using Terraria.ModLoader;

using MysticHunter.Souls.UI;

using Microsoft.Xna.Framework;

namespace MysticHunter
{
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

		public int selectedItem = 0;

		public MysticHunter() { }

		public override void Load()
		{
			Instance = this;

			if (!Main.dedServ)
			{
				SetupSoulHotkeys();
				SetupSoulUI();
			}
		}
		public override void Unload() => Instance = null;

		internal void SetupSoulHotkeys()
		{
			RedSoulActive = RegisterHotKey("Red Soul Active", "Z");
			BlueSoulActive = RegisterHotKey("Blue Soul Active", "X");
		}
		internal void SetupSoulUI()
		{
			soulIndexUI = new SoulIndexUI();
			soulIndexUI.Activate();
			siUserInterface = new UserInterface();
			siUserInterface.SetState(soulIndexUI);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			Player player = Main.LocalPlayer;
			if (player.selectedItem < 10)
				selectedItem = player.selectedItem;

			if (siUserInterface != null && SoulIndexUI.visible)
				siUserInterface.Update(gameTime);
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
			}
		}
	}
}
