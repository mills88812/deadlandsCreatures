using Fisobs.Core;

namespace DeadlandsCreatures
{
    public class CactusAbstract : AbstractPhysicalObject
    {
        public CactusAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, CactusFisob.AbstrCactus, null, pos, ID)
        {
            scaleX = 1;
            scaleY = 1;
            saturation = 0.5f;
            hue = 1f;
            damage = 2f;
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
                realizedObject = new Cactus(this, world);
        }

        public float hue;
        public float saturation;
        public float scaleX;
        public float scaleY;
        public float damage;

        public override string ToString()
        {
            return this.SaveToString($"{hue};{saturation};{scaleX};{scaleY};{damage}");
        }
    }
}
