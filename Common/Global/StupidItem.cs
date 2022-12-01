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

namespace StupidMode.Common.Global
{
    internal class StupidItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        int reforgeCount = 0;

        public override bool PreReforge(Item item)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            if (modItem.reforgeCount >= 10)
            {
                return false;
            }
            return base.PreReforge(item);
        }

        public override void PostReforge(Item item)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            modItem.reforgeCount++;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            StupidItem modItem = item.GetGlobalItem<StupidItem>();
            if (item.IsCandidateForReforge) tooltips.Add(new TooltipLine(StupidMode.Instance, "reforgeCounter", "[c/FFF014:" + modItem.reforgeCount + "/10 reforges]"));
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
        }
    }
}
