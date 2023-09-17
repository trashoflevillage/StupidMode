using StupidMode.Common.Global;
using Terraria;
using Terraria.ModLoader;

namespace StupidMode.Common.SceneEffects.Music
{
    public class BrokenVessel : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/BrokenVessel");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            StupidNPC modNPC;

            foreach (NPC n in Main.npc)
            {
                if (n.active && n.TryGetGlobalNPC(out modNPC))
                {
                    modNPC = n.GetGlobalNPC<StupidNPC>();
                    if (modNPC.bossMusic == BossMusic.BrokenVessel) return true;
                }
            }

            return false;
        }
    }
}