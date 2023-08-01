using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fisobs.Core;

namespace TestMod
{
    public class OpalAbstract : AbstractPhysicalObject
    {
        public OpalAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, OpalCritob.AbstrOpal, null, pos, ID)
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
                realizedObject = new Opal(this);
        }

        public float hue;
        public float saturation;
        public float scaleX;
        public float scaleY;

        public override string ToString()
        {
            return this.SaveToString($"{hue};{saturation};{scaleX};{scaleY}");
        }
    }
}