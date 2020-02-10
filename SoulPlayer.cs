using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using MysticHunter.Souls.Items;

namespace MysticHunter
{
	/// <summary>
	/// ModPlayer class for handling everything soul related (active/passive updates).
	/// </summary>
	public class SoulPlayer : ModPlayer
	{
		/// <summary>
		/// An array that keeps track of soul items.
		/// Red, Blue and Yellow souls are stacked in the array in that order (0, 1, 2).
		/// </summary>
		public Item[] souls;
		private BasicSoul RedSoul => souls[0] != null ? souls[0].modItem as BasicSoul : null;
		private BasicSoul BlueSoul => souls[1] != null ? souls[1].modItem as BasicSoul : null;
		private BasicSoul YellowSoul => souls[2] != null ? souls[2].modItem as BasicSoul : null;

		/// <summary>
		/// Initializes the `souls` array.
		/// Done in this function so every player has his/her own instance.
		/// </summary>
		public override void Initialize()
		{
			this.souls = new Item[3];
		}

		/// <summary>
		/// Used to process Yellow soul passives, if available.
		/// </summary>
		public override void PostUpdateEquips()
		{
			if (YellowSoul != null)
				YellowSoul.SoulUpdate(player);
		}

		/// <summary>
		/// Used to process Red and Blue soul active triggers/hotkeys, if available.
		/// </summary>
		/// <param name="triggersSet">..</param>
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (MysticHunter.Instance.RedSoulActive.JustPressed)
				RedSoul.SoulUpdate(player);

			if (MysticHunter.Instance.BlueSoulActive.JustPressed)
				BlueSoul.SoulUpdate(player);
		}

		/// <summary>
		/// Saving and loading for soul items.
		/// </summary>
		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			for (int i = 0; i < souls.Length; ++i)
			{
				// Failsave check.
				if (souls[i] == null) souls[i] = new Item();
				tag.Add("soul" + i, ItemIO.Save(souls[i]));
			}
			return tag;
		}
		public override void Load(TagCompound tag)
		{
			try
			{
				souls = new Item[3];
				for (int i = 0; i < souls.Length; i++)
				{
					Item item = tag.Get<Item>("soul" + i);
					souls[i] = item;
				}
			}
			catch { }
		}
	}
}
