//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using Gamekit3D;
using static Gamekit3D.Damageable;

[Action("PumpkinHulk/TakeDamage")]
[Help("Handles PumpkinHulk taking damage only when hit.")]
public class TakeDamage : GOAction
{
    // Parameter for damageable script
    [InParam("damageable")]
    public Damageable damageable;
    
    // Animator for triggering hit animation
    [InParam("animator")]
    public Animator animator;

    // The trigger name for the hit animation
    [InParam("hitAnimationTrigger")]
    public string hitAnimationTrigger;

    // The start method of the action
    public override void OnStart()
    {
        base.OnStart();
        damageable = gameObject.GetComponent<Damageable>();
    }

    // The update method of the action
    public override TaskStatus OnUpdate()
    {
        if (!PumpkinHulkBehavior.IsHit)
        {
            Debug.Log("TakeDamage skipped—PumpkinHulk was not hit.");
            return TaskStatus.FAILED;
        }
    // Damageable component to determine if PumpkinHulk was damaged by Ellen
    DamageMessage msg = new DamageMessage();
    
    msg.damager = GameObject.Find("Ellen").GetComponent<PlayerController>(); 
    if (msg.damager == null)
    {
        return TaskStatus.FAILED;
    }
    animator.SetTrigger("IsHit"); 
    return TaskStatus.COMPLETED;
    }
}