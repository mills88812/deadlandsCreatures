using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Sandbox;
using Fisobs.Properties;

namespace DeadlandsCreatures
{
    public class CactusFisob : Fisob
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType AbstrCactus = new("Cactus", true);
        public static readonly PlacedObject.Type Cactus = new ("Cactus", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID mCactus = new("Cactus", true);

        public CactusFisob() : base(AbstrCactus)
        {
            Icon = new CactusIcon();

            SandboxPerformanceCost = new(linear: 0.2f, 0f);

            RegisterUnlock(mCactus, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 280);
        }
        
        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
        {
            // Crate data is just floats separated by ; characters.
            string[] p = saveData.CustomData.Split(';');

            if (p.Length < 5)
            {
                p = new string[5];
            }

            var result = new CactusAbstract(world, saveData.Pos, saveData.ID)
            {
                hue = float.TryParse(p[0], out var h) ? h : 0,
                saturation = float.TryParse(p[1], out var s) ? s : 1,
                scaleX = float.TryParse(p[2], out var x) ? x : 1,
                scaleY = float.TryParse(p[3], out var y) ? y : 1,
                damage = float.TryParse(p[4], out var r) ? r : 0
            };

            // If this is coming from a sandbox unlock, the hue and size should depend on the data value (see CrateIcon below).
            if (unlock is SandboxUnlock u)
            {
                result.hue = u.Data / 1000f;

                if (u.Data == 0)
                {
                    result.scaleX += 1f;
                    result.scaleY += 1f;
                }
            }

            return result;
        }

        private static readonly CactusProperties properties = new();

        public override ItemProperties Properties(PhysicalObject forObject)
        {
            // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
            // The Mosquitoes example demonstrates this.
            return properties;
        }
    }
}
