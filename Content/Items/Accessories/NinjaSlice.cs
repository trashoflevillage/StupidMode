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
	public class NinjaSlice : ModItem
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
			player.GetModPlayer<StupidPlayer>().ninjaSlice = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			int i = -1;
			foreach (TooltipLine line in tooltips)
			{
				i++;
				if (line.Name == "tooltip")
                {
					break;
                }
            }

			List<string> bindings = KeybindSystem.TauntKeybind.GetAssignedKeys();
			TooltipLine tooltipLine;

			if (bindings.Count > 0)
            {
				tooltipLine = new(StupidMode.Instance, "Tooltip0", Language.GetOrRegister("Mods.StupidMode.Items.NinjaSlice.KeybindTooltip").Value + bindings[0]);
			} else
            {
				tooltipLine = new(StupidMode.Instance, "Tooltip0", Language.GetOrRegister("Mods.StupidMode.Items.NinjaSlice.KeybindTooltip").Value + Language.GetOrRegister("Mods.StupidMode.UnboundKeybind").Value);
			}

			if (i != -1)
            {
				tooltips[i] = tooltipLine;
			} else
            {
				tooltips.Add(tooltipLine);
            }
        }
    }
}