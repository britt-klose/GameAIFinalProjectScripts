//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using UnityEngine.AI;

[Action("PumpkinHulk/Chase")]
public class ChaseAction : GOAction
{
    // Animator for triggering chase animation
    [InParam("animator")]
    public Animator animator;

    [InParam("target")]
    public Transform target;  // Ellen's position

    private NavMeshAgent agent;

    [InParam("chaseSpeed")]
    private float chaseSpeed = 5f; // Adjust as needed


    // The start method of the action
    public override void OnStart()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
        base.OnStart();

        GameObject targetObj = GameObject.FindGameObjectWithTag("Player");
        if (targetObj != null)
        {
            target = targetObj.transform;
        }
        else
        {
            Debug.LogError("ChaseAction FAILED: Player is NULL!");
            return;
        }

        Debug.Log("ChaseAction started. AI is preparing to chase.");
    }
    
    // The update method of the action
    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            Debug.LogError("No target found! ChaseAction cannot proceed.");
            return TaskStatus.FAILED;
        }
        // Move PumpkinHulk towards the target
        agent.SetDestination(target.position);

        // Check if the target is within attack range
        if (Vector3.Distance(gameObject.transform.position, target.position) <= 2f)
        {
            return TaskStatus.COMPLETED;  // PumpkinHulk has reached attack range
        }

        return TaskStatus.RUNNING;  // Continue chasing
    }
}
