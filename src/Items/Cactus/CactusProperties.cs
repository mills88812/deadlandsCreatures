using Fisobs.Properties;

namespace DeadlandsCreatures
{
    public class CactusProperties : ItemProperties
    {
        public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

        public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        {
            grabability = Player.ObjectGrabability.OneHand;
        }

        public override void LethalWeapon(Scavenger scav, ref bool isLethal)
        {
            isLethal = true;
        }
    }
}
