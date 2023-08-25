using System;
using BepInEx;
using UnityEngine;
using Fisobs.Core;

namespace DeadlandsCreatures
{
    [BepInPlugin(MOD_ID, "Deadlands Creatures", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "DeadlandsCreatures";
        
        public void OnEnable()
        {
            On.Room.Loaded += Room_Loaded;
            Content.Register(new CactusFisob());
            Content.Register(new CactusChunkFisob());
            // This is needed for the cactus to work properly because of some Fisobs stuff
        }
        
        private void Room_Loaded(On.Room.orig_Loaded orig, Room self)
        {
            orig(self);
            for (int i = 0; i < self.roomSettings.placedObjects.Count; i++)
            {
                if (self.roomSettings.placedObjects[i].type == CactusFisob.Cactus)
                {
                    PlacedObject currentObject = self.roomSettings.placedObjects[i];
                    AbstractPhysicalObject cactusAbstr = new CactusAbstract(self.world, self.GetWorldCoordinate(currentObject.pos), self.game.GetNewID());
                    self.abstractRoom.AddEntity(cactusAbstr);
                    break;
                }
            }
        }
    }
}