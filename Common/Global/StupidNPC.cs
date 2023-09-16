using Microsoft.Xna.Framework;
using StupidMode.Common.System.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StupidMode.Common.Global
{
    internal class StupidNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool child;
        public bool dropMeteorite;
        public bool mimicTrap;
        public bool noLoot;
        public BossMusic bossMusic;

        public Item[]? additionalLoot = null;

        public static int eaterSwarmCooldown = 24;

        public IDictionary<string, Cooldown> cooldowns = new Dictionary<string, Cooldown>();
        public IDictionary<string, bool> triggers = new Dictionary<string, bool>();


        public override void SetDefaults(NPC npc)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
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

            int[] noTileCollide = new int[]
            {
                NPCID.Bee,
                NPCID.BeeSmall,
                NPCID.Hellbat,
                NPCID.Demon,
                NPCID.Lavabat
            };

            if (noTileCollide.Contains(npc.type))
            {
                npc.noTileCollide = true;
            }

            if (npc.type == NPCID.MoonLordCore)
            {
                npc.damage = 80;
            }

            if (npc.type == NPCID.WallofFleshEye)
            {
                npc.dontTakeDamage = true;
            }

            modNPC.bossMusic = GetBossMusic(npc.type);

            // Initialize boss ability cooldowns
            {
                NewCooldown(npc, NPCID.EyeofCthulhu, "boulderThrowActivate", 420);
                NewCooldown(npc, NPCID.EyeofCthulhu, "boulderThrow", 180, true, -1);

                NewCooldown(npc, NPCID.KingSlime, "slimeSpike", 40);

                NewCooldown(npc, NPCID.SkeletronHead, "waterbolt", 80);

                NewCooldown(npc, NPCID.QueenBee, "beenadeVolleyActivate", 420);
                NewCooldown(npc, NPCID.QueenBee, "beenadeVolley", 220, true, -1);
                NewCooldown(npc, NPCID.QueenBee, "beehiveDrop", 80);

                NewCooldown(npc, NPCID.WallofFlesh, "regurgitateActivate", 2000);
                NewCooldown(npc, NPCID.WallofFlesh, "regurgitate", 180, true, -1);

                NewCooldown(npc, NPCID.WallofFleshEye, "boulderThrowWOF", 120);

                NewCooldown(npc, NPCID.Spazmatism, "bouncyBoulderThrowActivate", 340, false);
                NewCooldown(npc, NPCID.Spazmatism, "bouncyBoulderThrow", 180, false, -1);

                NewCooldown(npc, NPCID.Retinazer, "laserRingActivate", 340);
                NewCooldown(npc, NPCID.Retinazer, "laserRing", 180, true, -1);
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();

            if (modNPC.bossMusic != BossMusic.None)
            {
                StupidNPC compare;

                bool sendCredits = true;

                foreach (NPC n in Main.npc)
                {
                    if (n.active && n.TryGetGlobalNPC(out compare))
                    {
                        compare = n.GetGlobalNPC<StupidNPC>();
                        if (npc.whoAmI != n.whoAmI && modNPC.bossMusic == compare.bossMusic)
                        {
                            sendCredits = false;
                            break;
                        }
                    }
                }

                if (sendCredits) SendBossMusicCredits(modNPC.bossMusic);
            }
        }

        public override void OnKill(NPC npc)
        {
            SpecialLoot(npc);
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
            if (CanSplit(npc))
            {
                NPC baby;
                for (int i = 0; i < 2; i++)
                {
                    NewChild(npc.GetSource_Death(), (int)npc.position.X + Main.rand.Next(-1, 1), (int)npc.position.Y, npc.type);
                }
            }

            if (modNPC.dropMeteorite)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Item.NewItem(npc.GetSource_Death(), new Rectangle((int)npc.position.X, (int)npc.position.Y, 1, 1), ItemID.Meteorite, 1);
                }
            }

            if (npc.type == NPCID.SkeletronHand)
            {
                for (int i = 0; i < 3; i++)
                {
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X, (int)npc.position.Y, NPCID.DarkCaster);
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X, (int)npc.position.Y, NPCID.AngryBonesBigMuscle);
                }

                List<NPC> npcs = new List<NPC>();
                foreach (NPC i in Main.npc) {
                    if (i.type == NPCID.SkeletronHand && i.active) {
                        npcs.Add(i);
                    }
                }

                foreach (NPC i in npcs)
                {
                    i.life = Main.npc[NPC.FindFirstNPC(NPCID.SkeletronHand)].lifeMax *= 2;
                    i.life = Main.npc[NPC.FindFirstNPC(NPCID.SkeletronHand)].lifeMax;
                    i.scale *= 2;
                    i.height *= 2;
                    i.width *= 2;
                }
            }

            if (npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail)
            {
                for (int i = 0; i < Main.rand.Next(4, 8); i++)
                {
                    NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 16)), ProjectileID.CursedFlameFriendly, 10, 3f);
                }
            }

            int[] goblins = new int[]
            {
                NPCID.GoblinArcher,
                NPCID.GoblinPeon,
                NPCID.GoblinScout,
                NPCID.GoblinShark,
                NPCID.GoblinSorcerer,
                NPCID.GoblinSummoner,
                NPCID.GoblinThief,
                NPCID.GoblinTinkerer,
                NPCID.GoblinWarrior,
                NPCID.BoundGoblin
            };

            if (goblins.Contains(npc.type))
            {
                for (int i = 0; i < 6; i++)
                {
                    int? index = NewHostileProjectile(npc.GetSource_Death(), npc.Center, new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-16, -1)), ProjectileID.SpikyBall, 8, 1f);
                    Main.projectile[index.Value].timeLeft = 300;
                }
            }

            int[] skeletons = new int[]
            {
                NPCID.Skeleton,
                NPCID.SkeletonAlien,
                NPCID.SkeletonArcher,
                NPCID.SkeletonAstonaut,
                NPCID.SkeletonCommando,
                NPCID.SkeletonMerchant,
                NPCID.SkeletonSniper,
                NPCID.SkeletonTopHat,
                NPCID.ArmoredSkeleton,
                NPCID.BigHeadacheSkeleton,
                NPCID.BigMisassembledSkeleton,
                NPCID.BigPantlessSkeleton,
                NPCID.BigSkeleton,
                NPCID.BoneThrowingSkeleton,
                NPCID.BoneThrowingSkeleton2,
                NPCID.BoneThrowingSkeleton3,
                NPCID.BoneThrowingSkeleton4,
                NPCID.GreekSkeleton,
                NPCID.HeadacheSkeleton,
                NPCID.HeavySkeleton,
                NPCID.MisassembledSkeleton,
                NPCID.PantlessSkeleton,
                NPCID.SmallHeadacheSkeleton,
                NPCID.SmallMisassembledSkeleton,
                NPCID.SmallPantlessSkeleton,
                NPCID.SmallSkeleton,
                NPCID.SporeSkeleton,
                NPCID.TacticalSkeleton
            };
            
            if (skeletons.Contains(npc.type) && Main.rand.NextFloat() < 0.25f)
            {
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X, (int)npc.position.Y, ModContent.NPCType<Content.NPCs.FlyingSkull>());
            }

            if (npc.type == NPCID.EaterofWorldsTail || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead)
            {
                eaterSwarmCooldown--;
                if (eaterSwarmCooldown == 0)
                {
                    eaterSwarmCooldown = 24;
                    SoundEngine.PlaySound(SoundID.Roar, npc.position);
                    SummonEnemySwarm(npc.GetSource_FromAI(), NPCID.EaterofSouls, 50, npc.position);
                }

                if (NPC.CountNPCS(NPCID.EaterofWorldsHead) + NPC.CountNPCS(NPCID.EaterofWorldsBody) + NPC.CountNPCS(NPCID.EaterofWorldsTail) == 1)
                {
                    npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Content.Items.Accessories.ShadowHeart>(), 1, true);
                }
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ProjectileID.Boulder)
            {
                modifiers.FinalDamage *= 0;
                modifiers.Knockback *= 0;
                modifiers.DisableCrit();
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            OnHitByAnything(npc, hit, damageDone);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            OnHitByAnything(npc, hit, damageDone);
        }

        public override bool PreKill(NPC npc)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
            if (npc.type == NPCID.ServantofCthulhu) {
                Projectile.NewProjectile(npc.GetSource_Death(), npc.position, npc.velocity * -1, ProjectileID.Boulder, 100, 5);
            }

            if (modNPC.child || modNPC.mimicTrap)
            {
                modNPC.noLoot = true;
                npc.value = 0;
            }
            return base.PreKill(npc);
        }

        public void OnHitByAnything(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            if (npc.type == NPCID.WallofFlesh || npc.type == NPCID.WallofFleshEye)
            {
                int vulnerableType;
                if (npc.type == NPCID.WallofFlesh)
                {
                    vulnerableType = NPCID.WallofFleshEye;
                } else
                {
                    vulnerableType = NPCID.WallofFlesh;
                }

                foreach (NPC i in Main.npc)
                {
                    if (i.type == npc.type) i.dontTakeDamage = true;
                    else if (i.type == vulnerableType) i.dontTakeDamage = false;
                }
            }
        }

        public override void AI(NPC npc)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();

            if (npc.type == NPCID.KingSlime && modNPC.TryTrigger("spawnNinja"))
            {
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X, (int)npc.position.Y, ModContent.NPCType<Content.NPCs.NinjaMinion>());
            }

            if (cooldowns.ContainsKey("slimeSpike"))
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
                            NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(velX, velY), ProjectileID.SpikedSlimeSpike, 15, 3f);
                        }
                    }
                }
            }

            if (cooldowns.ContainsKey("boulderThrow"))
            {
                if ((!Main.expertMode && !Main.masterMode && npc.life <= npc.lifeMax * 0.5) || ((Main.expertMode || Main.masterMode) && npc.life <= npc.lifeMax * 0.65))
                {
                    if (cooldowns["boulderThrow"].val == -1 && cooldowns["boulderThrowActivate"].TickCooldown())
                    {
                        cooldowns["boulderThrow"].val = 0;
                    }

                    if (cooldowns["boulderThrow"].val >= 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = npc.Center;
                            pos.Y += Main.rand.Next((npc.height / 2) * -1, (npc.height / 2));
                            pos.X += Main.rand.Next((npc.width / 2) * -1, (npc.width / 2));
                            Dust.NewDust(pos, 4, 4, DustID.Electric);
                        }

                        if (cooldowns["boulderThrow"].TickCooldown())
                        {
                            int? index;
                            for (int i = 0; i < Main.rand.Next(4, 8); i++)
                            {
                                index = NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 0)), ProjectileID.Boulder, npc.damage * 8, 3f);
                                Main.projectile[index.Value].tileCollide = false;
                            }
                            SoundEngine.PlaySound(SoundID.Item80, npc.Center);
                            cooldowns["boulderThrow"].val = -1;
                        }
                    }
                }
            }

            /*if (cooldowns.ContainsKey("bouncyBoulderThrow"))
            {
                if (npc.life <= npc.lifeMax * 0.4)
                {
                    int otherTwin = NPC.FindFirstNPC(NPCID.Retinazer);
                    if (otherTwin != -1 && Main.npc[otherTwin].life > Main.npc[otherTwin].lifeMax * 0.4)
                    {
                        npc.dontTakeDamage = true;
                    } else
                    {
                        npc.dontTakeDamage = false;
                    }
                    if (cooldowns["bouncyBoulderThrow"].val == -1 && cooldowns["bouncyBoulderThrowActivate"].TickCooldown())
                    {
                        cooldowns["bouncyBoulderThrow"].val = 0;
                    }

                    if (cooldowns["bouncyBoulderThrow"].val >= 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = npc.Center;
                            pos.Y += Main.rand.Next((npc.height / 2) * -1, (npc.height / 2));
                            pos.X += Main.rand.Next((npc.width / 2) * -1, (npc.width / 2));
                            Dust.NewDust(pos, 4, 4, DustID.Electric);
                        }

                        if (cooldowns["bouncyBoulderThrow"].TickCooldown())
                        {
                            int? index;
                            for (int i = 0; i < 15; i++)
                            {
                                index = NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 0)), ProjectileID.BouncyBoulder, npc.damage * 8, 3f);
                                Main.projectile[index.Value].tileCollide = true;
                            }
                            SoundEngine.PlaySound(SoundID.Item80, npc.Center);
                            cooldowns["bouncyBoulderThrow"].val = -1;
                        }
                    }
                }
            }

            if (cooldowns.ContainsKey("laserRing"))
            {
                if (npc.life <= npc.lifeMax * 0.4)
                {
                    int otherTwin = NPC.FindFirstNPC(NPCID.Spazmatism);
                    if (otherTwin != -1 && Main.npc[otherTwin].life > Main.npc[otherTwin].lifeMax * 0.4)
                    {
                        npc.dontTakeDamage = true;
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                    }
                    if (cooldowns["laserRing"].val == -1 && cooldowns["laserRingActivate"].TickCooldown())
                    {
                        cooldowns["laserRing"].val = 0;
                    }

                    if (cooldowns["laserRing"].val >= 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = npc.Center;
                            pos.Y += Main.rand.Next((npc.height / 2) * -1, (npc.height / 2));
                            pos.X += Main.rand.Next((npc.width / 2) * -1, (npc.width / 2));
                            Dust.NewDust(pos, 4, 4, DustID.Electric);
                        }

                        if (cooldowns["laserRing"].TickCooldown())
                        {
                            for (int i = 0; i < Main.rand.Next(4, 8); i++)
                            {
                                Vector2[] velocities = new Vector2[]
                                {
                                    new Vector2(1, 1),
                                    new Vector2(1, -1),
                                    new Vector2(-1, 1),
                                    new Vector2(-1, -1),
                                    new Vector2(0, 1),
                                    new Vector2(1, 0),
                                    new Vector2(0, -1),
                                    new Vector2(-1, 0)
                                };

                                foreach (Vector2 v in velocities)
                                {
                                    NewHostileProjectile(npc.GetSource_FromAI(), npc.position, v * 5, ProjectileID.DeathLaser, 300, 3);
                                }
                            }
                            SoundEngine.PlaySound(SoundID.Item33, npc.Center);
                            cooldowns["laserRing"].val = -1;
                        }
                    }
                }
            }*/

            if (cooldowns.ContainsKey("waterbolt"))
            {
                if (cooldowns["waterbolt"].TickCooldown())
                {
                    Vector2[] velocities = new Vector2[]
                    {
                        new Vector2(5, 5),
                        new Vector2(5, -5),
                        new Vector2(-5, 5),
                        new Vector2(-5, -5)
                    };

                    foreach (Vector2 v in velocities)
                    {
                        int? index = NewHostileProjectile(npc.GetSource_FromAI(), npc.position, v, ProjectileID.WaterBolt, 20, 3);
                        Main.projectile[index.Value].timeLeft = 240;
                    }
                }
            }

            if (npc.type == NPCID.BrainofCthulhu)
            {
                if (NPC.FindFirstNPC(NPCID.Creeper) == -1 && modNPC.TryTrigger("enteredPhaseTwo"))
                {
                    int[] npcs = SummonEnemySwarm(npc.GetSource_FromAI(), NPCID.NebulaBrain, 15, npc.Center);
                    foreach (int i in npcs)
                    {
                        Main.npc[i].life = 1;
                        Main.npc[i].defense = 999;
                    }
                }
            }

            if (cooldowns.ContainsKey("beenadeVolley"))
            {
                if (cooldowns["beenadeVolley"].val == -1 && cooldowns["beenadeVolleyActivate"].TickCooldown())
                {
                    cooldowns["beenadeVolley"].val = 0;
                }

                if (cooldowns["beenadeVolley"].val >= 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 pos = npc.Center;
                        pos.Y += Main.rand.Next((npc.height / 2) * -1, (npc.height / 2));
                        pos.X += Main.rand.Next((npc.width / 2) * -1, (npc.width / 2));
                        Dust.NewDust(pos, 2, 2, DustID.Honey);
                        Dust.NewDust(pos, 2, 2, DustID.Honey2);
                    }

                    if (cooldowns["beenadeVolley"].TickCooldown())
                    {
                        Vector2 spawnPos = npc.Center;
                        spawnPos.Y -= npc.height / 2;
                        Vector2 vel = new Vector2(0, 5);
                        for (float i = -2; i < 3; i++)
                        {
                            vel.X = i;
                            NewHostileProjectile(npc.GetSource_FromAI(), spawnPos, vel, ModContent.ProjectileType<Content.Projectiles.MeanBeenade>(), 60, 0);
                        }
                        SoundEngine.PlaySound(SoundID.Item64, npc.Center);
                        cooldowns["beenadeVolley"].val = -1;
                    }
                }

                if (cooldowns["beehiveDrop"].TickCooldown())
                {
                    Vector2 spawnPos = npc.Center;
                    spawnPos.Y -= npc.height / 2;
                    NewHostileProjectile(npc.GetSource_FromAI(), spawnPos, new Vector2(0, 2), ProjectileID.BeeHive, 50, 1f);
                }
            }
            
            if (cooldowns.ContainsKey("regurgitate"))
            {
                if (cooldowns["regurgitate"].val == -1 && cooldowns["regurgitateActivate"].TickCooldown())
                {
                    cooldowns["regurgitate"].val = 0;
                    SoundEngine.PlaySound(SoundID.NPCHit57, npc.position);
                }

                if (cooldowns["regurgitate"].val >= 0)
                {
                    if (npc.life < npc.lifeMax / 4 && cooldowns["regurgitate"].val < cooldowns["regurgitate"].maxVal / 4)
                        cooldowns["regurgitate"].val = cooldowns["regurgitate"].maxVal / 4;
                    else if (npc.life < npc.lifeMax / 2 && cooldowns["regurgitate"].val < cooldowns["regurgitate"].maxVal / 2)
                        cooldowns["regurgitate"].val = cooldowns["regurgitate"].maxVal / 2;


                    if (cooldowns["regurgitate"].TickCooldown())
                    {
                        Vector2 spawnPos = npc.Center;
                        spawnPos.Y -= npc.height / 2;
                        int[] npcTypes = new int[]
                        {
                            NPCID.FireImp,
                            NPCID.Demon,
                            NPCID.LavaSlime,
                            NPCID.Hellbat
                        };
                        if (npc.life < npc.lifeMax / 3)
                        {
                            npcTypes = new int[]
                            {
                                NPCID.RedDevil,
                                NPCID.FireImp,
                                NPCID.Lavabat,
                                NPCID.IchorSticker,
                                NPCID.Corruptor
                            };
                        }
                        for (float i = 0; i < 5; i++)
                        {
                            int index = NewChild(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, npcTypes[Main.rand.Next(npcTypes.Length)], 0, 0, 0, 0, 0, npc.target);
                            Vector2 vel = npc.velocity * Main.rand.Next(5, 11);
                            vel.Y = Main.rand.NextFloat(-3f, 3f);
                            if (Main.rand.NextBool()) npc.velocity.Y *= -1;
                            Main.npc[index].velocity = vel;
                            NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, vel, ProjectileID.BallofFire, 45, 1f);
                        }
                        SoundEngine.PlaySound(SoundID.NPCDeath13, npc.position);
                        cooldowns["regurgitate"].val = -1;
                    }
                }
            }

            if (cooldowns.ContainsKey("boulderThrowWOF"))
            {
                if (cooldowns["boulderThrowWOF"].TickCooldown())
                {
                    NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, 
                        npc.DirectionTo(Main.player[npc.target].position) * 20,
                        ProjectileID.Boulder, 200, 5f);
                }
            }
