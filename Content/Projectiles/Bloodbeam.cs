using Microsoft.Xna.Framework;
using StupidMode.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Content.Projectiles
{
    internal class Bloodbeam : ModProjectile
    {
		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Default;
			Projectile.extraUpdates = 80;
			Projectile.timeLeft = 800;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
		}

		// Note, this Texture is actually just a blank texture, FYI.
		public override string Texture => "StupidMode/Assets/Textures/Empty";

		public override void AI()
		{
			Projectile.position += Projectile.velocity;
			int dust = Dust.NewDust(Projectile.position, 1, 1, 178, 0f, 0f, 0, new Color(255, 0, 0), 1f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].position = Projectile.position;
			Main.dust[dust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
			Main.dust[dust].velocity *= 0.2f;
		}
	}
}
