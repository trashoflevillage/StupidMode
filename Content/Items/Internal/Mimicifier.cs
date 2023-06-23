using StupidMode.Common.Global;
using StupidMode.Content.Projectiles;
using StupidMode.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Content.Items.Internal
{
	public class Mimicifier : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 44;
			Item.rare = ModContent.RarityType<StupidRarity>();
			Item.shoot = ModContent.ProjectileType<Bloodbeam>();
			Item.useStyle = ItemUseStyleID.Shoot;
		}
    }
}