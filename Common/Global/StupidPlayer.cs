using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Events;
using Terraria.GameInput;
using StupidMode.Common.Systems;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace StupidMode.Common.Global
{
    internal class StupidPlayer : ModPlayer
    {
        public bool boulderCharm = false;
        public bool ninjaSlice = false;
        public int taunting = 0;
        Vector2? velBeforeTaunting;

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHitByAnything(ref hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            OnHitByAnything(ref hurtInfo);
        }

        public void OnHitByAnything(ref Player.HurtInfo hurtInfo)
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();

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
            modPlayer.ninjaSlice = false;
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

            if (taunting > 0)
            {
                taunting--;
            }
        }

        public override void PreUpdateMovement()
        {
            if (taunting > 0) Player.velocity = new Vector2(0, 0);
            else if (velBeforeTaunting != null)
            {
                Player.velocity = velBeforeTaunting.Value;
                velBeforeTaunting = null;
            }
        }

        public override bool CanUseItem(Item item)
        {
            if (taunting > 0) return false;
            return base.CanUseItem(item);
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (taunting > 0)
            {
                TauntParry(info);
                return true;
            }
            return base.FreeDodge(info);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.TauntKeybind.JustPressed)
            {
                if (ninjaSlice == true && taunting == 0)
                {
                    Taunt();
                }
            }
        }

        public void Taunt()
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            SoundEngine.PlaySound(new SoundStyle("StupidMode/Assets/Sounds/taunt"), Player.position);
            modPlayer.taunting = 15;
            velBeforeTaunting = Player.velocity;
        }

        public void TauntParry(Player.HurtInfo info)
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            SoundEngine.PlaySound(new SoundStyle("StupidMode/Assets/Sounds/tauntParry"), Player.position);
            modPlayer.taunting = 0;
            Player.SetImmuneTimeForAllTypes(60);
            Player.immuneNoBlink = true;
        }
    }
}
