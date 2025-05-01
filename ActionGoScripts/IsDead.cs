//*** Katrina Cwiertniewicz
//*** CSC 372
//*** 5/4/2025

using BBUnity.Conditions;
using Gamekit3D;
using Pada1.BBCore;

[Condition("PumpkinHulk/IsDead")]
[Help("Returns true if PumpkinHulk has died.")]
public class IsDead : GOCondition
{
    public override bool Check()
    {
    return PumpkinHulkBehavior.IsDead; 
    }
}
