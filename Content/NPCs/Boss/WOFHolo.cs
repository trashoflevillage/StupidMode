using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace StupidMode.Content.NPCs.Boss
{
    internal class WOFHolo : ModNPC
	{
		private const int protectionFrame = 0;
		private const int recoveryFrame = 1;

		private const int frameHeight = 610;

		ref float recoverTime => ref NPC.ai[0];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
        }
		public override void SetDefaults()
		{
			NPC.width = 110;
			NPC.height = 610;
			NPC.damage = 0;
			NPC.defense = 6;
			NPC.lifeMax = 1000;
			NPC.knockBackResist = 0f;
			NPC.ShowNameOnHover = true;
			NPC.noTileCollide = true;
			NPC.lavaImmune = true;
			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.noGravity = true;
		}

        public override void OnSpawn(IEntitySource source)
		{
			NPC wof = Main.npc[NPC.FindFirstNPC(NPCID.WallofFlesh)];
		}

        public override void AI()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}

			if (recoverTime > 0)
            {
				recoverTime--;
				NPC.frame.Y = recoveryFrame * frameHeight;
				NPC.dontTakeDamage = true;
				NPC.Opacity = 0.7f;
			} else
            {
				NPC.frame.Y = protectionFrame * frameHeight;
				NPC.dontTakeDamage = false;
				NPC.Opacity = 0.8f;
			}

			int wof = NPC.FindFirstNPC(NPCID.WallofFlesh);

			if (wof == -1 || !Main.npc[wof].active)
            {
				NPC.active = false;
            } else
            {
				NPC wofNPC = Main.npc[wof];
				NPC.direction = wofNPC.direction;
				NPC.spriteDirection = wofNPC.spriteDirection;
				NPC.position.X = wofNPC.position.X + (wofNPC.direction * wofNPC.width * 2);
				NPC.position.Y = wofNPC.Center.Y - (NPC.height/2);
			}
		}

        public override bool CheckDead()
        {
			NPC.life = NPC.lifeMax;
			recoverTime = 1800;
			return false;
        }

        public override bool CheckActive()
		{
			int wof = NPC.FindFirstNPC(NPCID.WallofFlesh);
			
			if (wof == -1 || !Main.npc[wof].active)
			{
				return true;
			}
			return false;
		}
    }
}
