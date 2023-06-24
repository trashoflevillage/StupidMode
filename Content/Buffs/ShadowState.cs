using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Content.Buffs
{
    internal class ShadowState : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.statLife >= player.statLifeMax2)
            {
                player.DelBuff(buffIndex);
            } else
            {
                player.AddBuff(ModContent.BuffType<ShadowState>(), 1);
            }
        }
    }
}
