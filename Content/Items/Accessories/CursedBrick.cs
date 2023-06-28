using StupidMode.Common.Global;
using StupidMode.Common.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace StupidMode.Content.Items.Accessories
{
	public class CursedBrick : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ModContent.RarityType<Rarities.StupidRarity>();
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<StupidPlayer>().cursedBrick = true;
        }
    }
}