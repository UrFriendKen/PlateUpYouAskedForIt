using Kitchen;
using UnityEngine;

namespace YouAskedForIt
{
    public class ParticleEmitterView : GenericObjectView
    {
        public ParticleSystem Emitter;

        public override void Initialise()
        {
            base.Initialise();
            Emitter?.Play();
        }

        public override void Remove()
        {
            base.Remove();
        }
    }
}
