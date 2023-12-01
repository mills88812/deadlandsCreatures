using Fisobs.Core;

using BepInEx;
using BepInEx.Logging;
using System.Security.Permissions;
using DeadlandsCreatures.Hooks;
using DeadlandsCreatures.Features.Opal;

// IMPORTANT
// This requires Fisobs to work!
// Big thx to Dual-Iron (on github) for help with Fisobs!
// This code was based off of Dual-Iron's Centishield as practice, I didn't make parts of this! (Probably add more details on that later)

#pragma warning disable CS0618 // Do not remove the following line.
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DeadlandsCreatures
{

    // See https://rainworldmodding.miraheze.org/wiki/Downpour_Reference/Mod_Directories

    [BepInPlugin("DeadLandsCreautres", "DeadLands Creatures", "0.1.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger { get; private set; } = null!;

        public void OnEnable()
        {
            Logger = base.Logger;

            DeadlandsCEnums.RegisterEnums();

            CreatureHooks.Apply();

            Content.Register(new OpalCritob());

            On.Room.Loaded += Room_Loaded;
        }

        private void Room_Loaded(On.Room.orig_Loaded orig, Room self)
        {
            orig(self);
            for (int i = 0; i < self.roomSettings.placedObjects.Count; i++)
            {
                if (self.roomSettings.placedObjects[i].type == OpalCritob.Opal)
                {
                    PlacedObject currentObject = self.roomSettings.placedObjects[i];
                    AbstractPhysicalObject cactusAbstr = new OpalAbstract(self.world, self.GetWorldCoordinate(currentObject.pos), self.game.GetNewID());
                    self.abstractRoom.AddEntity(cactusAbstr);
                    break;
                }
            }
            //AbstractCreature creature = new AbstractCreature(self.game.world, StaticWorld.GetCreatureTemplate(Type.Buzzard), null, self.GetWorldCoordinate(self.RandomPos()), self.game.GetNewID());
            //creature.RealizeInRoom();
            //Debug.Log("Spawned " + Type.Buzzard);
        }
    }
}