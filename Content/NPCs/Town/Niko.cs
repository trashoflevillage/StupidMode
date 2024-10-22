﻿using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.GameContent.UI;
using Terraria.ModLoader.IO;
using StupidMode.Common.Global;

namespace StupidMode.Content.NPCs.Town
{
	// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	[AutoloadHead]
	public class Niko : ModNPC
	{
		public static short bossHelp = 0;

		public int NumberOfTimesTalkedTo = 0;

		private static int ShimmerHeadIndex;
		private static Profiles.StackedNPCProfile NPCProfile;

		public override void Load()
		{
			// Adds our Shimmer Head to the NPCHeadLoader.
			ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
		}

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25; // The total amount of frames the NPC has

			NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
			NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames in the attacking animation.
			NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the NPC that it tries to attack enemies.
			NPCID.Sets.AttackType[Type] = 0; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
			NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
			NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.

			NPCID.Sets.ShimmerTownTransform[Type] = true; // Allows for this NPC to have a different texture after touching the Shimmer liquid.

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
							  // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
							  // If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			// Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
			// NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Like) // Example Person prefers the forest.
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Example Person dislikes the snow.
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // Loves living near the dryad.
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like) // Likes living near the guide.
				.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // Dislikes living near the merchant.
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate) // Hates living near the demolitionist.
			; // < Mind the semicolon!

			// This creates a "profile" for ExamplePerson, which allows for different textures during a party and/or while the NPC is shimmered.
			NPCProfile = new Profiles.StackedNPCProfile(
				new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
				new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
			);
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true; // Sets NPC to be a Town NPC
			NPC.friendly = true; // NPC Will not attack player
			NPC.width = 42;
			NPC.height = 60;
			NPC.aiStyle = 7;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;

			AnimationType = NPCID.Guide;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.StupidMode.Bestiary.Niko")
			});
		}

		public override bool CanTownNPCSpawn(int numTownNPCs)
		{
			return true;
		}

		public override ITownNPCProfile TownNPCProfile()
		{
			return NPCProfile;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Niko"
			};
		}

		public override void FindFrame(int frameHeight)
		{
			/*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
		}

		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();

			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(Language.GetTextValue("Mods.StupidMode.Dialogue.Niko.StandardDialogue1"));
			chat.Add(Language.GetTextValue("Mods.StupidMode.Dialogue.Niko.StandardDialogue2"));
			chat.Add(Language.GetTextValue("Mods.StupidMode.Dialogue.Niko.StandardDialogue3"));
			chat.Add(Language.GetTextValue("Mods.StupidMode.Dialogue.Niko.CommonDialogue"), 5.0);
			chat.Add(Language.GetTextValue("Mods.StupidMode.Dialogue.Niko.RareDialogue"), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 10)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(Language.GetTextValue("Mods.StupidMode.Dialogue.Niko.TalkALot"));
			}

			return chat; // chat is implicitly cast to a string.
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "Guidance (" + NPC.GetFullnameByID(StupidNPC.bosses.Keys.ToArray()[bossHelp]) + ")";
			button2 = "Cycle";
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton)
			{
				Main.npcChatText = getBossHelp(StupidNPC.bosses.Keys.ToArray()[bossHelp]);
			}
			else
			{
				bossHelp = (short)((bossHelp + 1) % StupidNPC.bosses.Count);
			}
		}

		private string getBossHelp(short boss) {
			if (!StupidNPC.bosses[boss])
            {
				return "Sorry, I haven't seen you fight that boss yet. Maybe get some experience with it before asking me for help!";
			} else
            {
				switch(boss)
                {
					default: return "Hmm... I'm stumped on this one. It looks like you're all on your own!\n(No guidance has been set yet. Sorry!)";
					case NPCID.KingSlime: return "King Slime shoots out slime spikes at a very high velocity. However, these are not very hard to avoid if you just keep a good distance! " +
                            "The ninja also stays pretty close to the slimy monarch, so you're safe from him too!";
					case NPCID.EyeofCthulhu: return "The eye seems to really rely on those nasty boulders. " +
                            "Avoid being under servants, and try hiding under the big guy when he does his boulder explosion! He doesn't shoot them below himself!";
                }
            }
		}

/*		public override void AddShops()
		{
			var npcShop = new NPCShop(Type, ShopName)
				.Add<ExampleItem>()
				//.Add<EquipMaterial>()
				//.Add<BossItem>()
				.Add(new Item(ModContent.ItemType<Items.Placeable.Furniture.ExampleWorkbench>()) { shopCustomPrice = Item.buyPrice(copper: 15) }) // This example sets a custom price, ExampleNPCShop.cs has more info on custom prices and currency. 
				.Add<Items.Placeable.Furniture.ExampleChair>()
				.Add<Items.Placeable.Furniture.ExampleDoor>()
				.Add<Items.Placeable.Furniture.ExampleBed>()
				.Add<Items.Placeable.Furniture.ExampleChest>()
				.Add<Items.Tools.ExamplePickaxe>()
				.Add<Items.Tools.ExampleHamaxe>()
				.Add<Items.Consumables.ExampleHealingPotion>(new Condition("Mods.ExampleMod.Conditions.PlayerHasLifeforceBuff", () => Main.LocalPlayer.HasBuff(BuffID.Lifeforce)))
				.Add<Items.Weapons.ExampleSword>(Condition.MoonPhasesQuarter0)
				//.Add<ExampleGun>(Condition.MoonPhasesQuarter1)
				.Add<Items.Ammo.ExampleBullet>(Condition.MoonPhasesQuarter1)
				//.Add<ExampleStaff>(Condition.MoonPhasesQuarter2)
				.Add<ExampleOnBuyItem>()
				.Add<Items.Weapons.ExampleYoyo>(Condition.IsNpcShimmered); // Let's sell an yoyo if this NPC is shimmered!

			if (ModContent.GetInstance<ExampleModConfig>().ExampleWingsToggle)
			{
				npcShop.Add<ExampleWings>(ExampleConditions.InExampleBiome);
			}

			if (ModContent.TryFind("SummonersAssociation/BloodTalisman", out ModItem bloodTalisman))
			{
				npcShop.Add(bloodTalisman.Type);
			}
			npcShop.Register(); // Name of this shop tab
		}

		public override void ModifyActiveShop(string shopName, Item[] items)
		{
			foreach (Item item in items)
			{
				// Skip 'air' items and null items.
				if (item == null || item.type == ItemID.None)
				{
					continue;
				}

				// If NPC is shimmered then reduce all prices by 50%.
				if (NPC.IsShimmerVariant)
				{
					int value = item.shopCustomPrice ?? item.value;
					item.shopCustomPrice = value / 2;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleCostume>()));
		}

		// Make this Town NPC teleport to the King and/or Queen statue when triggered. Return toKingStatue for only King Statues. Return !toKingStatue for only Queen Statues. Return true for both.
		public override bool CanGoToStatue(bool toKingStatue) => true;

		// Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
		public override void OnGoToStatue(bool toKingStatue)
		{
			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)ExampleMod.MessageType.ExampleTeleportToStatue);
				packet.Write((byte)NPC.whoAmI);
				packet.Send();
			}
			else
			{
				StatueTeleport();
			}
		}

		// Create a square of pixels around the NPC on teleport.
		public void StatueTeleport()
		{
			for (int i = 0; i < 30; i++)
			{
				Vector2 position = Main.rand.NextVector2Square(-20, 21);
				if (Math.Abs(position.X) > Math.Abs(position.Y))
				{
					position.X = Math.Sign(position.X) * 20;
				}
				else
				{
					position.Y = Math.Sign(position.Y) * 20;
				}

				Dust.NewDustPerfect(NPC.Center + position, ModContent.DustType<Sparkle>(), Vector2.Zero).noGravity = true;
			}
		}*/

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.MolotovCocktail;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 12f;
			randomOffset = 2f;
			// SparklingBall is not affected by gravity, so gravityCorrection is left alone.
		}

		public override void LoadData(TagCompound tag)
		{
			NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
		}

		public override void SaveData(TagCompound tag)
		{
			tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
		}

		// Let the NPC "talk about" minion boss
/*		public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
		{
			// By default this NPC will have a chance to use the Minion Boss Emote even if Minion Boss is not downed yet
			int type = ModContent.EmoteBubbleType<MinionBossEmote>();
			// If the NPC is talking to the Demolitionist, it will be more likely to react with angry emote
			if (otherAnchor.entity is NPC { type: NPCID.Demolitionist })
			{
				type = EmoteID.EmotionAnger;
			}

			// Make the selection more likely by adding it to the list multiple times
			for (int i = 0; i < 4; i++)
			{
				emoteList.Add(type);
			}

			// Use this or return null if you don't want to override the emote selection totally
			return base.PickEmote(closestPlayer, emoteList, otherAnchor);
		}*/
	}
}