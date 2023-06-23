using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using StupidMode.Common.Global;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace StupidMode.Common.PlayerDrawLayers
{
    internal class TauntDrawLayer : PlayerDrawLayer
	{
		private Asset<Texture2D> tauntTexture;
		public override Position GetDefaultPosition() => new BeforeParent(Terraria.DataStructures.PlayerDrawLayers.WebbedDebuffBack);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			return drawInfo.drawPlayer.GetModPlayer<StupidPlayer>().taunting > 0;
		}

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (tauntTexture == null)
			{
				tauntTexture = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Taunt");
			}

			var position = drawInfo.Center + new Vector2(0f, 0f) - Main.screenPosition;
			position = new Vector2((int)position.X, (int)position.Y); // You'll sometimes want to do this, to avoid quivering.

			drawInfo.DrawDataCache.Add(new DrawData(
				tauntTexture.Value, // The texture to render.
				position, // Position to render at.
				null, // Source rectangle.
				Color.White, // Color.
				0f, // Rotation.
				tauntTexture.Size() * 0.5f, // Origin. Uses the texture's center.
				1f, // Scale.
				SpriteEffects.None, // SpriteEffects.
				0 // 'Layer'. This is always 0 in Terraria.
			));
		}
    }
}
