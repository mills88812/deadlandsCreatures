using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using RWCustom;

namespace LizardCosmetics;

public class TailTipGlow : Template
{
    public int bumps;

    public float spineLength;

    public float sizeSkewExponent;

    public float sizeRangeMin;

    public float sizeRangeMax;

    public bool coloredHawk;


    private LightSource flatLightSource;

    internal Room room;

    internal TailSegment tailtip;

    private readonly LightSource[] lightSources = new LightSource[3];
    public TailTipGlow(LizardGraphics lGraphics, int startSprite)
        : base(lGraphics, startSprite)
    {
        coloredHawk = UnityEngine.Random.value < 0.5f;
        spritesOverlap = SpritesOverlap.BehindHead;
        float num;
        if (coloredHawk)
        {
            num = Mathf.Lerp(3f, 8f, Mathf.Pow(UnityEngine.Random.value, 0.7f));
            spineLength = Mathf.Lerp(0.3f, 0.7f, UnityEngine.Random.value) * lGraphics.BodyAndTailLength;
            sizeRangeMin = Mathf.Lerp(0.1f, 0.2f, UnityEngine.Random.value);
            sizeRangeMax = Mathf.Lerp(sizeRangeMin, 0.35f, Mathf.Pow(UnityEngine.Random.value, 0.5f));
        }
        else
        {
            num = Mathf.Lerp(6f, 12f, Mathf.Pow(UnityEngine.Random.value, 0.5f));
            spineLength = Mathf.Lerp(0.3f, 0.9f, UnityEngine.Random.value) * lGraphics.BodyAndTailLength;
            sizeRangeMin = Mathf.Lerp(0.2f, 0.3f, Mathf.Pow(UnityEngine.Random.value, 0.5f));
            sizeRangeMax = Mathf.Lerp(sizeRangeMin, 0.5f, UnityEngine.Random.value);
        }

        sizeSkewExponent = Mathf.Lerp(0.1f, 0.7f, UnityEngine.Random.value);
        bumps = (int)(spineLength / num);
        numberOfSprites = bumps;
    }

    public override void Update()
    {
        
        for (int i = 0; i < this.lightSources.Length; i++)
        {
            
            LightSource lightSource = this.lightSources[i];
            if (lightSource != null)
            {
                lightSource.stayAlive = true;
                lightSource.setPos = new Vector2?(Vector2.Lerp(lightSource.Pos, this.tailtip.pos , 0.2f));
                lightSource.setRad = new float?(Mathf.Lerp(lightSource.Rad, 8f, 0.2f));
                lightSource.setAlpha = new float?( Mathf.Min(lightSource.Alpha + 0.05f, 0.2f) );
                if (lightSource.slatedForDeletetion)
                {
                    this.lightSources[i] = null;
                }
            }
            else
            {
                Room room2 = room;
                LightSource[] array = this.lightSources;
                int num = i;
                LightSource lightSource2 = new LightSource(this.tailtip.pos, false, Custom.HSL2RGB(Mathf.Lerp(0.01f, 0.07f, (float)i / (float)(this.lightSources.Length - 1)), 1f, 0.5f), null);
                lightSource2.requireUpKeep = true;
                lightSource2.setAlpha = new float?( 0.2f );
                LightSource lightSource3 = lightSource2;
                array[num] = lightSource2;
                room2.AddObject(lightSource3);
            }
        }
        LightSource lightSource4 = this.flatLightSource;
        if (lightSource4 != null)
        {
            lightSource4.stayAlive = true;
            lightSource4.setAlpha = new float?( Mathf.Min(lightSource4.Alpha + 0.05f, Mathf.Lerp(0.1f, 0.2f, Random.value)) );
            lightSource4.setRad = new float?(Mathf.Lerp(24f, 33f, Random.value));
            lightSource4.setPos = new Vector2?(this.tailtip.pos);
            if (lightSource4.slatedForDeletetion)
            {
                this.flatLightSource = null;
            }
        }
        else
        {
            Room room3 = room;
            LightSource lightSource5 = new LightSource(this.tailtip.pos, false, Color.Lerp(Custom.HSL2RGB(Mathf.Lerp(0.01f, 0.07f, (float)(this.lightSources.Length - 1)), 1f, 0.5f), Color.white, 0.3f), null);
            lightSource5.flat = true;
            lightSource5.requireUpKeep = true;
            lightSource5.setAlpha = new float?(Mathf.Lerp(0.1f, 0.2f, Random.value) );
            LightSource lightSource3 = lightSource5;
            this.flatLightSource = lightSource5;
            room3.AddObject(lightSource3);
        }
        
        return;
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        for (int num = startSprite + numberOfSprites - 1; num >= startSprite; num--)
        {
            float num2 = Mathf.InverseLerp(startSprite, startSprite + numberOfSprites - 1, num);
            sLeaser.sprites[num] = new FSprite("DangleFruit0A");
            sLeaser.sprites[num].scale = Mathf.Lerp(sizeRangeMin, sizeRangeMax, Mathf.Lerp(Mathf.Sin(Mathf.Pow(num2, sizeSkewExponent) * (float)Math.PI), 1f, (num2 < 0.5f) ? 0.5f : 0f));
        }
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        for (int num = startSprite + numberOfSprites - 1; num >= startSprite; num--)
        {
            float num2 = Mathf.InverseLerp(startSprite, startSprite + numberOfSprites - 1, num);
            float num3 = Mathf.Lerp(0.05f, spineLength / lGraphics.BodyAndTailLength, num2);
            LizardGraphics.LizardSpineData lizardSpineData = lGraphics.SpinePosition(num3, timeStacker);
            sLeaser.sprites[num].x = lizardSpineData.outerPos.x - camPos.x;
            sLeaser.sprites[num].y = lizardSpineData.outerPos.y - camPos.y;
            if (coloredHawk || lGraphics.lizard.Template.type == CreatureTemplate.Type.WhiteLizard)
            {
                if (coloredHawk)
                {
                    sLeaser.sprites[num].color = Color.Lerp(lGraphics.HeadColor(timeStacker), lGraphics.BodyColor(num3), num2);
                }
                else
                {
                    sLeaser.sprites[num].color = lGraphics.DynamicBodyColor(num2);
                }
            }
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (!coloredHawk)
        {
            for (int i = startSprite; i < startSprite + numberOfSprites; i++)
            {
                float f = Mathf.Lerp(0.05f, spineLength / lGraphics.BodyAndTailLength, Mathf.InverseLerp(startSprite, startSprite + numberOfSprites - 1, i));
                sLeaser.sprites[i].color = lGraphics.BodyColor(f);
            }
        }
    }
}