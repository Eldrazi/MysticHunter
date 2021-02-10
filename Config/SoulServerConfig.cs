#region Using directives

using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Terraria;
using Terraria.ModLoader.Config;

#endregion

namespace MysticHunter.Config
{
	internal sealed class SoulServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[Slider]
		[Range(1, 50)]
		[DefaultValue(1)]
		[Label("Soul Base Drop Chance %")]
		[Tooltip("The base percentage chance for souls to drop.")]
		public int BaseSoulDropChance;

		[Label("Guaranteed Soul Drop")]
		[Tooltip("A configurable list of NPCs that should drop a guaranteed soul.")]
		public List<NPCDefinition> GuaranteedSoulDrops { get; set; } = new List<NPCDefinition>();

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			BaseSoulDropChance = Utils.Clamp(BaseSoulDropChance, 1, 50);
		}
	}
}
