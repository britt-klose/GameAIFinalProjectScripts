using Pada1.BBCore;
using BBUnity.Actions;
using UnityEngine;
using UnityEngine.AI;
using Pada1.BBCore.Tasks;

[Action("PumpkinHulk/WanderTimed")]
public class WanderTimed : GOAction
{
    [InParam("pointA")]
    public Transform pointA;
    [InParam("pointB")]
    public Transform pointB;
    [InParam("Speed")]
    public int speed;
    
    private NavMeshAgent agent;
    private Animator animator;
    private bool movingToB = true; // Flag to toggle between A and B

    private float timeToWander = 0f;
    private float maxTimeToWander = 30f;

    public override void OnStart()
    {
        Debug.Log("Executing WanderTimed action...");

        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = speed;  // Speed up Chomper
        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsWalking", true);

        // Initially set the destination to point A
        agent.SetDestination(pointA.position);
    }

    public override TaskStatus OnUpdate()
    {
        // Increment time every frame while the agent is moving
        if (agent.remainingDistance > 0.5f && !agent.pathPending)
        {
            // Accumulate time spent moving
            timeToWander += Time.deltaTime;
            
            // Debug log the time spent
            Debug.Log("Current Time: " + timeToWander);

            if (timeToWander >= maxTimeToWander)
            {
            
                return TaskStatus.COMPLETED; // Stop wandering after max time
            }
        }

        // If the agent has reached the destination, toggle destinations
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            if (movingToB)
            {
                agent.SetDestination(pointB.position);
            }
            else
            {
                agent.SetDestination(pointA.position);
            }
            
            movingToB = !movingToB;
        }

        return TaskStatus.RUNNING; // Keep running until the time is up
    }
}