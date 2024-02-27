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
        public static CreatureTemplate.Type Iguana;
        public static CreatureTemplate.Type BrownLizard;
        public static CreatureTemplate.Type GlowLizard;
        public static CreatureTemplate.Type CandleMouse;
        public static CreatureTemplate.Type SpinePlant;
        public static CreatureTemplate.Type SaltWorm;
        public static void RegisterValues()
        {
            Buzzard = new CreatureTemplate.Type("Buzzard", true);
            Iguana = new CreatureTemplate.Type("IguanaLizard", true);
            BrownLizard = new CreatureTemplate.Type("BrownLizard", true);
            GlowLizard = new CreatureTemplate.Type("GlowLizard", true);
        }
        public static void UnregisterValues()
        {
            if (Buzzard != null)
            {
                Buzzard.Unregister();
            }
            if (Iguana != null)
            {
                Iguana.Unregister();
            }
            if (BrownLizard != null)
            {
                BrownLizard.Unregister();
            }
            if (GlowLizard != null)
            {
                GlowLizard.Unregister();
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
