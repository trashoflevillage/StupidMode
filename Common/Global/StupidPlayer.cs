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
using StupidMode.Content.NPCs;
using Terraria.DataStructures;

namespace StupidMode.Common.Global
{
    internal class StupidPlayer : ModPlayer
    {
        public bool boulderCharm = false;
        public bool ninjaSlice = false;
        public bool crimsonOrb = false;
        public bool hasCrimsonOrbMinion = false;
        public bool shadowHeart = false;
        public bool thuleciteCrown = false;
        public int taunting = 0;
        public int oldDirection = 0;
        Vector2? velBeforeTaunting;

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHurtByAnything(hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            OnHurtByAnything(hurtInfo);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            OnHurtByAnything(info);
        }

        public void OnHurtByAnything(Player.HurtInfo hurtInfo)
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();

            if (Player.ZoneCorrupt && !modPlayer.shadowHeart && Main.myPlayer == Player.whoAmI)
            {
                Player.AddBuff(BuffID.CursedInferno, 120);
            }

            if (Player.ZoneCrimson && !modPlayer.crimsonOrb && Main.myPlayer == Player.whoAmI)
            {
                Player.AddBuff(BuffID.Ichor, 240);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            if (modPlayer.boulderCharm && Main.myPlayer == Player.whoAmI)
            {
                if (proj.type == ProjectileID.Boulder || proj.type == ProjectileID.BouncyBoulder || proj.type == ProjectileID.BoulderStaffOfEarth || proj.type == ProjectileID.MiniBoulder ||
                    proj.type == ProjectileID.MoonBoulder || proj.type == ProjectileID.LifeCrystalBoulder)
                {
                    modifiers.SetMaxDamage(Player.statLifeMax2 / 2);
                }
            }
            ModifyHurtByAnything(ref modifiers);
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            ModifyHurtByAnything(ref modifiers);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            ModifyHurtByAnything(ref modifiers);
        }

        public void ModifyHurtByAnything(ref Player.HurtModifiers modifiers)
        {
            if (Main.myPlayer != Player.whoAmI) return;
            if (Player.HasBuff(ModContent.BuffType<Content.Buffs.ShadowState>()))
            {
                modifiers.FinalDamage.Flat += Player.statLifeMax2;
            }
        }

        public override void PreUpdateBuffs()
        {
            if (Main.myPlayer == Player.whoAmI && NPC.GetFirstNPCNameOrNull(NPCID.Guide) == null)
            {
                Player.AddBuff(ModContent.BuffType<Content.Buffs.Misguided>(), 5);
            }
        }

        public override void PreUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
                PassiveEffects();
        }

        public override void ResetEffects()
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            modPlayer.boulderCharm = false;
            modPlayer.ninjaSlice = false;
            modPlayer.crimsonOrb = false;
            modPlayer.shadowHeart = false;
            modPlayer.thuleciteCrown = false;
        }

        public override void PostUpdateEquips()
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            if (Main.myPlayer == Player.whoAmI)
            {
                if (!modPlayer.crimsonOrb)
                {
                    if (modPlayer.hasCrimsonOrbMinion)
                    {
                        foreach (NPC i in Main.npc)
                        {
                            if (i.type == ModContent.NPCType<CrimsonOrbMinion>() && i.ai[0] == Player.whoAmI)
                            {
                                i.active = false;
                                break;
                            }
                        }
                        modPlayer.hasCrimsonOrbMinion = false;
                    }
                }
            }
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
            if (Main.myPlayer != Player.whoAmI) return;
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();

            if (taunting > 0)
            {
                Player.direction = oldDirection;
                Player.velocity = new Vector2(0, 0);
            }
            else if (velBeforeTaunting != null)
            {
                Player.velocity = velBeforeTaunting.Value;
                velBeforeTaunting = null;
            }
            oldDirection = Player.direction;

            modPlayer.SendClientChanges(modPlayer);
        }

        public override bool CanUseItem(Item item)
        {
            if (taunting > 0 && Main.myPlayer == Player.whoAmI) return false;
            return base.CanUseItem(item);
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();

            if (taunting > 0 && !Player.HasBuff(ModContent.BuffType<Content.Buffs.Overconfident>()) && Main.myPlayer == Player.whoAmI)
            {
                TauntParry(info);
                return true;
            }

            if (shadowHeart && !Player.HasBuff(ModContent.BuffType<Content.Buffs.ShadowState>()) && Player.statLife - info.Damage <= 0 && Main.myPlayer == Player.whoAmI)
            {
                Player.AddBuff(ModContent.BuffType<Content.Buffs.ShadowState>(), 1);
                Player.SetImmuneTimeForAllTypes(60);
                Player.immuneNoBlink = true;
                Player.statLife = 1;
                SoundEngine.PlaySound(SoundID.NPCHit54, Player.position);
                return true;
            }

            if (modPlayer.thuleciteCrown && info.Damage <= 10)
            {
                return true;
            }
            return base.FreeDodge(info);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!Player.dead && Main.myPlayer == Player.whoAmI)
            {
                if (KeybindSystem.TauntKeybind.JustPressed)
                {
                    if (ninjaSlice == true && taunting == 0)
                    {
                        Taunt();
                    }
                }
            }
        }

        public void Taunt()
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            SoundEngine.PlaySound(new SoundStyle("StupidMode/Assets/Sounds/taunt"), Player.position);
            modPlayer.taunting = 20;
            velBeforeTaunting = Player.velocity;
        }

        public void TauntParry(Player.HurtInfo info)
        {
            StupidPlayer modPlayer = Player.GetModPlayer<StupidPlayer>();
            SoundEngine.PlaySound(new SoundStyle("StupidMode/Assets/Sounds/tauntParry"), Player.position);
            Player.AddBuff(ModContent.BuffType<Content.Buffs.Overconfident>(), 1800);
            modPlayer.taunting = 0;
            Player.SetImmuneTimeForAllTypes(60);
            Player.immuneNoBlink = true;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.HasBuff(ModContent.BuffType<Content.Buffs.ShadowState>()) && Main.myPlayer == Player.whoAmI)
            {
                r = 0;
                g = 0;
                b = 0;
                a = 0.5f;
            }
        }

    }
}
