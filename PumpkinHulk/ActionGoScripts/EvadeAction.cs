//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using BBUnity.Actions;
using UnityEngine.AI;

[Action("PumpkinHulk/Evade")]
[Help("PumpkinHulk evades when health is low.")]
public class EvadeAction : GOAction
{

    // Animator for triggering evade animation
    [InParam("animator")]
    public Animator animator;

    // PumpkinHulk hidingSpot when Evading
    [InParam("hidingSpot")]
    public Transform hidingSpot;
    
    // Speed of Evading
    [InParam("evadeSpeed")]
    private float evadeSpeed = 3f; // Adjust as needed

    private NavMeshAgent agent;

    public override void OnStart()
    {
        base.OnStart();

        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = evadeSpeed;

        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;

        if (animator == null)
        {
            animator = gameObject.GetComponent<Animator>();  
        }
        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        animator.SetBool("IsEvading", true);
        Debug.Log("PumpkinHulk started evading due to low health!");
    }

    public override TaskStatus OnUpdate()
    {
    
        agent.SetDestination(hidingSpot.position);

        if(Vector3.Distance(gameObject.transform.position, hidingSpot.position) < 1.0f)
        {
            animator.SetBool("IsEvading", false);
            animator.SetBool("IsWalking", false);

            GameObject Ellen = GameObject.FindWithTag("Player");
            if(Ellen != null)
            {
                Vector3 direction = (Ellen.transform.position - gameObject.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            return TaskStatus.COMPLETED;
        }

        return TaskStatus.RUNNING;
    }
}
