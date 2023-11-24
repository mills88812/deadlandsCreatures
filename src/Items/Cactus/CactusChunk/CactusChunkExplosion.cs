using RWCustom;
using UnityEngine;

namespace DeadlandsCreatures
{
    sealed class CactusChunkExplosion
    {
        public static void CactusExplosion(Room room, WorldCoordinate pos, int projectiles)
        {
            for (int i = 0; i < projectiles; i++)
            {
                Vector2 launchDir = Custom.DegToVec(Random.value * 360);

                CactusChunkAbstract cacc = new CactusChunkAbstract(room.world, pos, room.game.GetNewID()); // Create an abstract object

                cacc.RealizeInRoom(); // Realize and place the object (add the object to the room)
                //cacc.realizedObject.firstChunk.vel += launchDir * 9f;
            }
        }
    }
}
