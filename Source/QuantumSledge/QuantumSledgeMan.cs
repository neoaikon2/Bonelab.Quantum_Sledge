#if !UNITY_EDITOR
using MelonLoader;
#endif
using UnityEngine;
using SLZ.Interaction;
using System;

namespace QuantumSledge
{
	[RegisterTypeInIl2Cpp] // Required to utilize the IL2CPP constructor style below
	public class QuantumSledgeMan : MonoBehaviour
	{
		bool lastTriggerState = false;
		float charge = 0;
		float chargeRate = .5f;
		float decayRate = .98f;		

		AudioSource chargeSfx = null;
		MeshRenderer sledgeMR = null;

#if !UNITY_EDITOR
		// If this isn't working on a new project make sure to have the [RegisterTypeInIl2Cpp] at the top
		public QuantumSledgeMan(IntPtr intPtr) : base(intPtr) {	}
#endif

		public void Start()
		{
			chargeSfx = GetComponent<AudioSource>();
			sledgeMR = GetComponentInChildren<MeshRenderer>();			
		}

		public void Stow()
		{
#if DEBUG
			DebugMsg("Stowed...");
#endif
		}

		public void Unstow()
		{
#if DEBUG
			DebugMsg("Unstowed...");
#endif
		}

		private void FixedUpdate()
		{
			CylinderGrip cg = GetComponentInChildren<CylinderGrip>();

			// Check if the trigger on either hand is being pressed
			bool triggerState = false;
			foreach(Hand h in cg.attachedHands)
			{
				if (h.GetIndexButton())
				{					
					triggerState = true;
					break;
				}
			}
			
			if (triggerState)
			{
				charge = charge <= 100 ? charge + chargeRate : 100;
				chargeSfx.Play();
			}
			else
			{
#if !UNITY_EDITOR
				UnhollowerBaseLib.Il2CppReferenceArray<Collider> nearbyEnemies = Physics.OverlapSphere(transform.position, 3, 0x01 << LayerMask.NameToLayer("EnemyColliders"));
#if DEBUG
				DebugHeartbeat(Time.deltaTime);
#endif
				foreach (Collider c in nearbyEnemies)
				{
					c.attachedRigidbody.AddExplosionForce(charge * 100.0f, transform.position, 3.0f, charge * 100.0f / 3.0f);
				}
#endif
				charge = 0;
				chargeSfx.Stop();
			}

			chargeSfx.volume = (charge / 100) * 0.45f;
			sledgeMR.material.SetColor("_EmissionColor", new Color(charge / 100, charge / 100, charge / 100));
		}

		private float heartbeat_t = 0;
		private void DebugHeartbeat(float dt)
		{
			heartbeat_t += dt;
			if(heartbeat_t >= 3)
			{
				
				heartbeat_t = 0;
				DebugMsg("Charge Level: " + charge);
			}
		}

		private void DebugMsg(string msg)
		{
#if UNITY_EDITOR
			Debug.Log(msg);
#else
			MelonLogger.Msg("[attached to " + gameObject.name + "] " + msg);
#endif
		}
	}
}
