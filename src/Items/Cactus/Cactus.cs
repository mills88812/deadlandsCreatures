using DeadlandsCreatures;
using MoreSlugcats;
using Noise;
using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeadlandsCreatures
{
    public class Cactus : Weapon, IDrawable, IPlayerEdible
    {
        public override bool HeavyWeapon
        {
            get
            {
                return true;
            }
        }

        public Cactus(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {
            base.bodyChunks = new BodyChunk[1];
            base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5.5f, 0.3f);
            this.bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
            base.airFriction = 0.999f;
            base.gravity = 0.9f;
            this.bounce = 0.4f;
            this.surfaceFriction = 0.4f;
            this.collisionLayer = 1;
            base.waterFriction = 0.98f;
            base.buoyancy = 0.4f;
            base.firstChunk.loudness = 4f;
            this.tailPos = base.firstChunk.pos;
            this.soundLoop = new ChunkDynamicSoundLoop(base.firstChunk);
            Random.State state = Random.state;
            Random.InitState(abstractPhysicalObject.ID.RandomSeed);
            Random.state = state;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            this.soundLoop.sound = SoundID.None;
            if (base.mode == Weapon.Mode.Free && this.collisionLayer != 1)
            {
                base.ChangeCollisionLayer(1);
            }
            else if (base.mode != Weapon.Mode.Free && this.collisionLayer != 2)
            {
                base.ChangeCollisionLayer(2);
            }
            if (base.firstChunk.vel.magnitude > 5f)
            {
                if (base.firstChunk.ContactPoint.y < 0)
                {
                    this.soundLoop.sound = SoundID.Rock_Skidding_On_Ground_LOOP;
                }
                else
                {
                    this.soundLoop.sound = SoundID.Rock_Through_Air_LOOP;
                }
                this.soundLoop.Volume = Mathf.InverseLerp(5f, 15f, base.firstChunk.vel.magnitude);
            }
            this.soundLoop.Update();
            if (base.firstChunk.ContactPoint.y != 0)
            {
                this.rotationSpeed = (this.rotationSpeed * 2f + base.firstChunk.vel.x * 5f) / 3f;
            }
        }

        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);
            if (this.floorBounceFrames > 0 && (direction.x == 0 || this.room.GetTile(base.firstChunk.pos).Terrain == Room.Tile.TerrainType.Slope))
            {
                return;
            }
            if(speed > 20f)
            {
                this.Explode(null);
            }
        }

        public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
        {
            if (result.obj == null)
            {
                return false;
            }
            this.ChangeMode(Weapon.Mode.Free);
            if (result.obj is Creature)
            {
                (result.obj as Creature).Violence(base.firstChunk, new Vector2?(base.firstChunk.vel * base.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Explosion, 0.8f, 85f);
            }
            else if (result.chunk != null)
            {
                result.chunk.vel += base.firstChunk.vel * base.firstChunk.mass / result.chunk.mass;
            }
            else if (result.onAppendagePos != null)
            {
                (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, base.firstChunk.vel * base.firstChunk.mass);
            }
            this.Explode(result.chunk);
            return true;
        }

        public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
        {
            base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            Room room = this.room;
            if (room != null)
            {
                room.PlaySound(SoundID.Slugcat_Throw_Rock, base.firstChunk);
            }
        }

        public override void HitByWeapon(Weapon weapon)
        {
            if (weapon.mode == Weapon.Mode.Thrown && this.thrownBy == null && weapon.thrownBy != null)
            {
                this.thrownBy = weapon.thrownBy;
            }
            base.HitByWeapon(weapon);
            this.Explode(null);
        }

        public override void WeaponDeflect(Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
        {
            base.WeaponDeflect(inbetweenPos, deflectDir, bounceSpeed);
            this.Explode(null);
        }

        public override void HitByExplosion(float hitFac, Explosion explosion, int hitChunk)
        {
            base.HitByExplosion(hitFac, explosion, hitChunk);
            if (Random.value < hitFac)
            {
                if (this.thrownBy == null)
                {
                    this.thrownBy = explosion.killTagHolder;
                }
                this.Explode(null);
            }
        }

        public void Explode(BodyChunk hitChunk)
        {
            if (base.slatedForDeletetion)
            {
                return;
            }

            Vector2 vector = Vector2.Lerp(base.firstChunk.pos, base.firstChunk.lastPos, 0.35f);

            this.room.AddObject(new Explosion(this.room, this, vector, 7, 75f, 2f, 2f, 280f, 0f, this.thrownBy, 0.7f, 160f, 0f));
            this.room.AddObject(new Explosion.ExplosionLight(vector, 160f, 1f, 3, Color.green));
            this.room.AddObject(new ExplosionSpikes(this.room, vector, 9, 4f, 5f, 5f, 90f, Color.green));
            this.room.AddObject(new ShockWave(vector, 60f, 0.045f, 4, false));

            for (int n = 34; n > 0; n--)
            {
                this.room.AddObject(new Spark(this.firstChunk.pos, Custom.RNV() * 2, Color.green, null, 10, 20));
            }

            this.room.PlaySound(SoundID.Snail_Pop, this.firstChunk.pos, 1f, 1.6f);
            this.room.PlaySound(SoundID.Rock_Hit_Creature, this.firstChunk.pos, 2f, 1.5f);

            for (int i = 0; i < 25; i++)
            {
                Vector2 a = Custom.RNV();
                if (this.room.GetTile(vector + a * 20f).Solid)
                {
                    if (!this.room.GetTile(vector - a * 20f).Solid)
                    {
                        a *= -1f;
                    }
                    else
                    {
                        a = Custom.RNV();
                    }
                }
            }
            CactusChunkExplosion.CactusExplosion(this.room, this.room.ToWorldCoordinate(base.firstChunk.pos), Random.Range(3, 9));
            
            this.room.ScreenMovement(new Vector2?(vector), default(Vector2), 0.6f);
            
            for (int m = 0; m < this.abstractPhysicalObject.stuckObjects.Count; m++)
            {
                this.abstractPhysicalObject.stuckObjects[m].Deactivate();
            }
            
            this.room.PlaySound(SoundID.Puffball_Eplode, vector);
            this.room.InGameNoise(new InGameNoise(vector, 9000f, this, 1f));
            bool flag = hitChunk != null;
            for (int n = 0; n < 5; n++)
            {
                if (this.room.GetTile(vector + Custom.fourDirectionsAndZero[n].ToVector2() * 20f).Solid)
                {
                    flag = true;
                    break;
                }
            }
            this.Destroy();
        }

        public void BitByPlayer(Creature.Grasp grasp, bool eu)
        {
            Bites--;
            if (Bites >= 0)
            {
                room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Duck_Pop, grasp.grabber.mainBodyChunk.pos, 1f, 0.5f + Random.value * 0.5f);
                CactusChunkExplosion.CactusExplosion(room, room.ToWorldCoordinate(firstChunk.pos), Random.Range(3, 9));
                grasp.Release();
                Destroy();
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("cactus_item");
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
            Vector2 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            this.lastDarkness = this.darkness;
            this.darkness = rCam.room.Darkness(vector) * (1f - rCam.room.LightSourceExposure(vector));
            if (this.darkness != this.lastDarkness)
            {
                this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }

                sLeaser.sprites[0].x = vector.x - camPos.x;
                sLeaser.sprites[0].y = vector.y - camPos.y;
                sLeaser.sprites[0].rotation = Custom.VecToDeg(v);
                sLeaser.sprites[0].scale = 0.75f;

            if (this.blink > 0 && Random.value < 0.5f)
            {
                sLeaser.sprites[0].color = base.blinkColor;
            }
            else
            {
                sLeaser.sprites[0].color = this.color;
            }
            if (base.slatedForDeletetion || this.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            color = Color.white;
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
        {
            newContainer ??= rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
                newContainer.AddChild(fsprite);
        }

        public void ThrowByPlayer()
        {

        }

        public AbstractConsumable AbstrConsumable => abstractPhysicalObject as AbstractConsumable;

        public int Bites = 1;

        public int BitesLeft => Bites;

        public int FoodPoints => 0;

        public bool Edible => true;

        public bool AutomaticPickUp => false;

        public Vector2? SetRotation;

        public float darkness;

        public float lastDarkness;
    }
}
