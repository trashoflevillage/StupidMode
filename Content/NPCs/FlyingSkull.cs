using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace StupidMode.Content.NPCs
{
	public class FlyingSkull : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 1;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{ // Influences how the NPC looks in the Bestiary
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.width = 20;
			NPC.height = 22;
			NPC.damage = 22;
			NPC.defense = 8;
			NPC.lifeMax = 66;
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A skull whose spirit has not fully left it yet."),
			});
		}

        public override void AI()
        {
			float speed = 0.2f;
			NPC.TargetClosest(false);
			NPC.rotation += 0.5f;
			if (!Main.player[NPC.target].dead)
			{
				Vector2 goTo = NPC.DirectionTo(Main.player[NPC.target].position);
				NPC.velocity.X += Math.Clamp(goTo.X, speed * -1, speed);
				NPC.velocity.Y += Math.Clamp(goTo.Y, (speed/2) * -1, speed/2);
			} else
            {
				NPC.noGravity = false;
            }
		}
    }
}