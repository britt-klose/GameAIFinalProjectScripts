//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using Gamekit3D;
using System;

[Action("PumpkinHulk/DeathAction")]
[Help("Handles PumpkinHulk death transition.")]
public class DeathAction : GOAction
{
    // Parameter for damageable script
    [InParam("damageable")]
    public Damageable damageable;

    // Animator for triggering death animation
    [InParam("animator")]
    public Animator animator;

    // Particle System for smokeEffect
    public ParticleSystem smokeEffect;

    // Boolean value for checking if Death Action was triggered
    private bool deathTriggered = false;

    
    // Initialize the node if necessary
    public override void OnStart()
    {
        base.OnStart();

        if (animator == null)
        {
            animator = gameObject.GetComponent<Animator>();
        }

        if (smokeEffect == null)
        {
            smokeEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        }
        
        Debug.Log("DeathAction initialized.");
    }

    public override TaskStatus OnUpdate()
    {
        if (!PumpkinHulkBehavior.IsDead)
        {
            return TaskStatus.FAILED;
        }
        if (damageable.currentHitPoints > 0)
        {
            return TaskStatus.RUNNING; // Continue executing behavior
        }

        Debug.Log("PumpkinHulk is dead. Executing death sequence...");


        if (!deathTriggered)
        {
            animator.Play("Dead", 0, 0f); 
            smokeEffect.Play();
            deathTriggered = true;
        }

        if (deathTriggered == true)
        {
            Debug.Log("PumpkinHulk is now deactivated.");
            return TaskStatus.COMPLETED;
        }

        return TaskStatus.RUNNING;
    }
}
