#region Using directives

using Terraria.ModLoader.IO;
using Terraria.ModLoader.Config;

#endregion

namespace MysticHunter.Souls.Framework
{
	public class SoulNPCDefinition : NPCDefinition
	{
		public byte stack;

		public SoulNPCDefinition(string mod, string name, byte stack) : base(mod, name)
		{
			this.stack = stack;
		}

		public new TagCompound SerializeData()
		{
			return new TagCompound
			{
				["mod"] = mod,
				["name"] = name,
				["stack"] = stack
			};
		}

		public static new SoulNPCDefinition Load(TagCompound tag)
		{
			return (new SoulNPCDefinition(tag.GetString("mod"), tag.GetString("name"), tag.Get<byte>("stack")));
		}
	}
}
