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
    internal class MeanBeenade : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Explosive;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.damage = 20;
            Projectile.knockBack = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Explode();
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 1) Explode();
        }

        public void Explode() {
            Vector2 pos = Projectile.position;
            SoundEngine.PlaySound(SoundID.Item14, pos);
            for (int a = 0; a < Main.rand.Next(2, 5); a++)
            {
                int size = Main.rand.Next(4, 16);
                Dust.NewDust(pos, size, size, DustID.Smoke, Main.rand.NextFloat(3f), Main.rand.NextFloat(3f));
            }
            int index = NPC.NewNPC(Projectile.GetSource_NaturalSpawn(), (int)pos.X + Main.rand.Next(-1, 1), (int)pos.Y + Main.rand.Next(-1, 1), Main.rand.Next(NPCID.Bee, NPCID.BeeSmall + 1));
            Main.npc[index].velocity = new Vector2(0, 0);
            Main.npc[index].target = Main.npc[NPC.FindFirstNPC(NPCID.QueenBee)].target;
            Main.npc[index].GetGlobalNPC<StupidNPC>().child = true;
            foreach (Player i in Main.player)
            {
                if (i.Distance(pos) <= 1f)
                {
                    i.Hurt(PlayerDeathReason.ByProjectile(i.whoAmI, Projectile.whoAmI), Projectile.damage, 0);
                }
            }
        }
    }
}
