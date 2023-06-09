using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;

namespace StupidMode.Content.Rarities
{
	public class StupidRarity : ModRarity
	{
		public override Color RarityColor => new Color(Main.DiscoR, Main.DiscoB, Main.DiscoG);
	}
}