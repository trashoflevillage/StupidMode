using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Common.Global
{
    internal class StupidProjectile : GlobalProjectile
    {
        public override void SetStaticDefaults() { }

        public override void SetDefaults(Projectile projectile)
        {
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (Main.rand.NextBool(35) && projectile.type == ProjectileID.DeerclopsIceSpike)
            {
                int[] potentialNPCs = new int[]
                {
                    NPCID.SnowFlinx,
                    NPCID.UndeadViking,
                    NPCID.IceSlime,
                    NPCID.SpikedIceSlime,
                    NPCID.CyanBeetle,
                    NPCID.IceBat
                };
                int index;
                Vector2 pos;
                pos = projectile.position;
                index = StupidNPC.NewChild(Projectile.GetSource_NaturalSpawn(), (int)pos.X, (int)pos.Y, potentialNPCs[Main.rand.Next(0, potentialNPCs.Length)]);
                Main.npc[index].velocity = new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-15, -10));
                for (int a = 0; a < 10; a++)
                    Dust.NewDust(projectile.position, 10, 10, DustID.Snow, 0, 2, 0, default, Main.rand.NextFloat(0.5f, 2));
            }
        }
    }
}
