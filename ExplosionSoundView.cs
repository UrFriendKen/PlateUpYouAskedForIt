using Kitchen;
using Kitchen.Components;
using UnityEngine;

namespace YouAskedForIt
{
    public class ExplosionSoundView : GenericObjectView
    {
        public SoundSource SoundSource;
        public AudioClip AudioClip;

        public override void Initialise()
        {
            base.Initialise();
            if (SoundSource != null && AudioClip != null)
            {
                SoundSource.VolumeMultiplier = Main.PrefManager?.Get<float>(Main.SOUND_EFFECTS_EXPLOSION_VOLUME_ID) ?? 0.5f;
                SoundSource?.Configure(SoundCategory.Effects, AudioClip);
                SoundSource?.Play();
            }
        }

        public override void Remove()
        {
            base.Remove();
        }
    }
}
