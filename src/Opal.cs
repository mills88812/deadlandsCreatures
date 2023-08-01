using System;
using System.Collections.Generic;
using System.Drawing.Text;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestMod
{
    public class Opal : PlayerCarryableItem, IDrawable, IPlayerEdible
    {
        	public AbstractConsumable AbstrConsumable
	{
		get
		{
			return this.abstractPhysicalObject as AbstractConsumable;
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
            base.bodyChunks = new BodyChunk[1];
            base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 8f, 0.2f);
            this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
            base.airFriction = 0.99999f;
            base.gravity = 0.9f;
            this.bounce = 0.3f;
            this.surfaceFriction = 0.7f;
            this.collisionLayer = 1;
            base.waterFriction = 0.95f;
            base.buoyancy = 1.1f;
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
                return this.bites;
            }
        }
        public void BitByPlayer(Creature.Grasp grasp, bool eu)
        {
            this.bites--;
            this.room.PlaySound((this.bites == 0) ? SoundID.Slugcat_Eat_Dangle_Fruit : SoundID.Slugcat_Bite_Dangle_Fruit, base.firstChunk.pos);
            base.firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
            if (this.bites < 1)
            {
                (grasp.grabber as Player).ObjectEaten(this);
                grasp.Release();
                this.Destroy();
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
            this.AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
            Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            this.lastDarkness = this.darkness;
            this.darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
            if (this.darkness != this.lastDarkness)
            {
                this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
            for (int i = 0; i < 2; i++)
            {
                sLeaser.sprites[i].x = vector.x - camPos.x;
                sLeaser.sprites[i].y = vector.y - camPos.y;
                sLeaser.sprites[i].rotation = Custom.VecToDeg(v);
                sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName("DangleFruit" + Custom.IntClamp(3 - this.bites, 0, 2).ToString() + ((i == 0) ? "A" : "B"));
            }
            if (this.blink > 0 && Random.value < 0.5f)
            {
                sLeaser.sprites[1].color = base.blinkColor;
            }
            else
            {
                sLeaser.sprites[1].color = this.color;
            }
            if (base.slatedForDeletetion || this.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            Color White = new Color(0.9f, 0.9f, 0.9f, 0.60f);


            sLeaser.sprites[0].color = White;
            this.color = Color.Lerp(new Color(0.5f, 1f, 0.7f), palette.blackColor, this.darkness);
        }








        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
        {
            newContainer ??= rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
                newContainer.AddChild(fsprite);
        }
    }





}