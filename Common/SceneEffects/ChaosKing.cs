using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Common.SceneEffects
{
    public class ChaosKing : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/ChaosKing");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return NPC.AnyNPCs(NPCID.WallofFlesh) || NPC.AnyNPCs(NPCID.WallofFleshEye);
        }
    }
}