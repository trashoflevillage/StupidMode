using Microsoft.Xna.Framework;
using StupidMode.Common.System.Util;
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
    internal class StupidNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool child;

        public IDictionary<string, Cooldown> cooldowns = new Dictionary<string, Cooldown>();
        public IDictionary<string, bool> triggers = new Dictionary<string, bool>();

        public override void SetDefaults(NPC npc)
        {
            if (!npc.friendly)
            {
                if (!npc.boss)
                {
                    npc.lifeMax += npc.lifeMax / 2;
                    npc.life = npc.lifeMax;
                }
                npc.trapImmune = true;
                npc.lavaImmune = true;
            }

            // Initialize boss ability cooldowns
            {
                NewCooldown(npc, NPCID.KingSlime, "slimeSpike", 40);
                NewCooldown(npc, NPCID.SkeletronHead, "waterbolt", 80);
                NewCooldown(npc, NPCID.EyeofCthulhu, "boulderThrow", 420);
                NewCooldown(npc, NPCID.EyeofCthulhu, "boulderChargeup", 180, -1);
            }
        }

        public override void OnKill(NPC npc)
        {
            if (CanSplit(npc))
                for (int i = 0; i < 2; i++)
                {
                    int npcIndex = NPC.NewNPC(npc.GetSource_Death(), (int)npc.position.X, (int)npc.position.Y, npc.type);
                    NPC newNPC = Main.npc[npcIndex];
                    newNPC.SpawnedFromStatue = true;
                    newNPC.lifeMax *= 2;
                    newNPC.life = newNPC.lifeMax;
                    newNPC.scale *= 0.8f;
                    newNPC.value = 0;
                    StupidNPC newModNPC = newNPC.GetGlobalNPC<StupidNPC>();
                    newModNPC.child = true;
                }

            if (npc.type == NPCID.SkeletronHand)
            {
                for (int i = 0; i < 3; i++)
                {
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X, (int)npc.position.Y, NPCID.DarkCaster);
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X, (int)npc.position.Y, NPCID.AngryBonesBigMuscle);
                }
            }

            if (npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail)
            {
                for (int i = 0; i < Main.rand.Next(4, 8); i++)
                {
                    int index = NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 16)), ProjectileID.CursedFlameFriendly, npc.damage, 3f);
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.KingSlime)
            {
                if (npc.velocity.Y == 0) npc.reflectsProjectiles = true;
                else npc.reflectsProjectiles = false;
            }

            return base.PreAI(npc);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ProjectileID.Boulder)
            {
                damage = 0;
                knockback = 0;
                crit = false;
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            OnHitByAnything(npc, damage, knockback, crit);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            OnHitByAnything(npc, damage, knockback, crit);
        }

        public override bool PreKill(NPC npc)
        {
            if (npc.type == NPCID.ServantofCthulhu) {
                Projectile.NewProjectile(npc.GetSource_Death(), npc.position, npc.velocity * -1, ProjectileID.Boulder, 100, 5);
            }
            return base.PreKill(npc);
        }

        public void OnHitByAnything(NPC npc, int damage, float knockback, bool crit)
        {
        }

        public override void AI(NPC npc)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();

            if (npc.type == NPCID.KingSlime)
            {
                if (npc.life <= npc.lifeMax / 2)
                {
                    if (cooldowns["slimeSpike"].TickCooldown())
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            float velX = Main.rand.NextFloat(1, 16);
                            float velY = Main.rand.NextFloat(1, 16) * -1;
                            if (Main.rand.NextBool()) velX *= -1;
                            NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(velX, velY), ProjectileID.SpikedSlimeSpike, npc.damage, 3f);
                        }
                    }
                }
            }

            if (npc.type == NPCID.EyeofCthulhu)
            {
                if ((!Main.expertMode && !Main.masterMode && npc.life <= npc.lifeMax * 0.5) || ((Main.expertMode || Main.masterMode) && npc.life <= npc.lifeMax * 0.65))
                {
                    if (cooldowns["boulderChargeup"].val == -1 && cooldowns["boulderThrow"].TickCooldown())
                    {
                        cooldowns["boulderChargeup"].val = 0;
                    }

                    if (cooldowns["boulderChargeup"].val >= 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = npc.Center;
                            pos.Y += Main.rand.Next((npc.height / 2) * -1, (npc.height / 2));
                            pos.X += Main.rand.Next((npc.width / 2) * -1, (npc.width / 2));
                            Dust.NewDust(pos, 4, 4, DustID.Electric);
                        }

                        if (cooldowns["boulderChargeup"].TickCooldown())
                        {
                            for (int i = 0; i < Main.rand.Next(4, 8); i++)
                            {
                                NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-16, 0)), ProjectileID.Boulder, npc.damage * 8, 3f);
                            }
                            cooldowns["boulderChargeup"].val = -1;
                        }
                    }
                }
            }

            if (npc.type == NPCID.SkeletronHead)
            {
                if (cooldowns["waterbolt"].TickCooldown())
                {
                    int timeLeft = 240;
                    int index = NewHostileProjectile(npc.GetSource_FromAI(), npc.position, new Vector2(5, 5), ProjectileID.WaterBolt, npc.damage, 3);
                    Main.projectile[index].timeLeft = timeLeft;

                    index = NewHostileProjectile(npc.GetSource_FromAI(), npc.position, new Vector2(5, -5), ProjectileID.WaterBolt, npc.damage, 3);
                    Main.projectile[index].timeLeft = timeLeft;

                    index = NewHostileProjectile(npc.GetSource_FromAI(), npc.position, new Vector2(-5, 5), ProjectileID.WaterBolt, npc.damage, 3);
                    Main.projectile[index].timeLeft = timeLeft;

                    index = NewHostileProjectile(npc.GetSource_FromAI(), npc.position, new Vector2(-5, -5), ProjectileID.WaterBolt, npc.damage, 3);
                    Main.projectile[index].timeLeft = timeLeft;
                }
            }

            if (npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail)
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (Framing.GetTileSafely((int)(npc.position.X / 16) + x, (int)(npc.position.Y / 16) + y)
                            .TileType == TileID.Platforms) WorldGen.KillTile((int)(npc.position.X / 16) + x, (int)(npc.position.Y / 16) + y, false, false, true);
                    }
                }
            }

            if (npc.type == NPCID.BrainofCthulhu)
            {
                if (NPC.FindFirstNPC(NPCID.Creeper) == -1 && modNPC.TryTrigger("enteredPhaseTwo"))
                {
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.NebulaBrain);
                }
            }
        }
        
        /// <summary>
        /// Returns true or false depending on if the NPC can split into children or not.
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static bool CanSplit(NPC npc)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
            int[] exceptions = new int[] {
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.Creeper,
                NPCID.ServantofCthulhu
            };
            if (!npc.friendly && !modNPC.child && !npc.boss && !exceptions.Contains(npc.type)) return true;
            return false;
        }

        public void NewCooldown(NPC npc, int npcType, string key, int counterMax, int? defaultVal = null)
        {
            if (npc.type == npcType)
            {
                cooldowns[key] = new Cooldown(counterMax, defaultVal);
            }
        }

        /// <summary>
        /// Attempts to set a trigger to be triggered.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if the trigger was not already triggered</returns>
        public bool TryTrigger(string key)
        {
            if (!triggers.ContainsKey(key) || !triggers[key])
            {
                triggers[key] = true;
                return true;
            }
            return false;
        }

        public int NewHostileProjectile(IEntitySource spawnSource, Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack, int Owner = 255, float ai0 = 0, float ai1 = 0)
        {
            int whoAmI = Projectile.NewProjectile(spawnSource, position, velocity, Type, Damage, KnockBack, Owner, ai0, ai1);
            Main.projectile[whoAmI].friendly = false;
            Main.projectile[whoAmI].hostile = true;
            return whoAmI;
        }
    }
}
