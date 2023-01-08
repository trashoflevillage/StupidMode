using Microsoft.Xna.Framework;
using StupidMode.Common.System.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Common.Global
{
    internal class StupidNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool child;
        public bool dropMeteorite;

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

                NewCooldown(npc, NPCID.Spazmatism, "boulderThrowActivate", 340, false);
                NewCooldown(npc, NPCID.Spazmatism, "boulderThrowSpazm", 180, false, -1);

                NewCooldown(npc, NPCID.Retinazer, "laserRingActivate", 340);
                NewCooldown(npc, NPCID.Retinazer, "laserRing", 180, true, -1);
            }
        }

        public override void OnKill(NPC npc)
        {
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
            if (CanSplit(npc))
                for (int i = 0; i < 2; i++)
                {
                    NewChild(npc.GetSource_Death(), (int)npc.position.X + Main.rand.Next(-1, 1), (int)npc.position.Y, npc.type);
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

            if (npc.position.Y / 16 > Main.UnderworldLayer)
            {
                WorldGen.PlaceLiquid(npc.position.ToTileCoordinates().X, npc.position.ToTileCoordinates().Y, LiquidID.Lava, 5);
            }
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
            StupidNPC modNPC = npc.GetGlobalNPC<StupidNPC>();
            if (npc.type == NPCID.ServantofCthulhu) {
                Projectile.NewProjectile(npc.GetSource_Death(), npc.position, npc.velocity * -1, ProjectileID.Boulder, 100, 5);
            }

            if (modNPC.child) return false;
            return base.PreKill(npc);
        }

        public void OnHitByAnything(NPC npc, int damage, float knockback, bool crit)
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

            if (cooldowns.ContainsKey("boulderThrowSpazm"))
            {
                if (npc.life <= npc.lifeMax * 0.4)
                {
                    if (cooldowns["boulderThrowSpazm"].val == -1 && cooldowns["boulderThrowActivate"].TickCooldown())
                    {
                        cooldowns["boulderThrowSpazm"].val = 0;
                    }

                    if (cooldowns["boulderThrowSpazm"].val >= 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = npc.Center;
                            pos.Y += Main.rand.Next((npc.height / 2) * -1, (npc.height / 2));
                            pos.X += Main.rand.Next((npc.width / 2) * -1, (npc.width / 2));
                            Dust.NewDust(pos, 4, 4, DustID.Electric);
                        }

                        if (cooldowns["boulderThrowSpazm"].TickCooldown())
                        {
                            int? index;
                            for (int i = 0; i < Main.rand.Next(4, 8); i++)
                            {
                                index = NewHostileProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 0)), ProjectileID.Boulder, npc.damage * 8, 3f);
                                Main.projectile[index.Value].tileCollide = false;
                            }
                            SoundEngine.PlaySound(SoundID.Item80, npc.Center);
                            cooldowns["boulderThrowSpazm"].val = -1;
                        }
                    }
                }
            }

            if (cooldowns.ContainsKey("laserRing"))
            {
                if (npc.life <= npc.lifeMax * 0.4)
                {
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
            }

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
                NPCID.ServantofCthulhu,
                NPCID.MoonLordHand,
                NPCID.MoonLordHead,
                NPCID.MoonLordCore,
                NPCID.MoonLordFreeEye,
                NPCID.MoonLordLeechBlob,
                NPCID.TheHungry,
                NPCID.TheHungryII,
                NPCID.LeechBody,
                NPCID.LeechHead,
                NPCID.LeechTail,
                NPCID.Spazmatism,
                NPCID.Retinazer
            };
            if (!npc.friendly && !modNPC.child && !npc.boss && !exceptions.Contains(npc.type)) return true;
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
                if (npc.type == npcType || (npc.type == NPCID.MoonLordCore && moonLordCopiesAbility))
                {
                    cooldowns[key] = new Cooldown(counterMax, defaultVal);
                }
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

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            int[] beeTypes = new int[]
            {
                NPCID.Bee,
                NPCID.BeeSmall,
                NPCID.QueenBee
            };

            if (target.HasBuff(BuffID.Honey) && beeTypes.Contains(npc.type))
            {
                damage *= 3;
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

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            foreach (Item i in shop.item)
            {
                i.value = (int)(i.value * (1 + (GetBossValue() * 0.1f)));
            }
        }
    }
}