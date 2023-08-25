using UnityEngine;

namespace DeadlandsCreatures;

sealed class CactusChunkAbstract : AbstractPhysicalObject
{
    public CactusChunkAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, CactusChunkFisob.CactusChunk, null, pos, ID)
    { 
        scaleX = 1;
        scaleY = 1;
        saturation = 0.5f;
        hue = 1f;
    }

    public override void Realize()
    {
        base.Realize();

        if (realizedObject == null)
            realizedObject = new CactusChunk(this, Room.realizedRoom.MiddleOfTile(pos.Tile), Vector2.zero);
    }
    
    public float hue;
    public float saturation;
    public float scaleX;
    public float scaleY;

    public override string ToString()
    {
        return "";// this.SaveToString($"");
    }
}
