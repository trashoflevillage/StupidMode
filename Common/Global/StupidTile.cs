using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
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
        }
    }
}
