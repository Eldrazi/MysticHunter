﻿#region Using directives

using Terraria;
using Terraria.ModLoader.Config;

#endregion

namespace MysticHunter.Souls.Framework
{
	/// <summary>
	/// The type of a soul. Used to determine how a soul is updated internally.
	/// </summary>
	public enum SoulType
	{
		Red = 0,
		Blue = 1,
		Yellow = 2
	}

	public abstract class BaseSoul
	{
		public abstract short soulNPC { get; }

		// TODO: Eldrazi - UNUSED for now. Will probably be used when the cross-mod content is fully supported.
		public virtual string soulNPCMod => null;
		public virtual string soulNPCName { get; }
		
		public abstract short cooldown { get; }

		public abstract SoulType soulType { get; }
		
		public abstract string soulDescription { get; }

		public NPCDefinition soulNPCDefinition;

		public abstract short ManaCost(Player player, short stack);
		public abstract bool SoulUpdate(Player player, short stack);

		/// <summary>
		/// An update that is called in <see cref="SoulPlayer"/> for an extra update if necessary.
		/// </summary>
		/// <param name="player">The player that has this soul equipped.</param>
		public virtual void PostUpdate(Player player) { }
		public virtual void OnHitNPC(Player player, NPC npc, Entity hitEntity, ref int damage, byte stack) { }

		// TODO: Rewrite this method to instead use statically available memory, instead of loading onto heap?
		public virtual short[] GetAdditionalTypes() => null;

		public virtual string SoulNPCName()
		{
			if (this.soulNPC == 0)
				return ("");
			return Lang.GetNPCNameValue(this.soulNPC);
		}
	}
}
