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
    internal class Bloodbolt : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterBolt);
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.damage *= 2;
            Projectile.knockBack *= 2;
        }
    }
}
