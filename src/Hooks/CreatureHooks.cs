using DeadlandsCreatures.Features.Buzzard;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeadlandsCreatures.Hooks
{
    internal class CreatureHooks
    {
        // Hooks covering various aspects of the game involving creatures as well as individuals.

        // Whenever you add a new creature make sure you add it to all of the AbstractCreature and CreatureTemplate hooks like I've done.
        // self is not necessary if you're using a mod which does self for you such as Fisob or Lizard Customizer, in short, do what you need to do.

        // Make sure your thuroughly review any addidtions you make to prevent conflicts and ensure everything fits and works properly.

        // From a design perspective consider how a creature will adapt and survive in an enviornment with as big of a contrast to Rain World as the Deadlands
        // rather than just how it'll eat the player. Follow the same principles Rain World was built upon where every creature has the same agency as the player.

        // Deadlands Creatures

        // Buzzard: Vulture varient that is faster, slimmer and very aggressive and persistant.
        //          Primarally a scavenger but will persue living food and won't give up without a fight. Might be able to pick up and use spears and rocks too.
        // SpinePlant: Monsterkelp like plant that uses a harpoon similar to a king vulture.
        //          If damaged critically it will eject its spines peircing most creatures nearby and dealing damage.
        //          This creature is not a varient of the MonsterKelp but is pretty similar so it uses its own code for everything.
        public static void Apply()
        {
            // Static World

            On.StaticWorld.InitCustomTemplates += OnInitCustomTemplates;
            On.StaticWorld.InitStaticWorld += OnInitStaticWorld;

            // WorldLoader

            On.WorldLoader.CreatureTypeFromString += OnCreatureTypeFromString;

            // AbstractCreature

            On.AbstractCreature.InitiateAI += OnInitiateAI;
            On.AbstractCreature.ctor += OnAbstractCreatureCtor;
            On.AbstractCreature.Update += OnAbstractCreatureUpdate;

            //IL.AbstractCreature.ctor += ILAbstractCreatureCtor;
            IL.AbstractCreature.Realize += ILRealize;

            // CreatureTemplate

            On.CreatureTemplate.ctor_Type_CreatureTemplate_List1_List1_Relationship += OnCreatureTemplateCtor;

            // Additional Debug

            On.DebugMouse.Update += (orig, self, eu) =>
            {
                orig(self, eu);
                if (!self.room.readyForAI || !self.room.BeingViewed) return;

                string text = self.label.text;
                text += $"\n\n--buzzardMod--\n";
                for (int i = 0; i < self.room.physicalObjects.Length; i++)
                {
                    for(int n = 0; n < self.room.physicalObjects[i].Count; n++)
                    {
                        if (self.room.physicalObjects[i][n] != null && self.room.physicalObjects[i][n] is Vulture)
                        {
                            Vulture vulture = (Vulture)self.room.physicalObjects[i][n];
                            if (vulture.Template.type == Type.Buzzard)
                            {
                                BuzzardModule module = null;
                                BuzzardHooks.BuzzardModule.TryGetValue(vulture, out module);
                                if (module != null)
                                {
                                    text += $"idx: " + n + "\n" + $"grb: {module.grabChunk}     wGrb: {module.wantToGrabChunk}";
                                }
                            }
                        }
                    }
                }
                
                self.label.text = text;
                self.label2.text = text;
            };

            // Creature Specific Hooks
            BuzzardHooks.Apply();
            SaltWormHooks.Apply();
            //SpinePlantHooks.Apply();
        }

        #region StaticWorld
        private static void OnInitCustomTemplates(On.StaticWorld.orig_InitCustomTemplates orig)
        {
            orig();

            List<TileTypeResistance> list2 = new();
            List<TileConnectionResistance> list3 = new();

            // Populate with CreatureTemplates for any creatures you're adding manually
            #region Buzzard

            CreatureTemplate creatureTemplate = StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Vulture);

            if (creatureTemplate == null)
            {
                Debug.Log("Ancestor not found!");
            }

            list2.Add(new TileTypeResistance(AItile.Accessibility.Air, 1f, PathCost.Legality.Allowed));
            list2.Add(new TileTypeResistance(AItile.Accessibility.OffScreen, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.Standard, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OutsideRoom, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.SkyHighway, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.OffScreenMovement, 1f, PathCost.Legality.Allowed));
            list3.Add(new TileConnectionResistance(MovementConnection.MovementType.SideHighway, 10f, PathCost.Legality.Allowed));
            CreatureTemplate template = (new CreatureTemplate(Type.Buzzard, creatureTemplate, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f)));
            template.baseDamageResistance = 9.5f;
            template.baseStunResistance = 7f;
            template.abstractedLaziness = 9;
            template.AI = true;
            template.requireAImap = true;
            template.canSwim = false;
            template.canFly = true;
            template.doPreBakedPathing = false;
            template.preBakedPathingAncestor = creatureTemplate;
            template.offScreenSpeed = 2.5f;
            template.bodySize = 5f;
            template.grasps = 1;
            template.stowFoodInDen = true;
            template.shortcutSegments = 5;
            template.visualRadius = 7000f;
            template.movementBasedVision = 0.4f;
            template.waterVision = 0.8f;
            template.throughSurfaceVision = 0.8f;
            template.hibernateOffScreen = true;
            template.dangerousToPlayer = 0.7f;
            template.meatPoints = 13;
            template.communityInfluence = 0.25f;
            template.socialMemory = true;
            template.waterRelationship = CreatureTemplate.WaterRelationship.AirAndSurface;
            template.jumpAction = "Fly";
            template.pickupAction = "Ensnare";
            template.throwAction = "Release";
            list2.Clear();
            list3.Clear();

            StaticWorld.creatureTemplates[Type.Buzzard.Index] = template;
            #endregion
            /*
            #region SpinePlant
            CreatureTemplate spinePlant = new CreatureTemplate(Type.SpinePlant, null, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 1f));
            spinePlant.baseDamageResistance = 3.5f;
            spinePlant.baseStunResistance = 1f;
            spinePlant.AI = true;
            spinePlant.requireAImap = false;
            spinePlant.doPreBakedPathing = false;
            spinePlant.stowFoodInDen = true;
            spinePlant.offScreenSpeed = 0f;
            spinePlant.bodySize = 2f;
            spinePlant.grasps = 1;
            spinePlant.visualRadius = 400f;
            spinePlant.movementBasedVision = 1f;
            spinePlant.waterVision = 0.7f;
            spinePlant.throughSurfaceVision = 0f;
            spinePlant.dangerousToPlayer = 0.5f;
            spinePlant.communityInfluence = 0.3f;
            spinePlant.wormGrassImmune = true;
            spinePlant.waterRelationship = CreatureTemplate.WaterRelationship.AirAndSurface;
            spinePlant.BlizzardWanderer = true;
            spinePlant.pickupAction = "Snatch";
            spinePlant.throwAction = "Harpoon";
            list2.Clear();
            list3.Clear();

            StaticWorld.creatureTemplates[Type.SpinePlant.Index] = spinePlant;
            #endregion

            #region SaltWorm

            CreatureTemplate creatureTemplate15 = StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Centipede);

            if (creatureTemplate == null)
            {
                Debug.Log("Ancestor not found!");
            }

            CreatureTemplate saltWorm = new CreatureTemplate(CreatureTemplate.Type.RedCentipede, creatureTemplate15, list2, list3, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 1f));
            saltWorm.baseDamageResistance = 1.5f;
            saltWorm.visualRadius = 1100f;
            saltWorm.communityInfluence = 0.25f;
            saltWorm.meatPoints = 1;
            saltWorm.shortcutColor = Color.white;
            saltWorm.shortcutSegments = 5;
            saltWorm.bodySize = 8.5f;
            saltWorm.dangerousToPlayer = 0.7f;
            saltWorm.BlizzardWanderer = false;
            saltWorm.BlizzardAdapted = false;
            saltWorm.jumpAction = "Swap Heads";
            saltWorm.pickupAction = "Grab/Smother";
            saltWorm.throwAction = "Release";
            list2.Clear();
            list3.Clear();

            #endregion
            */
            
        }
        private static void OnInitStaticWorld(On.StaticWorld.orig_InitStaticWorld orig)
        {
            Plugin.Logger.LogFatal("OnInitStaticWorld Begin");

            orig();

            // Establish Relationships here
            Plugin.Logger.LogFatal("OnInitStaticWorld Buzzard");
            #region Buzzard

            StaticWorld.EstablishRelationship(Type.Buzzard, CreatureTemplate.Type.GreenLizard, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.55f));
            StaticWorld.EstablishRelationship(Type.Buzzard, CreatureTemplate.Type.RedCentipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.55f));

            StaticWorld.EstablishRelationship(CreatureTemplate.Type.RedLizard, Type.Buzzard, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 0.4f));

            #endregion
            /*
            Plugin.Logger.LogFatal("OnInitStaticWorld SpinePlant");
            #region SpinePlant

            StaticWorld.EstablishRelationship(Type.SpinePlant, Type.SpinePlant, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            StaticWorld.EstablishRelationship(Type.SpinePlant, CreatureTemplate.Type.TentaclePlant, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            StaticWorld.EstablishRelationship(Type.SpinePlant, CreatureTemplate.Type.PoleMimic, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            StaticWorld.EstablishRelationship(Type.SpinePlant, CreatureTemplate.Type.Leech, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f));
            StaticWorld.EstablishRelationship(Type.SpinePlant, CreatureTemplate.Type.MirosBird, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.8f));
            StaticWorld.EstablishRelationship(Type.SpinePlant, CreatureTemplate.Type.BigEel, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 1f));
            StaticWorld.EstablishRelationship(Type.SpinePlant, CreatureTemplate.Type.DaddyLongLegs, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.6f));
            StaticWorld.EstablishRelationship(Type.SpinePlant, CreatureTemplate.Type.Deer, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 0.2f));

            StaticWorld.EstablishRelationship(CreatureTemplate.Type.Slugcat, Type.SpinePlant, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 1f));
            #endregion
            Plugin.Logger.LogFatal("OnInitStaticWorld SaltWorm");
            #region SaltWorm
            // Todo: Relationships
            #endregion
            */
            Plugin.Logger.LogFatal("OnInitStaticWorld End");
        }
        #endregion

        #region WorldLoader
        private static CreatureTemplate.Type OnCreatureTypeFromString(On.WorldLoader.orig_CreatureTypeFromString orig, string s)
        {
            // String Conversion Here

            if (s.Equals("buzzard", StringComparison.OrdinalIgnoreCase))
            {
                return Type.Buzzard;
            }
            if (s.Equals("spineplant", StringComparison.OrdinalIgnoreCase))
            {
                return Type.SpinePlant;
            }
            if (s.Equals("saltworm", StringComparison.OrdinalIgnoreCase))
            {
                return Type.SaltWorm;
            }
            return orig(s);
        }
        #endregion

        #region AbstractCreature

        private static void OnAbstractCreatureUpdate(On.AbstractCreature.orig_Update orig, AbstractCreature self, int time)
        {
            orig(self, time);
            if (ModManager.MMF && !self.InDen && self.state.alive && (self.creatureTemplate.type == Type.SpinePlant) && self.pos.abstractNode != -1 && self.GetNodeType != AbstractRoomNode.Type.Den)
            {
                self.destroyOnAbstraction = true;
                self.Die();
            }
        }

        private static void OnAbstractCreatureCtor(On.AbstractCreature.orig_ctor orig, AbstractCreature self, World world, CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID ID)
        {
            orig(self, world, creatureTemplate, realizedCreature, pos, ID);

            // Creature States

            if (creatureTemplate.type == Type.Buzzard && creatureTemplate.TopAncestor().type == CreatureTemplate.Type.Vulture)
            {
                self.state = new Vulture.VultureState(self);
            }

            // Creature Abstract AI

            if (creatureTemplate.AI)
            {
                if (creatureTemplate.type == Type.Buzzard)
                {
                    self.abstractAI = new VultureAbstractAI(world, self);
                }

                bool setDenPos = pos.abstractNode > -1 && pos.abstractNode < self.Room.nodes.Length
                    && self.Room.nodes[pos.abstractNode].type == AbstractRoomNode.Type.Den && !pos.TileDefined;

                if (setDenPos)
                {
                    self.abstractAI.denPosition = pos;
                }
            }

            // Extra Stuff

            if (creatureTemplate.type == Type.SpinePlant)
            {
                self.remainInDenCounter = 0;
            }
        }

        private static void ILAbstractCreatureCtor(ILContext il)
        {
            var c = new ILCursor(il);

            ILLabel postCheck = null;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCall(typeof(AbstractWorldEntity).GetMethod("get_Room")),
                x => x.MatchBrtrue(out postCheck),
                x => x.MatchLdarg(4)))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<AbstractCreature>>((abstractCreature) =>
                {
                    // More Abstract AI if needed
                    if (abstractCreature.creatureTemplate.AI && abstractCreature.creatureTemplate.type == Type.Buzzard)
                    {
                        abstractCreature.abstractAI = new VultureAbstractAI(abstractCreature.world, abstractCreature);
                    }
                });
            }
        }

        private static void ILRealize(ILContext il)
        {
            var c = new ILCursor(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCall(typeof(AbstractCreature).GetMethod("InitiateAI"))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<AbstractCreature>>((abstractCreature) =>
                {
                    // Creature Realization
                    if (abstractCreature.creatureTemplate.TopAncestor().type == Type.Buzzard)
                    {
                        abstractCreature.realizedCreature = new Vulture(abstractCreature, abstractCreature.world);
                    }
                    if (abstractCreature.creatureTemplate.TopAncestor().type == CreatureTemplate.Type.TentaclePlant)
                    {
                        if (abstractCreature.creatureTemplate.type == Type.SpinePlant)
                        {
                            abstractCreature.realizedCreature = new TentaclePlant(abstractCreature, abstractCreature.world);
                        }
                    }
                    if (abstractCreature.creatureTemplate.TopAncestor().type == Type.SaltWorm)
                    {
                        abstractCreature.realizedCreature = new Centipede(abstractCreature, abstractCreature.world);
                    }
                });
            }

        }

        private static void OnInitiateAI(On.AbstractCreature.orig_InitiateAI orig, AbstractCreature self)
        {
            orig(self);
            if (self.creatureTemplate.type == Type.Buzzard)
            {
                self.abstractAI.RealAI = new BuzzardAI(self, self.world);
            }
        }

        #endregion

        #region CreatureTemplate

        private static void OnCreatureTemplateCtor(On.CreatureTemplate.orig_ctor_Type_CreatureTemplate_List1_List1_Relationship orig, CreatureTemplate self, CreatureTemplate.Type type, CreatureTemplate ancestor, List<TileTypeResistance> tileResistances, List<TileConnectionResistance> connectionResistances, CreatureTemplate.Relationship defaultRelationship)
        {
            // Creature Templates
            orig(self, type, ancestor, tileResistances, connectionResistances, defaultRelationship);
            if (type == Type.Buzzard)
            {
                self.name = "Buzzard";
            }
            if (type == Type.SpinePlant)
            {
                self.name = "SpinePlant";
            }
            if (type == Type.SaltWorm)
            {
                self.name = "SaltWorm";
            }
        }

        #endregion
    }
}
