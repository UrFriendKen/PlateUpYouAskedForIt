using Kitchen;
using Kitchen.Components;
using UnityEngine;

namespace YouAskedForIt
{
    public class ExplosionView : GenericObjectView
    {
        public ParticleSystem Explosion;

        public override void Initialise()
        {
            base.Initialise();
            Explosion?.Play();
        }

        public override void Remove()
        {
            base.Remove();
        }
    }
}
