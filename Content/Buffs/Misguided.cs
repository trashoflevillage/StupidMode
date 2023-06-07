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
    internal class Misguided : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Misguided");
            // Description.SetDefault("There is nobody around to guide you. You feel directionless.");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            float modifier = Main.rand.NextFloat(0.5f, 1f);
            player.moveSpeed *= modifier;
            player.accRunSpeed *= modifier;
            player.runAcceleration *= modifier;
            player.maxRunSpeed *= modifier;
        }
    }
}
