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
    internal class ZombieHead : ModProjectile
    {
        
        ref float frame => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 15;
        }
        
        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.damage = 3;
            Projectile.knockBack = 0;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1800;
            Projectile.width = 32;
            Projectile.height = 30;
        }

        public override void AI()
        {
            Projectile.frame = (int)frame;
        }
    }
}
