using StupidMode.Common.Global;
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
			Item.rare = ItemRarityID.Gray;
		}
    }
}