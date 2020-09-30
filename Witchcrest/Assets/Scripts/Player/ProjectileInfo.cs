using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Projectile"), Serializable]
public class ProjectileInfo : ScriptableObject
{
    public int damage;
    public float speed=20f;
    public float lifeTime = 2f;
    public bool useGravity = false;
    [Header("Bounce")]
    public bool canBounce = false;
    
    
}
