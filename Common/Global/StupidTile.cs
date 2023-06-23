using Microsoft.Xna.Framework;
using StupidMode.Content.Items.Internal;
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

namespace StupidMode.Common.Global
{
    internal class StupidTile : GlobalTile
    {
        public static int[] ores = new int[] {
            TileID.Copper,
            TileID.Tin,
            TileID.Iron,
            TileID.Lead,
            TileID.Gold,
            TileID.Platinum,
            TileID.Demonite,
            TileID.Crimtane,
            TileID.Tungsten,
            TileID.Silver
        };
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (type == TileID.Trees && !fail && !effectOnly)
            {
                if (Main.rand.NextBool(150))
                {
                    Dust.NewDust(new Vector2(i * 16, j * 16), 1, 1, DustID.Cloud);
                    int[] typePool = new int[]
                    {
                        NPCID.Zombie,
                        NPCID.DemonEye,
                        NPCID.Worm,
                        NPCID.LavaSlime
                    };
                    int npcType;
                    for (int a = 0; a < Main.rand.Next(4, 7); a++)
                    {
                        npcType = typePool[Main.rand.Next(0, typePool.Length)];
                        int npcIndex = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), i * 16, j * 16, npcType);
                        NPC newNPC = Main.npc[npcIndex];
                        newNPC.velocity = new Vector2(0, -5);
                        newNPC.value = 0;
                    }
                }
            }

            if (ores.Contains(type) && Main.rand.NextBool(1, 350))
            {
                NPC.NewNPC(Entity.GetSource_NaturalSpawn(), i * 16, j * 16, NPCID.GiantShelly);
            }

            if (type == TileID.ShadowOrbs && !fail && !effectOnly && !noItem)
            {
                int eaterType;
                if (WorldGen.crimson) eaterType = NPCID.Crimera;
                else eaterType = NPCID.EaterofSouls;
                Main.npc[NPC.NewNPC(Entity.GetSource_NaturalSpawn(), i * 16, j * 16, eaterType)].scale *= 2;
            }
            
            if (type == TileID.Tombstones && !fail && !effectOnly && !noItem)
            {
                Main.npc[NPC.NewNPC(Entity.GetSource_NaturalSpawn(), i * 16, j * 16, NPCID.Ghost)].value = 0;
            }

            if (type == TileID.Meteorite && !fail && !effectOnly && !noItem)
            {
                noItem = true;
                Main.npc[NPC.NewNPC(NPC.GetSource_NaturalSpawn(), i * 16, j * 16, NPCID.MeteorHead)].GetGlobalNPC<StupidNPC>().dropMeteorite = true;
            }
        }

        public override void RightClick(int i, int j, int type)
        {
            if (type == TileID.Containers || type == TileID.Containers2)
            {
                for (int chestIndex = 0; chestIndex < Main.chest.Length; chestIndex++)
                {
                    Chest chest = Main.chest[chestIndex];

                    int[] xCheck = new int[] { i - 1, i, i + 1 };
                    int[] yCheck = new int[] { j - 1, j, j + 1 };

                    if (chest != null && xCheck.Contains(chest.x) && yCheck.Contains(chest.y) && (Main.tile[chest.x, chest.y].TileType == TileID.Containers || Main.tile[chest.x, chest.y].TileType == TileID.Containers2) && !Chest.IsLocked(i, j))
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ModContent.ItemType<Mimicifier>())
                            {
                                int mimic = NPC.NewNPC(new EntitySource_TileInteraction(Main.player[0], i, j), chest.x * 16, chest.y * 16, NPCID.Mimic);
                                StupidNPC modNPC = Main.npc[mimic].GetGlobalNPC<StupidNPC>();
                                modNPC.mimicTrap = true;
                                modNPC.additionalLoot = new Item[chest.item.Length];
                                for (int inventoryIndexB = 0; inventoryIndexB < 40; inventoryIndexB++)
                                {
                                    modNPC.additionalLoot[inventoryIndexB] = new Item(ItemID.None);
                                    if (chest.item[inventoryIndexB].type != ItemID.None && chest.item[inventoryIndexB].type != ModContent.ItemType<Mimicifier>())
                                    {
                                        modNPC.additionalLoot[inventoryIndexB] = new Item(chest.item[inventoryIndexB].type, chest.item[inventoryIndexB].stack, chest.item[inventoryIndexB].prefix);
                                    }
                                    chest.item[inventoryIndexB].SetDefaults(ItemID.None);
                                    WorldGen.KillTile(chest.x, chest.y, false, false, true);
                                }
                                return;
                            }
                        }
                        return;
                    }
                }
            }
        }
    }
}