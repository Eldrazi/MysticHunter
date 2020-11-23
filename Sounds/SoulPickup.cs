﻿using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace ExampleMod.Sounds.Item
{
	public class SoulPickup : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			soundInstance = sound.CreateInstance();
			soundInstance.Volume = volume * .5f;
			soundInstance.Pan = pan;
			return soundInstance;
		}
	}
}