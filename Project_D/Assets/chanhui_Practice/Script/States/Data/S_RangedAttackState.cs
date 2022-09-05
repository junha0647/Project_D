using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRangedAttackStateStateData", menuName = "Data/State Data/Ranged Attack State")]
public class S_RangedAttackState : ScriptableObject
{
    public GameObject projectile;
    public float projectileDamage = 10f;
    public float projectileSpeed = 12f;
    public float projectileTravelDistance;
}
