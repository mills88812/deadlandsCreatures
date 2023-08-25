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

                cacc.realizedObject = new CactusChunk(cacc, room.MiddleOfTile(pos.Tile), launchDir + (Vector2.up)); // Realize the object (Create the PhysicalObject)

                cacc.realizedObject.PlaceInRoom(room); // ...Place the object? Not sure why this is needed, but it doesn't spawn without it
                room.abstractRoom.AddEntity(cacc); // Add the object to the room
            }
        }
    }
}
