using System;
using System.Collections.Generic;
using System.Drawing.Text;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeadlandsCreatures.Features.Opal
{
    public class Opal : PlayerCarryableItem, IDrawable, IPlayerEdible
    {
        public AbstractConsumable AbstrConsumable
        {
            get
            {
                return abstractPhysicalObject as AbstractConsumable;
            }
        }



        public Vector2 rotation;

        public Vector2 lastRotation;

        public Vector2? setRotation;

        public float darkness;

        public float lastDarkness;


        public int bites = 3;

        public Opal(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 8f, 0.2f);
            bodyChunkConnections = new BodyChunkConnection[0];
            airFriction = 0.99999f;
            gravity = 0.9f;
            bounce = 0.3f;
            surfaceFriction = 0.7f;
            collisionLayer = 1;
            waterFriction = 0.95f;
            buoyancy = 1.1f;
        }



        public void ThrowByPlayer()
        {
        }

        public int FoodPoints
        {
            get
            {
                return 1;
            }
        }

        public bool Edible
        {
            get
            {
                return true;
            }
        }

        public bool AutomaticPickUp
        {
            get
            {
                return true;
            }
        }

        public int BitesLeft
        {
            get
            {
                return bites;
            }
        }
        public void BitByPlayer(Creature.Grasp grasp, bool eu)
        {
            bites--;
            room.PlaySound(bites == 0 ? SoundID.Slugcat_Eat_Dangle_Fruit : SoundID.Slugcat_Bite_Dangle_Fruit, firstChunk.pos);
            firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
            if (bites < 1)
            {
                (grasp.grabber as Player).ObjectEaten(this);
                grasp.Release();
                Destroy();
            }
        }


        public override void Update(bool eu)
        {
            base.Update(eu);
        }

        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);

            Vector2 center = placeRoom.MiddleOfTile(abstractPhysicalObject.pos);

            int i = 0;
            bodyChunks[i].HardSetPosition(new Vector2(1, 1) * 20f + center);
        }




        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[2];
            sLeaser.sprites[0] = new FSprite("DangleFruit0A", true);
            sLeaser.sprites[1] = new FSprite("DangleFruit0B", true);
            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            Vector2 v = Vector3.Slerp(lastRotation, rotation, timeStacker);
            lastDarkness = darkness;
            darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
            if (darkness != lastDarkness)
            {
                ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
            for (int i = 0; i < 2; i++)
            {
                sLeaser.sprites[i].x = vector.x - camPos.x;
                sLeaser.sprites[i].y = vector.y - camPos.y;
                sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
                sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName("DangleFruit" + Custom.IntClamp(3 - bites, 0, 2).ToString() + (i == 0 ? "A" : "B"));
            }
            if (blink > 0 && Random.value < 0.5f)
            {
                sLeaser.sprites[1].color = blinkColor;
            }
            else
            {
                sLeaser.sprites[1].color = color;
            }
            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            Color White = new Color(0.9f, 0.9f, 0.9f, 0.60f);


            sLeaser.sprites[0].color = White;
            color = Color.Lerp(new Color(0.5f, 1f, 0.7f), palette.blackColor, darkness);
        }








        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
        {
            newContainer ??= rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
                newContainer.AddChild(fsprite);
        }
    }





}