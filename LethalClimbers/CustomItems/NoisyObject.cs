using Unity.Netcode;
using UnityEngine;

namespace LethalClimbers.CustomItems
{
    public class NoisyObject : GrabbableObject
    {
        [Header("Audio Sources")]
        [Space(1f)]
        public AudioSource audioSource;

        public AudioSource audioSourceFar;

        [Space(3f)]
        [Header("Sound Effects")]
        [Space(1f)]
        public AudioClip[] noiseSFX;

        public AudioClip[] noiseSFXFar;

        [Space(3f)]
        [Header("Sound Settings")]
        [Space(1f)]
        public float noiseRange;

        public float maxLoudness;

        public float minLoudness;

        public float minPitch;

        public float maxPitch;

        [HideInInspector]
        private System.Random noisemakerRandom;

        public override void Start()
        {
            base.Start();

            noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);

            if (IsOwner && !audioSource.isPlaying)
            {
                // Prepare sound properties
                int randomNoisePosition = noisemakerRandom.Next(0, noiseSFX.Length);
                float volumeScale = noisemakerRandom.Next((int)(minLoudness * 100f), (int)(maxLoudness * 100f)) / 100f;
                float pitch = noisemakerRandom.Next((int)(minPitch * 100f), (int)(maxPitch * 100f)) / 100f;

                // Send sound to the network
                PlaySoundServerRpc(randomNoisePosition, volumeScale, pitch);
            }
        }

        [ServerRpc]
        public void PlaySoundServerRpc(int randomNoisePosition, float volumeScale, float pitch)
        {
            PlaySoundClientRpc(randomNoisePosition, volumeScale, pitch);
        }

        [ClientRpc]
        public void PlaySoundClientRpc(int randomNoisePosition, float volumeScale, float pitch)
        {
            PlaySoundFile(randomNoisePosition, volumeScale, pitch);
        }

        public void PlaySoundFile(int randomNoisePosition, float volumeScale, float pitch)
        {
            // Catch any doubled network sends
            if (audioSource.isPlaying)
            {
                return;
            }

            // Play close audio
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(noiseSFX[randomNoisePosition], volumeScale);

            // Play far audio
            if (audioSourceFar != null && !audioSourceFar.isPlaying)
            {
                audioSourceFar.pitch = pitch;
                audioSourceFar.PlayOneShot(noiseSFXFar[randomNoisePosition], volumeScale);
            }

            // Transmit to walkie talkies
            WalkieTalkie.TransmitOneShotAudio(audioSource, noiseSFX[randomNoisePosition], volumeScale);
            
            // Transmit to environment
            RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, volumeScale, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
        }
    }

}