using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using IL;
using LizardCosmetics;
using MoreSlugcats;
using On;
using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeadlandsCreatures
{
    // Token: 0x0200001E RID: 30
    public class LizardCosmetics
    {
        // Token: 0x060000B7 RID: 183 RVA: 0x00011C64 File Offset: 0x0000FE64
        public static void Init()
        {
            On.LizardCosmetics.BumpHawk.ctor += BumpHawkNegation;

            On.LizardCosmetics.TailTuft.ctor += TailTuftNegation;

            On.LizardCosmetics.LongHeadScales.DrawSprites += LongHeadsize;
        }

        // Token: 0x060000B8 RID: 184 RVA: 0x00011DB4 File Offset: 0x0000FFB4
       
        // Token: 0x060000B9 RID: 185 RVA: 0x00011FF8 File Offset: 0x000101F8
        public static void BumpHawkNegation(On.LizardCosmetics.BumpHawk.orig_ctor orig, BumpHawk BH, LizardGraphics liz, int startSprite)
        {
            orig(BH, liz, startSprite);
            if (liz.lizard.Template.type == Type.Iguana )
            {
                BH.numberOfSprites = 0;
            }
            
        }

        // Token: 0x060000BA RID: 186 RVA: 0x00012050 File Offset: 0x00010250
        

        // Token: 0x060000BB RID: 187 RVA: 0x000120AC File Offset: 0x000102AC
        public static void TailTuftNegation(On.LizardCosmetics.TailTuft.orig_ctor orig, TailTuft TT, LizardGraphics liz, int startSprite)
        {
            orig(TT, liz, startSprite);
            if (liz.lizard.Template.type == Type.Iguana )
            {
                Array.Resize<LizardScale>(ref TT.scaleObjects, TT.scaleObjects.Length - TT.scalesPositions.Length);
                Array.Resize<Vector2>(ref TT.scalesPositions, TT.scalesPositions.Length - (TT.colored ? (TT.numberOfSprites / 2) : TT.numberOfSprites));
                TT.numberOfSprites = 0;
            }
            

        }

        public static void LongHeadsize(On.LizardCosmetics.LongHeadScales.orig_DrawSprites orig, LongHeadScales self, global::RoomCamera.SpriteLeaser sLeaser, global::RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            self.spritesOverlap = Template.SpritesOverlap.BehindHead;
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self.lGraphics.lizard.Template.type == Type.GlowLizard)
            {
               

                

              




            }
        }
        // Token: 0x060000BC RID: 188 RVA: 0x0001214C File Offset: 0x0001034C


    }
}
