using Microsoft.Xna.Framework.Graphics;
using StupidMode.Common.Global;
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

            // Zombie head gores
            TextureAssets.Gore[3] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[154] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[191] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[241] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[243] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[246] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[262] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[309] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[451] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[454] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[457] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[488] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[491] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[722] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            TextureAssets.Gore[1214] = ModContent.Request<Texture2D>("StupidMode/Assets/Textures/Empty", ReLogic.Content.AssetRequestMode.ImmediateLoad);

            Terraria.On_NPC.NPCLoot_DropItems += On_NPC_NPCLoot_DropItems;
        }

        private void On_NPC_NPCLoot_DropItems(Terraria.On_NPC.orig_NPCLoot_DropItems orig, Terraria.NPC self, Terraria.Player closestPlayer)
        {
            StupidNPC modNPC = self.GetGlobalNPC<StupidNPC>();
            if (!modNPC.noLoot)
                orig(self, closestPlayer);
        }
    }
}