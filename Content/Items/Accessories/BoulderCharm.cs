﻿using StupidMode.Common.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Content.Items.Accessories
{
	public class BoulderCharm : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 32;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ModContent.RarityType<Rarities.StupidRarity>();
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<StupidPlayer>().boulderCharm = true;
			player.AddBuff(BuffID.Spelunker, 1);
        }
    }
}