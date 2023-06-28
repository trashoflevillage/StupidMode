using StupidMode.Common.Global;
using StupidMode.Content.NPCs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Content.Items.Accessories
{
    public class SuperAccessoryPrehardmode : ModItem
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
            // Ninja Slice
            player.GetModPlayer<StupidPlayer>().ninjaSlice = true;

            // Boulder Charm
            player.GetModPlayer<StupidPlayer>().boulderCharm = true;
            player.AddBuff(BuffID.Spelunker, 1);

            // Crimson Orb
            player.GetModPlayer<StupidPlayer>().crimsonOrb = true;
            StupidPlayer modPlayer = player.GetModPlayer<StupidPlayer>();

            if (!modPlayer.hasCrimsonOrbMinion && !player.dead && Main.myPlayer == player.whoAmI)
            {
                modPlayer.hasCrimsonOrbMinion = true;
                SoundEngine.PlaySound(SoundID.Item2, player.position);
                NPC.NewNPC(player.GetSource_Accessory(Item, "crimsonOrb"), (int)player.position.X, (int)player.position.Y - 50, ModContent.NPCType<CrimsonOrbMinion>(), 0, player.whoAmI, 300);
            }

            // Shadow Heart
            player.GetModPlayer<StupidPlayer>().shadowHeart = true;

            // Thulecite Crown
            player.GetModPlayer<StupidPlayer>().thuleciteCrown = true;
            player.statDefense += 4;

            // Bee Shield
            player.GetModPlayer<BeeDashPlayer>().BeeShieldEquipped = true;

            // Cursed Brick
            player.GetModPlayer<StupidPlayer>().cursedBrick = true;

            // Fleshy Mass
            player.GetModPlayer<StupidPlayer>().fleshyMass = true;
            player.buffImmune[ModContent.BuffType<Buffs.Misguided>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<NinjaSlice>())
                .AddIngredient(ModContent.ItemType<BoulderCharm>())
                .AddIngredient(ModContent.ItemType<CrimsonOrb>())
                .AddIngredient(ModContent.ItemType<ShadowHeart>())
                .AddIngredient(ModContent.ItemType<ThuleciteCrown>())
                .AddIngredient(ModContent.ItemType<BeeShield>())
                .AddIngredient(ModContent.ItemType<CursedBrick>())
                .AddIngredient(ModContent.ItemType<FleshyMass>());
        }
    }
}