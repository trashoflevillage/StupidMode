using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using StupidMode.Common.Global;
using Terraria.Audio;

namespace StupidMode.Content.NPCs
{
	public class NinjaMinion : ModNPC
	{
		private int taunting = 0;
		private int tauntingCooldown = 1800;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 2;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{ // Influences how the NPC looks in the Bestiary
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 30;
			NPC.damage = 0;
			NPC.defense = 8;
			NPC.lifeMax = 1;
			NPC.dontTakeDamage = true;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 0f;
			NPC.knockBackResist = 0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Lego ninjago enthusiast."),
			});
		}

        public override void AI()
        {
            int kingSlime = NPC.FindFirstNPC(NPCID.KingSlime);
            if (kingSlime != -1)
			{
				if (taunting > 0)
				{
					NPC.frame.Y = 102;
					NPC.velocity = new Vector2(0, 0);
					taunting--;
				}
				else
				{
					NPC.frame.Y = 0;
					NPC.FaceTarget();
					Vector2 newVel = NPC.position.DirectionTo(new Vector2(Main.npc[kingSlime].Center.X, Main.npc[kingSlime].position.Y - 300)) + new Vector2(Main.rand.Next(-5, 5), 0);
					newVel.X = Math.Clamp(newVel.X, -30, 30);
					newVel.Y *= 3;
					newVel.Y = Math.Clamp(newVel.Y, -200f, 200f);
					NPC.velocity.X += newVel.X;
					NPC.velocity.Y = newVel.Y;
					tauntingCooldown--;
					if (tauntingCooldown == 0)
					{
						tauntingCooldown = 1800;
						Taunt();
					}
				}
            }
            else
            {
                NPC.active = false;
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
			return true;
        }

		private void Taunt()
        {
			taunting = 15;
			for (int i = 0; i < Main.rand.Next(4, 8); i++)
			{
				Vector2[] velocities = new Vector2[]
				{
					new Vector2(1, 1),
					new Vector2(1, -1),
					new Vector2(-1, 1),
					new Vector2(-1, -1),
					new Vector2(0, 1),
					new Vector2(1, 0),
					new Vector2(0, -1),
					new Vector2(-1, 0)
				};

				foreach (Vector2 v in velocities)
				{
					StupidNPC.NewHostileProjectile(NPC.GetSource_FromAI(), NPC.position, v * 10, ProjectileID.Shuriken, 5, 0);
				}
			}
			SoundEngine.PlaySound(new SoundStyle("StupidMode/Assets/Sounds/taunt"), NPC.position);
		}
    }
}