﻿using RWCustom;
using UnityEngine;

namespace DeadlandsCreatures;

sealed class CactusChunk : PlayerCarryableItem, IDrawable, IPlayerEdible
{
    public AbstractConsumable AbstrConsumable => abstractPhysicalObject as AbstractConsumable;

    int bites = 2;

    public int BitesLeft => bites;

    public int FoodPoints => 0;

    public bool Edible => true;

    public bool AutomaticPickUp => true;

    public int chuckType;


    /////////////////////////////////////
    // Constructor
    /////////////////////////////////////

    public CactusChunk(AbstractPhysicalObject abstr, Vector2 pos, Vector2 vel) : base(abstr)
    {
        bodyChunks = new[] { new BodyChunk(this, 0, pos + vel, 4, 0.1f) { goThroughFloors = true, vel = vel } };
        bodyChunks[0].lastPos = bodyChunks[0].pos;


        base.bodyChunkConnections = new BodyChunkConnection[0];

        base.airFriction = 0.999f;
        base.gravity = 0.9f;
        base.bounce = 0.6f;
        base.surfaceFriction = 0.43f;
        base.collisionLayer = 1;
        base.waterFriction = 0.95f;
        base.buoyancy = 0.75f;
    }

    /////////////////////////////////////
    // More friction? I think?
    /////////////////////////////////////

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);

        if (direction != new IntVector2(0, -1)) return;

        this.bodyChunks[0].vel *= 0.5f;
    }

    /////////////////////////////////////
    // Stun
    /////////////////////////////////////

    public override void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        base.Collide(otherObject, myChunk, otherChunk);

        if (otherObject is Creature && this.firstChunk.vel.magnitude > 5)
        {
            Creature creature = (Creature)otherObject;
            creature.Stun(40);
            this.room.AddObject(new CreatureSpasmer(creature, true, creature.stun));
        }
    }

    /////////////////////////////////////
    // Consumption
    /////////////////////////////////////

    public void BitByPlayer(Creature.Grasp grasp, bool eu)
    {
        bites--;

        room.PlaySound((bites == 0) ? SoundID.Slugcat_Eat_Dangle_Fruit : SoundID.Slugcat_Bite_Dangle_Fruit, base.firstChunk.pos);

        base.firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);

        if (bites < 1)
        {
            if (room.game.IsStorySession)
            {
                (grasp.grabber as Player).ObjectEaten(this);
                (grasp.grabber as Player).AddQuarterFood();
                (grasp.grabber as Player).AddQuarterFood();
            } else
            {
                (grasp.grabber as Player).ObjectEaten(this);
                (grasp.grabber as Player).AddFood(1);
            }

            grasp.Release();
            Destroy();
        }
    }

    public void ThrowByPlayer()
    {
            
    }

    /////////////////////////////////////
    // Drawing
    /////////////////////////////////////

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];

        int spriteNumber = (this.abstractPhysicalObject as CactusChunkAbstract).spriteType;
        string spriteName = "cactus_fragment_0" + spriteNumber;
        if (spriteNumber > 9)
        {
            spriteName = "cactus_fragment_" + spriteNumber;
        }

        sLeaser.sprites[0] = new FSprite(spriteName);

        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(base.bodyChunks[0].lastPos, base.bodyChunks[0].pos, timeStacker);

        sLeaser.sprites[0].color = color;
        sLeaser.sprites[0].x = pos.x - camPos.x;
        sLeaser.sprites[0].y = pos.y - camPos.y;
        sLeaser.sprites[0].scale = 0.45f * bites;


        if (blink > 2)
            sLeaser.sprites[0].color = blinkColor;
        else
            sLeaser.sprites[0].color = color;


        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        color = Color.white;
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }
}
