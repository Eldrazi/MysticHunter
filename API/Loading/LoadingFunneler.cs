using Terraria;
using Terraria.UI;

using MysticHunter.Souls.UI;
using MysticHunter.Souls.Framework;
using Terraria.ModLoader;
using MysticHunter.Souls.Items;

namespace MysticHunter.API.Loading
{
	/// <summary>
	/// A loading funneler which centralizes all loading.
	/// </summary>
	internal static class LoadingFunneler
	{
		/// <summary>
		/// Initial load setup.
		/// Initializes all loaders and sets up mod-specifi content for <see cref="MysticHunter"/>.
		/// Should be called in <see cref="MysticHunter.Load"/>.
		/// </summary>
		internal static void Load()
		{
			LoadMod();
			if (!Main.dedServ)
			{
				LoadModClient();
			}
		}

		/// <summary>
		/// Finishes mod and registry loading.
		/// Loads all content from registered mods into this mod.
		/// Should be called in <see cref="MysticHunter.PostAddRecipes"/>.
		/// </summary>
		internal static void PostLoad()
		{
			RegistryLoader.LoadModContent();
		}

		/// <summary>
		/// Unloads all mod content.
		/// Should be called in <see cref="MysticHunter.Unload"/>
		/// </summary>
		internal static void Unload()
		{
			UnloadMod();
		}

		/// <summary>
		/// Called on both client and server side.
		/// </summary>
		internal static void LoadMod()
		{
			RegistryLoader.Load();
		}
		internal static void UnloadMod()
		{
			RegistryLoader.Unload();
		}

		/// <summary>
		/// Only called on clients.
		/// Loads UI and binds hotkeys.
		/// </summary>
		internal static void LoadModClient()
		{
			MysticHunter.Instance.AddEquipTexture(new PossessedArmorHead(), null, EquipType.Head, "PossessedArmorHead", "MysticHunter/Souls/Items/PossessedArmor_Head");
			MysticHunter.Instance.AddEquipTexture(new PossessedArmorBody(), null, EquipType.Body, "PossessedArmorBody", "MysticHunter/Souls/Items/PossessedArmor_Body", "MysticHunter/Souls/Items/PossessedArmor_Arms");
			MysticHunter.Instance.AddEquipTexture(new PossessedArmorLegs(), null, EquipType.Legs, "PossessedArmorLegs", "MysticHunter/Souls/Items/PossessedArmor_Legs");

			MysticHunter.Instance.AddEquipTexture(new IceTortoiseShield(), null, EquipType.Shield, "IceTortoiseShield", "MysticHunter/Souls/Items/IceTortoiseShield_Shield");

			// Bind hotkeys.
			MysticHunter.Instance.RedSoulActive = MysticHunter.Instance.RegisterHotKey("Red Soul Active", "Z");
			MysticHunter.Instance.BlueSoulActive = MysticHunter.Instance.RegisterHotKey("Blue Soul Active", "X");

			MysticHunter.Instance.SoulIndexUIHotkey = MysticHunter.Instance.RegisterHotKey("Soul Index UI", "I");

			// Initialize UI.
			MysticHunter.Instance.soulIndexUI = new SoulIndexUI();
			MysticHunter.Instance.siUserInterface = new UserInterface();

			MysticHunter.Instance.soulIndexUIOpenClose = new SoulIndexUIOpenClose();
			MysticHunter.Instance.siOpenButtonUserInterface = new UserInterface();

			MysticHunter.Instance.soulIndexUI.Activate();
			MysticHunter.Instance.siUserInterface.SetState(MysticHunter.Instance.soulIndexUI);

			MysticHunter.Instance.soulIndexUIOpenClose.Activate();
			MysticHunter.Instance.siOpenButtonUserInterface.SetState(MysticHunter.Instance.soulIndexUIOpenClose);
		}
	}
}