/*
            if (npc.type == NPCID.TheDestroyer)
            {
                // Make the destroyer's probes upgrade with each incrementation of health.
                if (npc.life <= npc.lifeMax / 3)
                {

                }

                if (npc.life <= (npc.lifeMax / 3) + (npc.lifeMax / 3))
                {

                }

                if (npc.life <= npc.lifeMax - (npc.lifeMax / 3))
                {

                }
            }*/
        }
        
        /// <summary>
        /// Returns true or false depending on if the NPC can split into children or not.
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static bool CanSplit(NPC npc)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
            int[] slimes = new int[] {
                NPCID.GreenSlime,
                NPCID.BlueSlime,
                NPCID.RedSlime,
                NPCID.PurpleSlime,
                NPCID.YellowSlime,
                NPCID.BlackSlime,
                NPCID.IceSlime,
                NPCID.SandSlime,
                NPCID.JungleSlime,
                NPCID.SpikedIceSlime,
                NPCID.SpikedJungleSlime,
                NPCID.LavaSlime,
                NPCID.DungeonSlime,
                NPCID.Pinky,
                NPCID.GoldenSlime,
                NPCID.SlimeSpiked,
                NPCID.UmbrellaSlime,
                NPCID.BunnySlimed,
                NPCID.SlimeRibbonWhite,
                NPCID.SlimeRibbonYellow,
                NPCID.SlimeRibbonGreen,
                NPCID.SlimeRibbonRed,
                NPCID.ToxicSludge,
                NPCID.Crimslime,
                NPCID.BigCrimslime,
                NPCID.LittleCrimslime,
                NPCID.Gastropod,
                NPCID.IlluminantSlime,
                NPCID.RainbowSlime,
                NPCID.QueenSlimeMinionBlue,
                NPCID.QueenSlimeMinionPurple,
                NPCID.QueenSlimeMinionPink,
                NPCID.HoppinJack
            };
            if (!npc.friendly && !modNPC.child && !npc.boss && slimes.Contains(npc.type)) return true;
            return false;
        }

        public void NewCooldown(NPC npc, int npcType, string key, int counterMax, bool moonLordCopiesAbility = true, int ? defaultVal = null)
        {
            if (npc.type == npcType || (npc.type == NPCID.MoonLordCore && moonLordCopiesAbility))
            {
                cooldowns[key] = new Cooldown(counterMax, defaultVal);
            }
        }
        public void NewCooldown(NPC[] npcs, int npcType, string key, int counterMax, bool moonLordCopiesAbility = true, int? defaultVal = null)
        {
            foreach (NPC npc in npcs)
            {
                NewCooldown(npc, npcType, key, counterMax, moonLordCopiesAbility, defaultVal);
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

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            int[] beeTypes = new int[]
            {
                NPCID.Bee,
                NPCID.BeeSmall,
                NPCID.QueenBee
            };

            if (target.HasBuff(BuffID.Honey) && beeTypes.Contains(npc.type))
            {
                modifiers.FinalDamage *= 3;
            }

            if (npc.type == NPCID.Deerclops)
            {
                target.AddBuff(BuffID.Confused, 120);
            }
        }

        /// <summary>
        /// Spawns a new projectile that is marked as hostile and not friendly.<br></br>
        /// Does the client checks necessary.
        /// </summary>
        /// <param name="spawnSource"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="Type"></param>
        /// <param name="Damage"></param>
        /// <param name="KnockBack"></param>
        /// <param name="Owner"></param>
        /// <param name="ai0"></param>
        /// <param name="ai1"></param>
        /// <returns></returns>
        public static int? NewHostileProjectile(IEntitySource spawnSource, Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack, int Owner = 255, float ai0 = 0, float ai1 = 0)
        {
            int? output = null;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int whoAmI = Projectile.NewProjectile(spawnSource, position, velocity, Type, Damage, KnockBack, Owner, ai0, ai1);
                Main.projectile[whoAmI].friendly = false;
                Main.projectile[whoAmI].hostile = true;
                output = whoAmI;
            }
            return output;
        }

        public static int NewChild(IEntitySource source, int X, int Y, int Type, int Start = 0, float ai0 = 0, float ai1 = 0, float ai2 = 0, float ai3 = 0, int Target = 255)
        {
            int index = NPC.NewNPC(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
            NPC newNPC = Main.npc[index];
            newNPC.active = true;
            StupidNPC modNPC = newNPC.GetGlobalNPC<StupidNPC>();
            modNPC.child = true;
            newNPC.SpawnedFromStatue = true;
            newNPC.lifeMax *= 2;
            newNPC.life = newNPC.lifeMax;
            newNPC.scale *= 0.8f;
            newNPC.value = 0;
            newNPC.GivenName = "Baby " + newNPC.GivenOrTypeName;
            newNPC.netUpdate = true;
            return index;
        }

        private static int GetBossValue()
        {
            int val = 0;
            if (NPC.downedSlimeKing) val = 1;
            if (NPC.downedBoss1) val = 2;
            if (NPC.downedBoss2) val = 3;
            if (NPC.downedDeerclops) val = 4;
            if (NPC.downedQueenBee) val = 5;
            if (NPC.downedBoss3) val = 6;
            if (Main.hardMode) val = 7;
            if (NPC.downedMechBossAny) val = 8;
            if (NPC.downedFishron) val = 9;
            if (NPC.downedPlantBoss) val = 10;
            if (NPC.downedEmpressOfLight) val = 11;
            if (NPC.downedGolemBoss) val = 12;
            if (NPC.downedAncientCultist) val = 13;
            if (NPC.downedMoonlord) val = 14;
            return val;
        }

        /*public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            foreach (Item i in items)
            {
                i.value = (int)(i.value * (1 + (GetBossValue() * 0.1f)));
            }
        }*/
        
        private void SpecialLoot(NPC npc)
        {
            int? dropItem = null;

            switch(npc.type)
            {
                case NPCID.KingSlime: dropItem = ModContent.ItemType<Content.Items.Accessories.NinjaSlice>(); break;
                case NPCID.EyeofCthulhu: dropItem = ModContent.ItemType<Content.Items.Accessories.BoulderCharm>(); break;
                case NPCID.BrainofCthulhu: dropItem = ModContent.ItemType<Content.Items.Accessories.CrimsonOrb>(); break;
                case NPCID.Deerclops: dropItem = ModContent.ItemType<Content.Items.Accessories.ThuleciteCrown>(); break;
                case NPCID.QueenBee: dropItem = ModContent.ItemType<Content.Items.Accessories.BeeShield>(); break;
                case NPCID.SkeletronHead: dropItem = ModContent.ItemType<Content.Items.Accessories.CursedBrick>(); break;
            }

            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
            if (modNPC.additionalLoot != null)
            {
                foreach (Item i in modNPC.additionalLoot) {
                    if (i.type != ItemID.None)
                    {
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), i);
                    }
                }
            }

            if (dropItem != null)
            {
                npc.DropItemInstanced(npc.position, npc.Size, dropItem.Value, 1, true);
            }
        }

        public int[] SummonEnemySwarm(IEntitySource source, int type, int size, Vector2 pos)
        {
            Vector2 newPos;
            Vector2 tilePos;
            int shiftDirection;
            int[] npcs = new int[size];
            bool hasTile;
            for (int i = 0; i < size; i++)
            {
                newPos.X = pos.X + Main.rand.NextFloat(-2000, 2000);
                newPos.Y = pos.Y + Main.rand.NextFloat(-2000, 2000);
                tilePos = newPos.ToTileCoordinates16().ToVector2();
                hasTile = Framing.GetTileSafely(tilePos).HasTile;
                if (hasTile)
                {
                    if (newPos.Y > pos.Y) shiftDirection = -1;
                    else shiftDirection = 1;
                    for (int j = 0; j < 40; j++)
                    {
                        tilePos.Y += shiftDirection;
                        hasTile = Framing.GetTileSafely(tilePos).HasTile;
                        if (!hasTile)
                        {
                            break;
                        }
                    }
                }
                hasTile = Framing.GetTileSafely(tilePos).HasTile;
                newPos = tilePos.ToWorldCoordinates();
                if (!hasTile)
                {
                    npcs[i] = NPC.NewNPC(source, (int)newPos.X, (int)newPos.Y, type);
                }
            }
            return npcs;
        }
        
        public static int? FindClosestNPC(Vector2 position, bool includeFriendly, bool includeNotFriendly, bool includeCreatures, bool includeInvulnerable, NPC type = null, int? maximumDistance = null)
        {
            NPC closestNPC = null;
            float? oldDistance = null;
            float distance;

            foreach (NPC i in Main.npc)
            {
                if (i.active && i.type != NPCID.TargetDummy && (!includeInvulnerable || !i.dontTakeDamage) && ((i.friendly && includeFriendly) || (!Main.npcCatchable[i.type] && !i.friendly && includeNotFriendly) || (Main.npcCatchable[i.type] && includeCreatures)))
                {
                    if (type == null || i.type == type.value)
                    {
                        distance = position.Distance(i.position);
                        if (maximumDistance == null || distance <= maximumDistance)
                            if (oldDistance == null)
                            {
                                oldDistance = distance;
                                closestNPC = i;
                            }
                            else if (distance < oldDistance)
                            {
                                oldDistance = distance;
                                closestNPC = i;
                            }
                    }
                }
            }

            if (closestNPC == null) return null;
            return closestNPC.whoAmI;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
            if (modNPC.child) bitWriter.WriteBit(true);
            else bitWriter.WriteBit(false);
            binaryWriter.Write7BitEncodedInt(npc.life);
            binaryWriter.Write7BitEncodedInt(npc.lifeMax);
            binaryWriter.Write7BitEncodedInt((int)(npc.scale*100));
        }

        // Retrieve data in the same order that it is sent!
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (bitReader.ReadBit())
            {
                npc.GivenName = "Baby " + npc.TypeName;
            }
            npc.life = binaryReader.Read7BitEncodedInt();
            npc.lifeMax = binaryReader.Read7BitEncodedInt();
            npc.scale = (float)binaryReader.Read7BitEncodedInt()/100;
        }

        private static BossMusic GetBossMusic(int type)
        {
            switch (type)
            {
                case NPCID.KingSlime: return BossMusic.UltimateBattle;
                case NPCID.EyeofCthulhu: return BossMusic.UltimateBattle;
                case NPCID.SkeletronHead: return BossMusic.UltimateBattle;
                case NPCID.EaterofWorldsHead: return BossMusic.UltimateBattle;
                case NPCID.SkeletronPrime: return BossMusic.UltimateBattle;

                case NPCID.WallofFlesh: return BossMusic.ChaosKing;
                case NPCID.WallofFleshEye: return BossMusic.ChaosKing;

                case NPCID.QueenBee: return BossMusic.DSTBeeQueen;
                    
                default: return BossMusic.None;
            }
        }

        private static void SendBossMusicCredits(BossMusic music) {
            if (music == BossMusic.None) return;

            string author = "";
            string source = "";
            string songName = "";

            switch (music)
            {
                case BossMusic.UltimateBattle: songName = "Ultimate Battle"; author = "Laura Shigihara"; source = "Plants Vs. Zombies"; break;
                case BossMusic.ChaosKing: songName = "Chaos King"; author = "Toby Fox"; source = "Deltarune"; break;
                case BossMusic.DSTBeeQueen: songName = "Bee Queen's Theme"; author = "Klei Entertainment"; source = "Don't Starve Together"; break;
            }

            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Now Playing: " + songName + " by " + author + " from " + source + "."), new Color(25, 217, 234));
        }
    }

    public enum BossMusic {
        None = -1,
        UltimateBattle = 0,
        ChaosKing = 1,
        DSTBeeQueen = 2
    }
}