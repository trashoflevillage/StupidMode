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
		public override void SaveWorldData(TagCompound tag)
		{
			foreach (KeyValuePair<short, bool> i in StupidNPC.bosses)
            {
				tag["summonedBoss_" + i.Key] = i.Value;
            }
		}

		public override void LoadWorldData(TagCompound tag)
		{
			short[] bosses = StupidNPC.bosses.Keys.ToArray();
			foreach (short i in bosses)
            {
				string asKey = "summonedBoss_" + i;
				if (!tag.ContainsKey(asKey)) StupidNPC.bosses[i] = false;
				else
                {
					StupidNPC.bosses[i] = tag.GetBool(asKey);
                }
            }
		}
	}
}