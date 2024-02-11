using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace LethalClimbers.CustomItems
{
    public class LcWeaponObject : GrabbableObject
    {
        [Space(3f)]
        [Header("Weapon Settings")]
        [Space(1f)]

        public int weaponHitForce = 1;

        [HideInInspector]
        public bool reelingUp;

        [HideInInspector]
        public bool isHoldingButton;

        [HideInInspector]
        private RaycastHit rayHit;

        [HideInInspector]
        private Coroutine reelingUpCoroutine;

        [HideInInspector]
        private RaycastHit[] objectsHitByWeapon;

        [HideInInspector]
        private List<RaycastHit> objectsHitByWeaponList = new List<RaycastHit>();

        [HideInInspector]
        private PlayerControllerB previousPlayerHeldBy;

        private readonly int weaponMask = 11012424;

        [Space(3f)]
        [Header("Sound Settings")]
        [Space(1f)]

        public AudioSource audioSource;

        [Space(3f)]
        [Header("Sound Effects")]
        [Space(1f)]

        public AudioClip reelUpSfx;

        public AudioClip swingSfx;

        public AudioClip[] hitSfx;

        public override void Start()
        {
            base.Start();

			// Debug logging
			BasePlugin.LogSource.LogDebug($"{GetInstanceID()} Start()");
		}

		public override void DiscardItem()
		{
			// Debug logging
			BasePlugin.LogSource.LogDebug($"{GetInstanceID()} DiscardItem()");

			if (playerHeldBy != null)
			{
				playerHeldBy.activatingItem = false;
			}

			base.DiscardItem();
		}

		public override void ItemActivate(bool used, bool buttonDown = true)
		{
			// Debug logging
			BasePlugin.LogSource.LogDebug($"{GetInstanceID()} ItemActivate()");

			if (playerHeldBy == null)
			{
				return;
			}

			isHoldingButton = buttonDown;
			
			// Debug logging
			BasePlugin.LogSource.LogDebug($"Is player pressing down button?: {buttonDown}");
			BasePlugin.LogSource.LogDebug("PLAYER ACTIVATED ITEM TO HIT WITH WEAPON. Who sent this log: " + GameNetworkManager.Instance.localPlayerController.gameObject.name);
			
			if (!reelingUp && buttonDown)
			{
				reelingUp = true;
				previousPlayerHeldBy = playerHeldBy;

				// Debug logging
				BasePlugin.LogSource.LogDebug($"Set previousPlayerHeldBy: {previousPlayerHeldBy}");
				
				if (reelingUpCoroutine != null)
				{
					StopCoroutine(reelingUpCoroutine);
				}

				reelingUpCoroutine = StartCoroutine(reelUpWeapon());
			}
		}

		private IEnumerator reelUpWeapon()
		{
			playerHeldBy.activatingItem = true;
			playerHeldBy.twoHanded = true;
			playerHeldBy.playerBodyAnimator.ResetTrigger("weaponHit");
			playerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: true);
			audioSource.PlayOneShot(reelUpSfx);
			ReelUpSFXServerRpc();

			yield return new WaitForSeconds(0.35f);
			yield return new WaitUntil(() => !isHoldingButton || !isHeld);
			SwingWeapon(!isHeld);
			
			yield return new WaitForSeconds(0.13f);
			yield return new WaitForEndOfFrame();
			HitWeapon(!isHeld);
			
			yield return new WaitForSeconds(0.3f);
			reelingUp = false;
			reelingUpCoroutine = null;
		}

		[ServerRpc]
		public void ReelUpSFXServerRpc()
		{
			ReelUpSFXClientRpc();
		}

		[ClientRpc]
		public void ReelUpSFXClientRpc()
		{
			audioSource.PlayOneShot(reelUpSfx);
		}

		public void SwingWeapon(bool cancel = false)
		{
			previousPlayerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: false);
			if (!cancel)
			{
				audioSource.PlayOneShot(swingSfx);
				previousPlayerHeldBy.UpdateSpecialAnimationValue(specialAnimation: true, (short)previousPlayerHeldBy.transform.localEulerAngles.y, 0.4f);
			}
		}

		public void HitWeapon(bool cancel = false)
		{
			if (previousPlayerHeldBy == null)
			{
				BasePlugin.LogSource.LogDebug("Previousplayerheldby is null on this client when HitWeapon is called.");
				return;
			}

			previousPlayerHeldBy.activatingItem = false;
			bool flag = false;
			bool flag2 = false;
			int num = -1;

			if (!cancel)
			{
				previousPlayerHeldBy.twoHanded = false;
				objectsHitByWeapon = Physics.SphereCastAll(previousPlayerHeldBy.gameplayCamera.transform.position + previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f, 0.8f, previousPlayerHeldBy.gameplayCamera.transform.forward, 1.5f, weaponMask, QueryTriggerInteraction.Collide);
				objectsHitByWeaponList = objectsHitByWeapon.OrderBy((RaycastHit x) => x.distance).ToList();

				for (int i = 0; i < objectsHitByWeaponList.Count; i++)
				{
					IHittable component;
					RaycastHit hitInfo;

					if (objectsHitByWeaponList[i].transform.gameObject.layer == 8 || objectsHitByWeaponList[i].transform.gameObject.layer == 11)
					{
						flag = true;
						string text = objectsHitByWeaponList[i].collider.gameObject.tag;
						for (int j = 0; j < StartOfRound.Instance.footstepSurfaces.Length; j++)
						{
							if (StartOfRound.Instance.footstepSurfaces[j].surfaceTag == text)
							{
								num = j;
								break;
							}
						}
					}

					else if (objectsHitByWeaponList[i].transform.TryGetComponent(out component) && !(objectsHitByWeaponList[i].transform == previousPlayerHeldBy.transform) && (objectsHitByWeaponList[i].point == Vector3.zero || !Physics.Linecast(previousPlayerHeldBy.gameplayCamera.transform.position, objectsHitByWeaponList[i].point, out hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault)))
					{
						flag = true;
						Vector3 forward = previousPlayerHeldBy.gameplayCamera.transform.forward;
						Debug.DrawRay(objectsHitByWeaponList[i].point, Vector3.up * 0.25f, Color.green, 5f);

						try
						{
							component.Hit(weaponHitForce, forward, previousPlayerHeldBy, playHitSFX: true);
							flag2 = true;
						}

						catch (Exception arg)
						{
							BasePlugin.LogSource.LogDebug($"Exception caught when hitting object with weapon from player #{previousPlayerHeldBy.playerClientId}: {arg}");
						}
					}
				}
			}

			if (flag)
			{
				RoundManager.PlayRandomClip(audioSource, hitSfx);
                FindObjectOfType<RoundManager>().PlayAudibleNoise(base.transform.position, 17f, 0.8f);

				if (!flag2 && num != -1)
				{
					// Play close audio
					audioSource.PlayOneShot(StartOfRound.Instance.footstepSurfaces[num].hitSurfaceSFX);

					// Transmit to walkie talkies
					WalkieTalkie.TransmitOneShotAudio(audioSource, StartOfRound.Instance.footstepSurfaces[num].hitSurfaceSFX);
				}

				// Animate
				playerHeldBy.playerBodyAnimator.SetTrigger("weaponHit");

				// Check for sounds
				HitWeaponServerRpc(num);
			}
		}

		[ServerRpc]
		public void HitWeaponServerRpc(int hitSurfaceID)
		{
			HitWeaponClientRpc(hitSurfaceID);
		}

		[ClientRpc]
		public void HitWeaponClientRpc(int hitSurfaceID)
		{
			// Transmit to environment
			RoundManager.PlayRandomClip(audioSource, hitSfx);

			if (hitSurfaceID != -1)
			{
				HitSurfaceWithWeapon(hitSurfaceID);
			}
		}

		private void HitSurfaceWithWeapon(int hitSurfaceID)
		{
			if (!IsOwner)
			{
				// Play close audio
				audioSource.PlayOneShot(StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
			}

			// Transmit to walkie talkies
			WalkieTalkie.TransmitOneShotAudio(audioSource, StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
		}

	}
}