using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using StupidMode.Content.Projectiles;
using StupidMode.Common.Global;
using Microsoft.Xna.Framework.Graphics;

namespace StupidMode.Content.NPCs
{
	public class CrimsonOrbMinion : ModNPC
	{
		public override string Texture => "StupidMode/Content/Items/Accessories/CrimsonOrb";

		public ref float AI_Owner => ref NPC.ai[0];
		public ref float Attack_Cooldown => ref NPC.ai[1];

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 1;
		}

		public override void SetDefaults()
		{
			NPC.width = 26;
			NPC.height = 26;
			NPC.damage = 200;
			NPC.defense = 999999;
			NPC.lifeMax = 999999;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 0f;
			NPC.knockBackResist = 0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.dontTakeDamage = true;
			NPC.friendly = true;
			NPC.ShowNameOnHover = false;
		}

		public override void AI()
		{
			NPC.position.X = Main.player[(int)AI_Owner].position.X;
			NPC.position.Y = Main.player[(int)AI_Owner].position.Y - 100;
			int? targetIndex = StupidNPC.FindClosestNPC(NPC.position, false, true, false, false, null, 800);

			if (targetIndex.HasValue)
			{
				Attack_Cooldown--;
				if (Attack_Cooldown == 0f)
				{
					NPC.target = targetIndex.Value;
					NPC target = Main.npc[targetIndex.Value];
					NPC.TargetClosest(false);
					Attack_Cooldown = 300;
					SoundEngine.PlaySound(SoundID.Item72, NPC.position);
					Projectile.NewProjectile(NPC.GetSource_FromAI("bloodBeamAttack"), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero),
						ModContent.ProjectileType<Bloodbeam>(), NPC.damage, 0, -1);
				}
			} else
            {
				Attack_Cooldown = 300;
            }
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPC.spriteDirection = -1;
			return base.PreDraw(spriteBatch, screenPos, drawColor);
		}
    }
}