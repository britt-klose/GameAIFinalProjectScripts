//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using Pada1.BBCore;
using BBUnity.Actions;
using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore.Tasks;

[Action("PumpkinHulk/WanderRandomly")]
public class WanderRandomlyAction : GOAction
{
    [InParam("pointA")]
    public Transform pointA;  // Starting point A

    [InParam("pointB1")]
    public Transform pointB1;  // Random target point B1

    [InParam("pointB2")]
    public Transform pointB2;  // Random target point B2
    
    [InParam("Speed")]
    public int speed;

    [InParam("target")] 
    public Transform target; // Target to chase if detected

    private NavMeshAgent agent;
    private Animator animator;
    private bool movingToA = false;  // Start by moving to a random point, not A
    private Transform targetPoint;
    private float timeToWander = 0f;
    private float maxTimeToWander = 30f;
    private float chaseTriggerDistance = 20f;  // Distance to start chasing


    // The start method of the action
    public override void OnStart()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = speed;   // Speed up PumpkinHulk and enable its walking animation
        
        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsWalking", true);

        // Start by randomly choosing either B1 or B2
        targetPoint = (Random.Range(0, 2) == 0) ? pointB1 : pointB2;
        agent.SetDestination(targetPoint.position);
    }

      // The update method of the action
    public override TaskStatus OnUpdate()
    {
        // Debug log to check if Ellen is detected and the distance to her
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(gameObject.transform.position, target.position);
            Debug.Log($"Ellen detected: {target != null} at distance: {distanceToTarget}");
        }

        // Check if target is within chase range and transition out of wandering
        if (target != null && Vector3.Distance(gameObject.transform.position, target.position) <= chaseTriggerDistance)
        {
            Debug.Log("Target detected! Exiting wander and switching to chase mode.");
            
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsChasing", true);
            
            return TaskStatus.COMPLETED;  // Exit wandering and transition to chasing
        }

        // Increment time every frame while the agent is moving
        if (agent.remainingDistance > 0.5f && !agent.pathPending)
        {
            // Accumulate time spent moving
            timeToWander += Time.deltaTime;
            
            if (timeToWander >= maxTimeToWander)
            {
                return TaskStatus.COMPLETED; // Stop wandering after max time
            }
        }
        // Check if we've reached the current destination (either one of the random points or Point A)
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            // If we are at one of the random points (B1 or B2), move back to A
            if (!movingToA)
            {
                agent.SetDestination(pointA.position);  // Move back to A
                targetPoint = pointA;  // Set target to A
                movingToA = true;  // After reaching A, we will move randomly to B1 or B2 next
            }
            else
            {
                // If we are at A, randomly choose B1 or B2
                targetPoint = (Random.Range(0, 2) == 0) ? pointB1 : pointB2;
                agent.SetDestination(targetPoint.position);  // Move to the selected random point
                movingToA = false;  // After reaching B1 or B2, we will move back to A next
            }
        }

        return TaskStatus.RUNNING;  // Keep the task running to repeat the cycle
    }
}