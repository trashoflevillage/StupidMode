using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Common.SceneEffects
{
    public class UltimateBattle : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/UltimateBattle");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return NPC.AnyNPCs(NPCID.EyeofCthulhu) || 
                NPC.AnyNPCs(NPCID.KingSlime) || 
                NPC.AnyNPCs(NPCID.EaterofWorldsHead) || 
                NPC.AnyNPCs(NPCID.SkeletronHead) || 
                NPC.AnyNPCs(NPCID.SkeletronPrime);
        }
    }
}