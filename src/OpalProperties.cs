using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fisobs.Properties;

namespace TestMod
{
    public class OpalProperties : ItemProperties
    {
        public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

        public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        {
            // The player should only be able to grab one Crate at a time

            grabability = Player.ObjectGrabability.OneHand;

        }

    }
}