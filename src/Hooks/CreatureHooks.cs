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

        // Deadlands Creatures

        // Buzzard: Vulture varient that is faster, slimmer and very aggressive and persistant.
        //          Primarally a scavenger but will persue living food and won't give up without a fight. Might be able to pick up and use spears and rocks too.
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

            //IL.AbstractCreature.ctor += ILAbstractCreatureCtor;
            IL.AbstractCreature.Realize += ILRealize;

            // CreatureTemplate

            On.CreatureTemplate.ctor_Type_CreatureTemplate_List1_List1_Relationship += OnCreatureTemplateCtor;

            // Creature Specific Hooks
            BuzzardHooks.Apply();
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
        }
        private static void OnInitStaticWorld(On.StaticWorld.orig_InitStaticWorld orig)
        {
            orig();

            // Establish Relationships here

            #region Buzzard

            StaticWorld.EstablishRelationship(Type.Buzzard, CreatureTemplate.Type.GreenLizard, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.55f));
            StaticWorld.EstablishRelationship(Type.Buzzard, CreatureTemplate.Type.RedCentipede, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Eats, 0.55f));

            StaticWorld.EstablishRelationship(CreatureTemplate.Type.RedLizard, Type.Buzzard, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 0.4f));

            #endregion
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
            return orig(s);
        }
        #endregion

        #region AbstractCreature

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
                });
            }

        }

        private static void OnInitiateAI(On.AbstractCreature.orig_InitiateAI orig, AbstractCreature self)
        {
            orig(self);
            /*
            if (self.creatureTemplate.TopAncestor().type == DeadlandsCreatures.Type.Buzzard)
            {
                self.abstractAI.RealAI = new VultureAI(self, self.world);
                return;
            }*/
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
        }

        #endregion
    }
}
