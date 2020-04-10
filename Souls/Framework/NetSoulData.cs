namespace MysticHunter.Souls.Framework
{
	/// <summary>
	/// Mainly used where netsyncing and saving/loading is crucial (ea <see cref="SoulPlayer"/>).
	/// </summary>
	public class NetSoulData
	{
		public short soulNPC;
		public byte stack;

		public NetSoulData(short soulNPC = 0, byte stack = 0)
		{
			this.soulNPC = soulNPC;
			this.stack = stack;
		}

		public static bool operator ==(NetSoulData data, NetSoulData other) => (data.soulNPC == other.soulNPC && data.stack == other.stack);
		public static bool operator !=(NetSoulData data, NetSoulData other) => !(data == other);
	}
}
