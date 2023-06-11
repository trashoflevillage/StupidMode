using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace StupidMode
{
    public class StupidMode : Mod
    {
        public StupidMode() { Instance = this; }
        public static StupidMode Instance { get; private set; }

        public override void Load()
        {
            TextureAssets.Ninja = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
        }
    }
}