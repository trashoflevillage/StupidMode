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
        /*
        int reforgeCount = 0;

        public override bool CanReforge(Item item)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            if (modItem.reforgeCount >= 10)
            {
                return false;
            }
            return base.CanReforge(item);
        }

        public override void PostReforge(Item item)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            modItem.reforgeCount++;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            if (tag.ContainsKey("reforgeCount"))
                modItem.reforgeCount = tag.GetInt("reforgeCount");
            else modItem.reforgeCount = 0;
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            tag.Set("reforgeCount", modItem.reforgeCount);
        }*/

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            if (item.rare == ModContent.RarityType<StupidRarity>())
            {
                Color color = new Color(Main.DiscoR, Main.DiscoB, Main.DiscoG);
                string hex = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
                tooltips.Add(new TooltipLine(StupidMode.Instance, "Expert", "[c/" + hex + ":Stupid]"));
            }
        }
    }
}
