using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace StupidMode.Common.Global
{
    internal class StupidSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) 
                Sandstorm.StartSandstorm();
        }
    }
}