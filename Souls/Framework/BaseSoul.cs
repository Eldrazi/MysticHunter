using Terraria;

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
		public bool acquired 
		{
			get { return stack >= 1; }
			set 
			{
				if (value == false)
					stack = 0;
				else if (stack == 0)
					stack = 1;
			}
		}
		public byte stack { get; set; }

		public abstract short soulNPC { get; }
		public abstract string soulDescription { get; }

		public abstract short cooldown { get; }

		public abstract SoulType soulType { get; }

		public abstract short ManaCost(Player player, short stack);
		public abstract bool SoulUpdate(Player player, short stack);

		/// <summary>
		/// An update that is called in <see cref="SoulPlayer"/> for an extra update if necessary.
		/// </summary>
		/// <param name="player">The player that has this soul equipped.</param>
		public virtual void PostUpdate(Player player) { }
	}

	public static class ISoulExtensions
	{
		public static string SoulNPCName(this BaseSoul soul)
		{
			if (soul == null || soul.soulNPC == 0)
				return ("");
			return Lang.GetNPCNameValue(soul.soulNPC);
		}
	}
}
