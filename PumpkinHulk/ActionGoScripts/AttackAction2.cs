// using UnityEngine;
// using Pada1.BBCore;           // Core attributes
// using Pada1.BBCore.Tasks;     // TaskStatus
// using BBUnity.Actions;
// using UnityEngine.AI;

// /// <summary>
// /// AttackAction2 is a custom action that makes PumpkinHulk attack a target.
// /// </summary>
// [Action("PumpkinHulk/Attack2")] // This is how it will be listed in the editor
// [Help("PumpkinHulk attacks the target if within range.")]
// public class AttackAction2 : GOAction
// {
//     [InParam("Enemy")]
//     public Transform target;
//     private NavMeshAgent agent;

//     [InParam("attackRange")]
//     public float attackRange = 2f;

//     [InParam("animator")]
//     public Animator animator;

//     [InParam("attackAnimationTrigger2")]
//     public string attackAnimationTrigger;

//     private bool attackCompleted = false; // ✅ Track whether attack is completed

//     public override void OnStart()
//     {
//         agent = gameObject.GetComponent<NavMeshAgent>();

//         base.OnStart();

//         if (target == null)
//         {
//             Debug.LogWarning("Target is not assigned for PumpkinHulk's attack.");
//         }

//         attackCompleted = false; // ✅ Reset attack status at the beginning
//     }

//     public override TaskStatus OnUpdate()
//     {
//         if (target == null)
//         {
//             Debug.LogError("PumpkinHulk has no valid target—AttackAction1 failed!");
//             return TaskStatus.FAILED;
//         }

//         float distanceToTarget = Vector3.Distance(gameObject.transform.position, target.position);
//         Debug.Log($"Distance to target: {distanceToTarget}");

//         if (distanceToTarget <= attackRange && !attackCompleted)
//         {
//             Debug.Log("Within attack range—executing attack!");

//             agent.isStopped = true;
//             agent.ResetPath(); // ✅ Prevents unwanted movement

//             if (animator != null)
//             {
//                 animator.SetTrigger(attackAnimationTrigger);
//                 Debug.Log($"Attack triggered successfully with animation: {attackAnimationTrigger}");
//                 attackCompleted = true; // ✅ Mark attack as completed
//             }
//             else
//             {
//                 Debug.LogError("Animator is NULL—attack animation cannot play!");
//             }

//             return TaskStatus.RUNNING; // ✅ Allow animation to finish
//         }

//         // ✅ Check if attack animation finished before resetting status
//         if (attackCompleted && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
//         {
//             Debug.Log("Attack animation finished—resetting attack state.");
//             attackCompleted = false; // ✅ Reset for next attack cycle
//             agent.isStopped = false; // ✅ Resume movement after attack
//             return TaskStatus.COMPLETED; // ✅ Allows Behavior Tree to continue normally
//         }

//         return TaskStatus.RUNNING;
//     }
// }
