using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlandsCreatures
{
    public class DeadlandsCEnums
    {
        public static void RegisterEnums()
        {
            Type.RegisterValues();
            MaskType.RegisterValues();
        }
        public static void UnregisterEnums()
        {
            Type.UnregisterValues();
            MaskType.UnregisterValues();
        }
    }

    public class Type
    {
        public static CreatureTemplate.Type Buzzard;
        public static CreatureTemplate.Type CandleMouse;
        public static CreatureTemplate.Type SpinePlant;
        public static CreatureTemplate.Type SaltWorm;
        public static void RegisterValues()
        {
            Buzzard = new CreatureTemplate.Type("Buzzard", true);
        }
        public static void UnregisterValues()
        {
            if (Buzzard != null)
            {
                Buzzard.Unregister();
            }
        }
    }

    public class MaskType
    {
        public static VultureMask.MaskType BUZZARD;

        public static void RegisterValues()
        {
            BUZZARD = new VultureMask.MaskType("BUZZARD", true);
        }
        public static void UnregisterValues()
        {
            if (BUZZARD != null)
            {
                BUZZARD.Unregister();
            }
        }
    }
}
