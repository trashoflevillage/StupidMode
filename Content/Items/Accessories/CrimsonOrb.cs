using StupidMode.Common.Global;
using StupidMode.Content.NPCs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Content.Items.Accessories
{
    public class CrimsonOrb : ModItem
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
            player.GetModPlayer<StupidPlayer>().crimsonOrb = true;
            StupidPlayer modPlayer = player.GetModPlayer<StupidPlayer>();

            if (!modPlayer.hasCrimsonOrbMinion && !player.dead)
            {
                modPlayer.hasCrimsonOrbMinion = true;
                SoundEngine.PlaySound(SoundID.Item2, player.position);
                NPC.NewNPC(player.GetSource_Accessory(Item, "crimsonOrb"), (int)player.position.X, (int)player.position.Y - 50, ModContent.NPCType<CrimsonOrbMinion>(), 0, player.whoAmI, 300);
            }
        }
    }
}