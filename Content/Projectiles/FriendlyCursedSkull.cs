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
    internal class FriendlyCursedSkull : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.damage = 20;
            Projectile.knockBack = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 50;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 160;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.alpha++;
        }
    }
}
