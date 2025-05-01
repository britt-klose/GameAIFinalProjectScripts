using BBUnity.Conditions;
using Gamekit3D;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using UnityEngine;

[Condition("PumpkinHulk/IsHit")]
[Help("Returns true if the PumpkinHulk was hit.")]
public class IsHit : GOCondition
{
    public override bool Check()
    {
    return PumpkinHulkBehavior.IsHit; 
    }
}