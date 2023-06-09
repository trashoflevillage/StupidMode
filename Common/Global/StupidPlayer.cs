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
        public bool boulderCharm = false;

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHitByAnything(hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            OnHitByAnything(hurtInfo);
        }

        public void OnHitByAnything(Player.HurtInfo hurtInfo)
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

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            if (modPlayer.boulderCharm)
            {
                if (proj.type == ProjectileID.Boulder || proj.type == ProjectileID.BouncyBoulder || proj.type == ProjectileID.BoulderStaffOfEarth || proj.type == ProjectileID.MiniBoulder ||
                    proj.type == ProjectileID.MoonBoulder || proj.type == ProjectileID.LifeCrystalBoulder)
                {
                    modifiers.SetMaxDamage(Player.statLifeMax2 / 2);
                }
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

        public override void ResetEffects()
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            modPlayer.boulderCharm = false;
        }

        public void PassiveEffects()
        {
            if (Main.bloodMoon)
                Player.AddBuff(BuffID.Bleeding, 60, false);

            if (!Player.behindBackWall)
            {
                if (Player.ZoneDesert && Main.dayTime)
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
