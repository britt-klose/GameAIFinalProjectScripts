//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using System.Collections;
using Gamekit3D.Message;
using UnityEngine;

namespace Gamekit3D
{
    [DefaultExecutionOrder(100)]
    public class PumpkinHulkBehavior : MonoBehaviour, IMessageReceiver
    {
        // Boolean parameters to determine if IsHealthLow, IsDead, IsHit are true
        public static bool IsHealthLow = false;
        public static bool IsDead = false;
        public static bool IsHit = false;
        public int healthThreshold = 5;
        
        // Parameter for damageable script
        public Damageable damageable;
    
        protected void OnEnable()
        {
            damageable = GetComponent<Damageable>();
        }

        public void OnReceiveMessage(Message.MessageType type, object sender, object msg)
        {
            Debug.Log("PumpkinHulk received a message: " + type);

            switch (type)
            {
                case Message.MessageType.DEAD:
                    IsDead = true;
                    break;
                case Message.MessageType.DAMAGED:
                    ApplyDamage((Damageable.DamageMessage)msg);
                    break;
                default:
                    break;
            }
        }

        public void ApplyDamage(Damageable.DamageMessage msg)
        {
            if (msg.damager.name == "Staff")
                CameraShake.Shake(0.06f, 0.1f);

                IsHit = true;

                IsHealthLow = damageable.currentHitPoints < healthThreshold;
                IsDead = damageable.currentHitPoints <= 0;

                if (damageable.currentHitPoints <= 0) // If health reaches 0, trigger death
                {
                    IsDead = true;
                }
                StartCoroutine(ResetHit()); 
        }

        private IEnumerator ResetHit()
        {
            yield return new WaitForSeconds(0.5f); // Adjust delay if needed
            IsHit = false; // Reset hit flag
        }
    }
}
