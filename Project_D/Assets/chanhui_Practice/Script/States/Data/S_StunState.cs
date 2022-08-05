using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newStunStateData", menuName = "Data/State Data/Stun State")]
public class S_StunState : ScriptableObject
{
    public float stunTime = 1f;

    public float stunKnockbackTime = 0.2f;
    public float stunKnockbackSpeed = 10f;
    public Vector2 stunKnockbackAngle;
}
