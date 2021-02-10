#region Using directives

using MysticHunter.Common.GlobalItems;
using Terraria;
using Terraria.ModLoader;

#endregion

namespace MysticHunter.Souls.Prefixes
{
	internal sealed class SoulDropPrefix : ModPrefix
	{
		private readonly byte _power;
		
		public override PrefixCategory Category => PrefixCategory.Accessory;

		public SoulDropPrefix() { }
		public SoulDropPrefix(byte power) 
		{
			this._power = power;
		}

		public override bool Autoload(ref string name)
		{
			if (!base.Autoload(ref name))
			{
				return (false);
			}

			mod.AddPrefix("SoulSiphon", new SoulDropPrefix(1));
			mod.AddPrefix("SoulStealing", new SoulDropPrefix(2));
			mod.AddPrefix("SoulReaping", new SoulDropPrefix(3));

			return (false);
		}

		public override float RollChance(Item item)
			=> 5f;

		public override bool CanRoll(Item item)
			=> true;

		public override void Apply(Item item)
			=> item.GetGlobalItem<SoulItem>().soulDropModifier = _power;

		public override void ModifyValue(ref float valueMult)
		{
			float multiplier = 1f + 0.05f * _power;
			valueMult *= multiplier;
		}
	}
}
