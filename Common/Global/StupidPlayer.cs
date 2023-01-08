using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Events;

namespace StupidMode.Common.Global
{
    internal class StupidPlayer : ModPlayer
    {
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            OnHitByAnything(damage, crit);
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            OnHitByAnything(damage, crit);
        }

        public void OnHitByAnything(int damage, bool crit)
        {
            if (Player.ZoneCorrupt)
            {
                Player.AddBuff(BuffID.CursedInferno, 120);
            }

            if (Player.ZoneCrimson)
            {
                Player.AddBuff(BuffID.Ichor, 240);
            }
        }

        public override void PreUpdateBuffs()
        {
            if (NPC.GetFirstNPCNameOrNull(NPCID.Guide) == null)
            {
                Player.AddBuff(ModContent.BuffType<Content.Buffs.Misguided>(), 5);
            }
        }

        public override void PreUpdate()
        {
            PassiveEffects();
        }

        public void PassiveEffects()
        {
            if (Main.bloodMoon)
                Player.AddBuff(BuffID.Bleeding, 60, false);

            if (!Player.behindBackWall)
            {
                if (Player.ZoneDesert)
                    Player.AddBuff(BuffID.OnFire, 60, false);
                if (Player.position.Y / 16 > Main.UnderworldLayer && !Player.HasBuff(BuffID.ObsidianSkin))
                    Player.AddBuff(BuffID.OnFire3, 60, false);
            }

            if (Player.ZoneJungle)
            {
                if (Player.breath < Player.breathMax)
                    Player.AddBuff(BuffID.Poisoned, 60, false);
            }
        }
    }
}
