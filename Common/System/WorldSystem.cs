using StupidMode.Common.System.Genpasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace StupidMode.Common.System
{
    internal class WorldSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            tasks.Add(new MimicifyGenpass("Buffing chest loot", 1f));
        }
    }
}
