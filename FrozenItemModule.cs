using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using Unity;


namespace IceWind
{
    public class FrozenItemModule:ItemModule
    {
        public bool alwaysActive = true;
        public string freezeVFXName = "None";
        public bool addParticles = false;
        public bool removeIceImbuingOnUngrab = false;
        public float iceDamage = 0.1f;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<FrozenItem>();
        }

    }
}
