using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace StupidMode.Common.Global
{
    internal class StupidBuff : GlobalBuff
    {
        public override void ModifyBuffTip(int type, ref string tip, ref int rare)
        {
            switch (type)
            {
                case BuffID.Honey: tip += "\nBee type enemies will deal triple damage!"; break;
                default: break;
            }
        }
    }
}
