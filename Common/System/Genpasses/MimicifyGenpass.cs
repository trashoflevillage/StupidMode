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
            int[] itemsToPlaceInIceChests = { ModContent.ItemType<Mimicifier>() };
            int itemsToPlaceInIceChestsChoice = 0;
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInIceChests[itemsToPlaceInIceChestsChoice]);
                            itemsToPlaceInIceChestsChoice = (itemsToPlaceInIceChestsChoice + 1) % itemsToPlaceInIceChests.Length;
                            // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(Main.rand.Next(itemsToPlaceInIceChests));
                            break;
                        }
                    }
                }
            }
        }
    }
}
