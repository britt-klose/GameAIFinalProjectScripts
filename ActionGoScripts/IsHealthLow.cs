using UnityEngine;
using Pada1.BBCore;
using BBUnity.Conditions;
using Gamekit3D;

[Condition("PumpkinHulk/IsHealthLow")]
[Help("Returns true if health is below the threshold.")]

// When PumpkinHulk's health is less than threshold the Check returns IsHealthLow
public class IsHealthLow : GOCondition
{
    [InParam("healthThreshold")]
    public int healthThreshold = 5;

// Check to return if PumpkinHUlk IsDead from PumpkinHulk Controller scripts
    public override bool Check()
    {
        return PumpkinHulkBehavior.IsHealthLow;
    }
}

