#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	internal sealed class MourningWoodSoul : PostHMSoul, IEventSoul, IBossSoul
	{
		public override short soulNPC => NPCID.MourningWood;
		public override string soulDescription => "Fire a spray of Greek Fire";

		public override short cooldown => 15;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 3;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 70;
			int modifier = 1;

			if (stack >= 5)
			{
				damage += 20;
				modifier++;
			}
			if (stack >= 9)
			{
				damage += 20;
				modifier++;
			}		

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<MourningWoodSoulProj>(), damage, 1, p.whoAmI, 0, modifier);
			return (true);
		}
	}

	internal sealed class MourningWoodSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.GreekFire1;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Greek Fire");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.aiStyle = 14;
			aiType = ProjectileID.GreekFire1;

			projectile.penetrate = -1;
			projectile.timeLeft = 360;

			projectile.friendly = true;
		}

		public override Color? GetAlpha(Color lightColor) => Color.Transparent;

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			int rand = Main.rand.Next(10);
			int modifier = (int)projectile.ai[1];
			if (modifier == 1 && rand < 2)
			{
				target.AddBuff(BuffID.OnFire, 180);
			}
			else if (modifier == 2 && rand < 4)
			{
				target.AddBuff(BuffID.CursedInferno, 180);
			}
			else if (rand < 6)
			{
				target.AddBuff(BuffID.ShadowFlame, 180);
			}
		}
	}
}
