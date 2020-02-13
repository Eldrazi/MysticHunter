using System.Linq;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using MysticHunter.Souls.Framework;

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
		public ISoul[] souls;
		public ISoul RedSoul
		{
			get { return souls[0]; }
			set { souls[0] = value; }
		}
		public ISoul BlueSoul
		{
			get { return souls[1]; }
			set { souls[1] = value; }
		}
		public ISoul YellowSoul
		{
			get { return souls[2]; }
			set { souls[2] = value; }
		}

		public byte[] soulsStack;

		private short redSoulCooldown, blueSoulCooldown, yellowSoulCooldown;

		/// <summary>
		/// Initializes the `souls` array.
		/// Done in this function so every player has his/her own instance.
		/// </summary>
		public override void Initialize()
		{
			this.souls = new ISoul[3];
			this.soulsStack = new byte[3] { 1, 1, 1 };
		}

		/// <summary>
		/// Used to process Yellow soul passives, if available.
		/// </summary>
		public override void PostUpdateEquips()
		{
			if (YellowSoul != null && yellowSoulCooldown == 0)
				YellowSoul.SoulUpdate(player, soulsStack[2]);

			if (redSoulCooldown > 0)
				redSoulCooldown--;
			if (blueSoulCooldown > 0)
				blueSoulCooldown--;
			if (yellowSoulCooldown > 0)
				yellowSoulCooldown--;
		}

		/// <summary>
		/// Used to process Red and Blue soul active triggers/hotkeys, if available.
		/// </summary>
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (RedSoul != null && redSoulCooldown <= 0 && MysticHunter.Instance.RedSoulActive.JustPressed)
			{
				if (player.CheckMana(RedSoul.ManaCost(player, soulsStack[0]), true, false) && RedSoul.SoulUpdate(player, soulsStack[0]))
					redSoulCooldown = RedSoul.cooldown;
			}

			if (BlueSoul != null && blueSoulCooldown <= 0 && MysticHunter.Instance.BlueSoulActive.JustPressed)
			{
				if (player.CheckMana(BlueSoul.ManaCost(player, soulsStack[1]), true, false) && BlueSoul.SoulUpdate(player, soulsStack[1]))
					blueSoulCooldown = BlueSoul.cooldown;
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
				tag.Add("soulStack" + i, soulsStack[i]);
			}

			// A simple query to get every soul that the player has unlocked and store it in a list for saving purposed.
			List<short> acquiredSouls = MysticHunter.Instance.SoulDict.Values.Where(v => v.acquired == true).Select(v => v.soulNPC).ToList();
			tag.Add("acquiredSouls", acquiredSouls);

			return tag;
		}
		public override void Load(TagCompound tag)
		{
			try
			{
				souls = new ISoul[3];
				soulsStack = new byte[3];
				for (int i = 0; i < souls.Length; i++)
				{
					short id = tag.GetShort("soul" + i);
					if (id != 0)
						souls[i] = MysticHunter.Instance.SoulDict[id];

					soulsStack[i] = tag.GetByte("soulStack" + i);
					if (soulsStack[i] <= 0)
						soulsStack[i] = 1;
				}

				SoulManager.ResetSoulAcquisition((List<short>)tag.GetList<short>("acquiredSouls"));
				SoulManager.RepopulateSoulIndexUI();
			}
			catch { }
		}
	}
}
