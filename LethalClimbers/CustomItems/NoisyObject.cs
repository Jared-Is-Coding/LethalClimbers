using UnityEngine;

namespace LethalClimbers.CustomItems
{
    public class NoisyObject : GrabbableObject
    {
        public AudioSource noiseAudio;

        public AudioSource noiseAudioFar;

        [Space(3f)]
        public AudioClip[] noiseSFX;

        public AudioClip[] noiseSFXFar;

        [Space(3f)]
        public float noiseRange;

        public float maxLoudness;

        public float minLoudness;

        public float minPitch;

        public float maxPitch;

        private System.Random noisemakerRandom;

        public Animator triggerAnimator;

        public override void Start()
        {
            base.Start();

            noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);

            if (!(GameNetworkManager.Instance.localPlayerController == null) && !noiseAudio.isPlaying)
            {
                int randomNoisePosition = noisemakerRandom.Next(0, noiseSFX.Length);
                float volumeScale = noisemakerRandom.Next((int)(minLoudness * 100f), (int)(maxLoudness * 100f)) / 100f;
                float pitch = noisemakerRandom.Next((int)(minPitch * 100f), (int)(maxPitch * 100f)) / 100f;

                noiseAudio.pitch = pitch;
                noiseAudio.PlayOneShot(noiseSFX[randomNoisePosition], volumeScale);

                // Debug
                BasePlugin.LogSource.LogDebug($"NoisyObject {GetInstanceID()} called noiseAudio.PlayOneShot");

                if (noiseAudioFar != null && !noiseAudioFar.isPlaying)
                {
                    noiseAudioFar.pitch = pitch;
                    noiseAudioFar.PlayOneShot(noiseSFXFar[randomNoisePosition], volumeScale);

                    // Debug
                    BasePlugin.LogSource.LogDebug($"NoisyObject {GetInstanceID()} called noiseAudioFar.PlayOneShot");
                }

                if (triggerAnimator != null)
                {
                    triggerAnimator.SetTrigger("playAnim");

                    // Debug
                    BasePlugin.LogSource.LogDebug($"NoisyObject {GetInstanceID()} called triggerAnimator.SetTrigger");
                }

                WalkieTalkie.TransmitOneShotAudio(noiseAudio, noiseSFX[randomNoisePosition], volumeScale);
                RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, volumeScale, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
            }
        }
    }

}