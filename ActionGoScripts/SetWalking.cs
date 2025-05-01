//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using UnityEngine;
using Pada1.BBCore;           // Core attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using BBUnity.Actions;        // GOAction

/// <summary>
/// A generalized action to set the Animator Controller parameter 'IsWalking' to either true or false.
/// </summary>
[Action("PumpkinHulk/SetWalkingState")]
public class SetWalkingState : GOAction
{
    // Define input parameter for the Animator
    [InParam("animator")]
    public Animator animator;

    // Define input parameter to set IsWalking to true or false
    [InParam("isWalking")]
    public bool isWalking; // This can be set to either true or false

 
    // The start method of the action
    public override void OnStart()
    {
        base.OnStart();
        // Any initialization if needed can be done here
    }

    /// <summary>
    /// Sets the IsWalking parameter in the Animator to the given value.
    /// </summary>
    /// <returns>TaskStatus.Completed if action is executed successfully</returns>
    public override TaskStatus OnUpdate()
    {
        if (animator != null)
        {
            // Set the 'IsWalking' parameter in the Animator to the value of isWalking
            animator.SetBool("IsWalking", isWalking);
            return TaskStatus.COMPLETED;  // Action has been completed
        }

        return TaskStatus.FAILED;  // Return failed if the animator reference is null
    }
}