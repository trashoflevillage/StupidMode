using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using StupidMode.Content.Rarities;

namespace StupidMode.Common.Global
{
    internal class StupidItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            if (item.rare == ModContent.RarityType<StupidRarity>())
            {
                Color color = new Color(65, 181, 109);
                string hex = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
                tooltips.Add(new TooltipLine(StupidMode.Instance, "Expert", "[c/" + hex + ":Stupid]"));
            }
        }
    }
}
