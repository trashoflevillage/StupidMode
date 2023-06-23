using StupidMode.Content.Items.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace StupidMode.Common.System.Genpasses
{
    internal class MimicifyGenpass : GenPass
    {
        public MimicifyGenpass(string name, float weight) : base(name, weight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int[] itemsToPlaceInChests = { ModContent.ItemType<Mimicifier>() };
            int itemsToPlaceInChestsChoice = 0;
            for (int chestIndex = 0; chestIndex < Main.chest.Length; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && (Main.tile[chest.x, chest.y].TileType == TileID.Containers || Main.tile[chest.x, chest.y].TileType == TileID.Containers2))
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests[itemsToPlaceInChestsChoice]);
                            itemsToPlaceInChestsChoice = (itemsToPlaceInChestsChoice + 1) % itemsToPlaceInChests.Length;
                            break;
                        }
                    }
                }
            }
        }
    }
}
