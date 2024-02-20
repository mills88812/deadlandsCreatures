using UnityEngine;

using Fisobs.Core;

namespace DeadlandsCreatures
{
    public class CactusIcon : Icon
    {
        public override int Data(AbstractPhysicalObject apo)
        {
            return apo is CactusAbstract cactus ? (int)(cactus.hue * 1000f) : 0;
        }

        public override Color SpriteColor(int data)
        {
            Debug.Log("" + data + ", " + (data / 1000f));
            return RWCustom.Custom.HSL2RGB(data / 1000f, 0.65f, 0.5f);
        }

        public override string SpriteName(int data)
        {
            // Fisobs autoloads the embedded resource named `icon_{Type}` automatically
            // For Crates, this is `icon_Crate`
            return "icon_Cactus";
        }
    }
}
