//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using UnityEngine;
using Pada1.BBCore;           // Core attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using BBUnity.Actions;
using UnityEngine.AI;        

/// <summary>
/// AttackAction is a custom action that makes PumpkinHulk attack a target.
/// </summary>
[Action("PumpkinHulk/Attack")] // This is how it will be listed in the editor
[Help("Chomper attacks the target if within range.")]
public class AttackAction : GOAction
{
    // The target that PumpkinHulk is attacking
    [InParam("Enemy")]
    public Transform target;
    private NavMeshAgent agent;

    // The distance at which PumpkinHulk can attack
    [InParam("attackRange")]
    public float attackRange = 10f;

    // Animator for triggering attack animation
    [InParam("animator")]
    public Animator animator;

    // The trigger name for the attack animation
    [InParam("attackAnimationTrigger")]
    public string attackAnimationTrigger;

    // The start method of the action
    public override void OnStart()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        base.OnStart();
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned for PumpkinHulk's attack.");
        }
    }

    // The update method of the action
    public override TaskStatus OnUpdate()
    {
        //Debug.Log("Executing Attack action...");

        // If there is no target, fail the action
        if (target == null)
        {
            return TaskStatus.FAILED;
        }

        // Access the PumpkinHulk transform explicitly using gameObject.transform
        float distanceToTarget = Vector3.Distance(gameObject.transform.position, target.position);

        // Check if PumpkinHulk is within attack range of the target
        if (distanceToTarget <= attackRange)
        {
            // Trigger the attack animation
            if (animator != null)
            {
            // Pick a random attack trigger (Attack1, Attack2, or Attack3)
            int randomTrigger = Random.Range(1, 4);
            string triggerName = "Attack" + randomTrigger; // Convert int to string for trigger

            // Reset all attack triggers before setting a new one
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");

            animator.SetTrigger(triggerName); // No ResetTrigger() used before
            //Debug.Log("Triggered attack: " + triggerName);
            }

            // Attack completed successfully
            return TaskStatus.COMPLETED;
        }
        else
        {
            // If the target is out of range, fail the action
            return TaskStatus.FAILED;
        }
    }
}