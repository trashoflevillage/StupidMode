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