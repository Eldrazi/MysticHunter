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

        public override bool Equals(object other)
        {
            return Equals(other as NetSoulData);
        }
        public virtual bool Equals(NetSoulData other)
        {
            if (other == null) return (false);
            if (object.ReferenceEquals(this, other)) return (true);
            return (this.soulNPC == other.soulNPC && this.stack == other.stack);
        }

        public override int GetHashCode()
        {
            return (this.soulNPC + this.stack);
        }

        public static bool operator ==(NetSoulData item1, NetSoulData item2)
        {
            if (object.ReferenceEquals(item1, item2)) return (true);
            if ((object)item1 == null || (object)item2 == null) return (false);
            return (item1.soulNPC == item2.soulNPC && item1.stack == item2.stack);
        }
		public static bool operator !=(NetSoulData data, NetSoulData other) => !(data == other);
	}
}
