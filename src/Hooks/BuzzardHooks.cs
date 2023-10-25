using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeadlandsCreatures.Hooks
{
    internal class BuzzardHooks
    {

        public static void Apply()
        {

            // Vulture

            On.Vulture.ctor += OnVultureCtor;

            On.Vulture.AirBrake += OnAirBrake;

            IL.Vulture.ctor += ILVutureCtor;
            IL.Vulture.Violence += ILVultureViolence;

            // State

            On.Vulture.VultureState.ctor += OnVultureStateCtor;

            // AI

            On.VultureAI.ctor += OnVultureAICtor;

            // Graphics

            On.VultureGraphics.ctor += OnVultureGraphicsCtor;

            IL.VultureGraphics.ctor += ILVultureGraphicsCtor;
            IL.VultureGraphics.InitiateSprites += ILInitiateSprites;

            // Vulture Mask

            
        }

        

        #region Vulture

        private static void ILVutureCtor(ILContext il)
        {
            var c = new ILCursor(il);

            ILLabel postCheck = null;

            if (c.TryGotoNext(MoveType.After, // Smaller body chunks
                x => x.MatchLdcR4(1.4f),
                x => x.MatchStloc(0)))
            {
                c.Emit(OpCodes.Ldarg_0); // Thank you forthbridge for this method
                c.Emit(OpCodes.Ldloc_0);
                c.EmitDelegate<Func<Vulture, float, float>>((vulture, value) =>
                {
                    if (vulture.Template.type == Type.Buzzard)
                    {
                        return 0.7f;
                    }
                    return value;
                });
                c.Emit(OpCodes.Stloc_0);
            }
            else
            {
                Plugin.Logger.LogFatal("ILVutureCtor failed!");
            }
        }
        private static void OnVultureStateCtor(On.Vulture.VultureState.orig_ctor orig, Vulture.VultureState self, AbstractCreature creature)
        {
            orig(self, creature);
            // Extra Wing Health, bigger wings can take a bigger beating
            if (creature.creatureTemplate.type == Type.Buzzard)
            {
                for (int i = 0; i < self.wingHealth.Length; i++)
                {
                    self.wingHealth[i] = 2f;
                }
            }
        }

        private static void OnAirBrake(On.Vulture.orig_AirBrake orig, Vulture self, int frames)
        {
            // Faster Air Braking
            if(self.Template.type == Type.Buzzard && frames > 5)
            {
                frames = frames - 5;
            }
            orig(self, frames);
        }

        private static void OnVultureCtor(On.Vulture.orig_ctor orig, Vulture self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.Template.type == Type.Buzzard)
            {
                foreach (var chunk in self.bodyChunks)
                {
                    chunk.rad = chunk.rad - 2.5f;
                }
            }
        }

        private static void OnVultureAICtor(On.VultureAI.orig_ctor orig, VultureAI self, AbstractCreature creature, World world)
        {
            orig(self, creature, world);
            if (creature.creatureTemplate.type == Type.Buzzard)
            {
                self.pathFinder.accessibilityStepsPerFrame = 70;
                self.pathFinder.stepsPerFrame = 60;
                foreach (var module in self.modules)
                {
                    if (module is PreyTracker preyTracker)
                    {
                        preyTracker.persistanceBias = 2f;
                    }
                }
            }
        }

        private static void ILVultureViolence(ILContext il)
        {
            var c = new ILCursor(il);

            if (c.TryGotoNext(MoveType.After, // Less disencouragement which means more presistance
                x => x.MatchCallvirt(typeof(VultureAI).GetMethod("set_disencouraged", new[] { typeof(float) }))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<Vulture>>((vulture) =>
                {
                    if (vulture.Template.type == Type.Buzzard)
                    {
                        Debug.Log("Buzzard Violence!");
                        vulture.AI.disencouraged = vulture.AI.disencouraged * 0.15f;
                    }
                });
            }
            else
            {
                Plugin.Logger.LogFatal("ILVultureViolence failed!");
            }
        }

        private static void OnVultureGraphicsCtor(On.VultureGraphics.orig_ctor orig, VultureGraphics self, Vulture ow)
        {
            orig(self, ow);
            if (self.vulture.Template.type == Type.Buzzard)
            {
                self.ColorA = new HSLColor(1, 1, 0.6f);
                self.ColorB = new HSLColor(0.5f, 1, 0.6f);
            }
        }

        private static void ILVultureGraphicsCtor(ILContext il)
        {
            var c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdcI4(0x19),
                x => x.MatchCall(typeof(UnityEngine.Random).GetMethod("Range", new[] { typeof(int), typeof(int) })),
                x => x.MatchStfld(typeof(VultureGraphics).GetField("feathersPerWing"))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<VultureGraphics>>((graphics) =>
                {
                    if (graphics.vulture.Template.type == Type.Buzzard)
                    {
                        graphics.feathersPerWing = 0;
                    }
                });
            }
            else
            {
                Plugin.Logger.LogFatal("ILVultureGraphicsCtor failed!");
            }
        }

        private static void ILInitiateSprites(ILContext il)
        {
            var c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdcR4(1.2f),
                x => x.MatchCallvirt(typeof(FNode).GetMethod("set_scale"))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldfld, typeof(RoomCamera.SpriteLeaser).GetField("sprites"));
                c.EmitDelegate<Action<VultureGraphics, FSprite[]>>((graphics, sprites) =>
                {
                    Debug.Log(sprites);
                    if (graphics.vulture.Template.type == Type.Buzzard)
                    {
                        sprites[graphics.BodySprite].scale = 0.8f;
                    }
                });
            }
            else
            {
                Plugin.Logger.LogFatal("ILInitiateSprites failed!");
            }
        }

        #endregion

    }
}
