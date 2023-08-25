using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Sandbox;
using Fisobs.Properties;

namespace DeadlandsCreatures;

sealed class CactusChunkFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType CactusChunk = new("CactusChunk", true);

    public CactusChunkFisob() : base(CactusChunk)
    {

    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        // Centi shield data is just floats separated by ; characters.
        string[] p = entitySaveData.CustomData.Split(';');

        if (p.Length < 4)
        {
            p = new string[4];
        }
        
        var result = new CactusChunkAbstract(world, entitySaveData.Pos, entitySaveData.ID)
        {
            hue = float.TryParse(p[0], out var h) ? h : 0,
            saturation = float.TryParse(p[1], out var s) ? s : 1,
            scaleX = float.TryParse(p[2], out var x) ? x : 1,
            scaleY = float.TryParse(p[3], out var y) ? y : 1,
        };
            
        return result;
    }


    private static readonly CactusChunkProperties properties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
        // The Mosquitoes example demonstrates this.
        return properties;
    }

    /////////////////////////////////////
    // Properties
    /////////////////////////////////////

    sealed class CactusChunkProperties : ItemProperties
    {
        public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        {
            grabability = Player.ObjectGrabability.OneHand;
        }
    }
}
