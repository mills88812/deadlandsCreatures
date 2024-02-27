using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using LizardCosmetics;
using System.Collections.Generic;
using System.Drawing;



namespace DeadlandsCreatures.Hooks
{
    internal class GlowLizardHook
    {

         

        public static void Apply()
        {
            On.Lizard.ctor += OnGlowctor;
            On.LizardVoice.GetMyVoiceTrigger += OnGlowVoice;
            On.LizardTongue.ctor += OnTongeglow;
            On.LizardLimb.ctor += OnGlowLimb;

            // On.LizardAI.ctor += OnIguanaAIctor;
            On.LizardAI.Update += OnGlowAIUpdate;
            On.LizardGraphics.ctor += OnGlowcolorctor;
            On.LizardGraphics.Update += OnUpdateGraphics;
            On.LizardGraphics.DrawSprites += OnwhiteGlow;
            On.LizardGraphics.ColorBody += OnGlowcolorBody;
            //  On.LizardGraphics.BodyColor += OnIguanaBodycolor;
            // On.LizardGraphics.DynamicBodyColor += IguanaLizardBodyColors3;
            //  On.LizardGraphics.ApplyPalette += OnIguanacolorBody;

            // On.LizardGraphics.InitiateSprites += OnGlowInitiateSprites;
            IL.OverseerAbstractAI.HowInterestingIsCreature += ILOverseeIguana;

            
        }

        
        private static void OnGlowctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.Template.type == Type.GlowLizard)
            {

                var state = Random.state;
                Random.InitState(abstractCreature.ID.RandomSeed);

                self.tongue = new LizardTongue(self);
                self.effectColor = Custom.HSL2RGB(Custom.WrappedRandomVariation(.06f, .03f, .66f), 1f, Custom.ClampedRandomVariation(0.6f, .05f, 0.9f));

                Random.state = state;

            }



        }

        private static SoundID OnGlowVoice (On.LizardVoice.orig_GetMyVoiceTrigger orig, LizardVoice self)
        {
            SoundID res = orig(self);
            if (self.lizard is Lizard a && a.Template.type == Type.GlowLizard)
            {
                var array = new[] { "A", "B", "C", "D", "E" };
                var list = new List<SoundID>();
                for (int i = 0; i<array.Length; i++)
                {
                    var soundID = SoundID.None;
                    var text2 = "Lizard_Voice_Blue_" + array[i];
                    if (SoundID.values.entries.Contains(text2))
                        soundID = new (text2);
                    if (soundID != SoundID.None && soundID.Index != -1 && a.abstractCreature.world.game.soundLoader.workingTriggers[soundID.Index])
                        list.Add(soundID);
                }
                if (list.Count == 0)
                    res = SoundID.None;
                else
                    res = list[Random.Range(0, list.Count)];
            }
            return res;
        }
        private static void OnGlowLimb(On.LizardLimb.orig_ctor orig, LizardLimb self, GraphicsModule owner, BodyChunk connectionChunk, int num, float rad, float sfFric, float aFric, float huntSpeed, float quickness, LizardLimb otherLimbInPair)
        {
            if (owner is LizardGraphics iguana && iguana.lizard.Template.type == Type.GlowLizard)
            {
                self.grabSound = SoundID.Lizard_BlueWhite_Foot_Grab;
                self.releaseSeound = SoundID.Lizard_BlueWhite_Foot_Release;

            };


            orig(self, owner, connectionChunk, num, rad, sfFric, aFric, huntSpeed, quickness, otherLimbInPair);
        }
        private static void OnGlowcolorctor(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
        {
            orig(self, ow);

            Random.State state = Random.state;
            Random.InitState(self.lizard.abstractCreature.ID.RandomSeed);
            var num = self.startOfExtraSprites + self.extraSprites;
            int num2 = 0;
            if (self.lizard.Template.type == Type.GlowLizard)
            {





                if (Random.value < 0.1f)
                {
                    num = self.AddCosmetic(num, new BumpHawk(self, num));
                    num2++;
                }
                
                






                Random.state = state;
            }

        }
        private static void OnGlowAIUpdate(On.LizardAI.orig_Update orig, global::LizardAI self)
        {
            orig(self);

           
            if (self.lizard.Template.type == Type.GlowLizard)
            {
                self.noiseTracker.hearingSkill = Custom.LerpMap(self.runSpeed, 0f, 0.7f, 1.8f, 0.7f);
            }

        }
        private static void OnwhiteGlow(On.LizardGraphics.orig_DrawSprites orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);

            if (self.lizard.Template.type == Type.GlowLizard)
            {
                sLeaser.sprites[self.SpriteHeadStart + 1].color = new Color(1f, 1f, 1f);
                sLeaser.sprites[self.SpriteHeadStart + 2].color = new Color(1f, 1f, 1f);
                sLeaser.sprites[self.SpriteHeadStart + 4].color = new Color(1f, 1f, 1f);
                
            }
            
        }
        private static void OnTongeglow(On.LizardTongue.orig_ctor orig, LizardTongue self, Lizard lizard)
        {

            orig(self, lizard);
            if (lizard.Template.type == Type.GlowLizard)
            {
                self.range = 180f;
                GlowLizardHook.s_elasticRange.SetValue(self, 0.8f);
                GlowLizardHook.s_totR.SetValue(self, self.range * 1.1f);
                self.lashOutSpeed = 30f;
                self.reelInSpeed = 0.002f;
                self.chunkDrag = 0.1f;
                self.terrainDrag = 0.05f;
                self.dragElasticity = 0.02f;
                self.emptyElasticity = 0.003f;
                self.involuntaryReleaseChance = 0.006f;
                self.voluntaryReleaseChance = 0.01f;
                self.baseDragOnly = true;
                self.attachesBackgroundWalls = false;
                self.attachTerrainChance = 0.3f;
                self.pullAtChunkRatio = 0.05f;
                self.detatchMinDistanceTerrain = 60f;
                self.totRExtraLimit = 80f;
            }
        }
        /* private static void Onvoicess(On.LizardVoice.orig_GetMyVoiceTrigger orig, LizardVoice self)
         {



             SoundID result = orig(self);
                 Lizard lizard = self.lizard;
                 if (lizard != null)
                 {
                     string[] array = new string[]
                     {
                     "A",
                     "B",
                     "C",
                     "D",
                     "E"
                     };
                     List<SoundID> list = [];
                     if (lizard.Template.type == TypeL.GlowLizard)
                     {
                         for (int i = 0; i < array.Length; i++)
                         {
                             SoundID soundID = SoundID.None;
                             string text = "Lizard_Voice_Red_" + array[i];
                             if (ExtEnum<SoundID>.values.entries.Contains(text))
                             {
                                 soundID = new SoundID(text, false);
                             }
                             if (soundID != SoundID.None && soundID.Index != -1 && lizard.abstractCreature.world.game.soundLoader.workingTriggers[soundID.Index])
                             {
                                 list.Add(soundID);
                             }
                         }
                         if (list.Count == 0)
                         {
                     }
                     else
                         {
                         _ = list[Random.Range(0, list.Count)];
                         }
                     }
                 }
             return ;

         }*/
        private static void OnUpdateGraphics(On.LizardGraphics.orig_Update orig, LizardGraphics self)
        {
            orig(self);


           
            if (self.lizard.Template.type == Type.GlowLizard)
            {
                float lightMode = 0;
                if (self.lizard.animation == Lizard.Animation.ThreatReSpotted || self.lizard.animation == Lizard.Animation.PreyReSpotted || self.lizard.animation == Lizard.Animation.PreySpotted || self.lizard.animation == Lizard.Animation.FightingStance || self.lizard.animation == Lizard.Animation.ThreatSpotted)
                {
                    lightMode += 140;
                }
                if (self.lizard.animation == Lizard.Animation.HearSound)
                {
                    lightMode += 100;
                }
                if (self.lizard.Stunned)
                {

                    lightMode = 0;

                }
                if (self.lizard.dead)
                {

                    self.lightSource = null;

                }

                if (!self.lizard.dead )
                {

                    lightMode = Mathf.Lerp(lightMode, 5f, 1f / 20f);

                }
                if (self.lightSource != null)
                {
                    self.lightSource.stayAlive = true;
                    self.lightSource.setPos = self.head.pos;
                    self.lightSource.setRad = Mathf.Lerp(Mathf.Lerp(160f, 560f, lightMode * (100f + self.flicker)), ( lightMode) + 200f, 800f);
                    self.lightSource.setAlpha = Mathf.Lerp(Mathf.Lerp(0.8f, 0.3f, lightMode), 2f, Custom.SCurve(1f, 0.3f) * 0.5f) * 0.9f * (1f + self.flicker * 0.4f);
                    self.lightSource.color = Custom.HSL2RGB(Custom.WrappedRandomVariation(.06f, .03f, .66f), 1f, Custom.ClampedRandomVariation(0.6f, .03f, 0.9f));
                    if (self.lightSource.slatedForDeletetion || self.lizard.room.Darkness(self.head.pos) == 0f)
                    {
                        self.lightSource = null;
                    }
                }
                else if (self.lizard.room.Darkness(self.head.pos) > 0f)
                {
                    self.lightSource = new LightSource(self.head.pos, environmentalLight: false, new Color(1f, 1f, 1f), self.lizard);
                    self.lightSource.requireUpKeep = true;
                    self.lizard.room.AddObject(self.lightSource);
                }
                
                /*if (self.lightSource != null)
                {
                    self.lightSource.stayAlive = true;
                    self.lightSource.setPos = self.tail[4].pos;
                    self.lightSource.setRad = Mathf.Lerp(Mathf.Lerp(160f, 560f, lightMode * (100f + self.flicker)), (lightMode) + 200f, 800f);
                    self.lightSource.setAlpha = Mathf.Lerp(Mathf.Lerp(0.8f, 0.3f, lightMode), 2f, Custom.SCurve(1f, 0.3f) * 0.5f) * 0.9f * (1f + self.flicker * 0.4f);
                    self.lightSource.color = Custom.HSL2RGB(Custom.WrappedRandomVariation(.06f, .03f, .66f), 1f, Custom.ClampedRandomVariation(0.6f, .03f, 0.9f));
                    if (self.lightSource.slatedForDeletetion || self.lizard.room.Darkness(self.tail[4].pos) == 0f)
                    {
                        self.lightSource = null;
                    }
                }
                else if (self.lizard.room.Darkness(self.tail[4].pos) > 0f)
                {
                    self.lightSource = new LightSource(self.tail[4].pos, environmentalLight: false, new Color(1f, 1f, 1f), self.lizard);
                    self.lightSource.requireUpKeep = true;
                    self.lizard.room.AddObject(self.lightSource);
                }*/
            }

        }


        private static void OnGlowcolorBody(On.LizardGraphics.orig_ColorBody orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, Color col)
        {

            //.21f, .53f, .9f
            orig(self, sLeaser, col);


            if (self.lizard.Template.type == Type.GlowLizard)
            {


                for (int j = self.SpriteLimbsStart; j < self.SpriteLimbsEnd; j++)
                {
                    sLeaser.sprites[j].color = self.effectColor;
                }

            }

        }
        




        private static void OnGlowInitiateSprites(On.LizardGraphics.orig_InitiateSprites orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            int num3 = self.SpriteLimbsColorStart - self.SpriteLimbsStart;
            if (self.lizard.Template.type == Type.GlowLizard)
            {
                for (int num4 = self.SpriteLimbsStart; num4 < self.SpriteLimbsEnd; num4++)
                {
                    FSprite fSprite6 = new FSprite("pixel");
                    sLeaser.sprites[num4] = fSprite6;
                    fSprite6.x = -10000f;
                    fSprite6.scale = self.lizard.lizardParams.limbSize;
                    fSprite6.color = Custom.HSL2RGB(Custom.WrappedRandomVariation(.06f, .03f, .66f), 1f, Custom.ClampedRandomVariation(0.34f, .03f, .9f));
                    fSprite6 = new FSprite("pixel");
                    sLeaser.sprites[num4 + num3] = fSprite6;
                    fSprite6.x = -10000f;
                    fSprite6.scale = self.lizard.lizardParams.limbSize;
                    fSprite6.color = Custom.HSL2RGB(Custom.WrappedRandomVariation(.06f, .03f, .66f), 1f, Custom.ClampedRandomVariation(0.34f, .03f, .9f));
                }
            }
        }
        private static void ILOverseeIguana(ILContext il)
        {
            ILCursor c = new(il);
            ILLabel? label = null;
            if (c.TryGotoNext(
                x => x.MatchLdarg(1),
                x => x.MatchLdfld<AbstractCreature>("creatureTemplate"),
                x => x.MatchLdfld<CreatureTemplate>("type"),
                x => x.MatchLdsfld<CreatureTemplate.Type>("BlueLizard"),
                x => x.MatchCall(out _),
                x => x.MatchBrtrue(out label))
            && label != null)
            {
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate((AbstractCreature testCrit) => testCrit.creatureTemplate.type == Type.GlowLizard);
                c.Emit(OpCodes.Brtrue, label);
            }
            else
                Plugin.Logger.LogFatal("Couldn't ILHook OverseerAbstractAI.HowInteresting Is Glow Lizard!");

        }
        internal static void Dispose()
        {
            GlowLizardHook.s_elasticRange = null;
            GlowLizardHook.s_totR = null;
        }

        private static FieldInfo s_totR = typeof(global::LizardTongue).GetField("totR", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static FieldInfo s_elasticRange = typeof(LizardTongue).GetField("elasticRange", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }
}
