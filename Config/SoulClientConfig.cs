#region Using directives

using System.ComponentModel;

using Terraria.ModLoader.Config;

using Microsoft.Xna.Framework;

#endregion

namespace MysticHunter.Config
{
	internal sealed class SoulClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[DefaultValue(typeof(Vector2), "505, 28")]
		[Range(0f, 1920f)]
		[Label("Soul Index Position")]
		[Tooltip("The position of the Soul Index button.")]
		public Vector2 SoulIndexPosition { get; set; }
	}
}
