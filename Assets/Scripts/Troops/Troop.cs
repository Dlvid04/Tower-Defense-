using UnityEngine;


[CreateAssetMenu(fileName = "New BasicTroop", menuName = "Troops/BasicTroop")]
public class Troop : ScriptableObject
{

    public bool isHero = false;         

    [Header("Leben und Bewegung")]
    [SerializeField, Min(1)] float maxHealth = 100;
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    [SerializeField, Range(0.1f, 15)] float MovementSpeed = 1;
    public float Speed
    {
        get { return Mathf.Clamp(MovementSpeed, 0.1f, 15); }
    }

    [Header("Kampf")]
    [Min(0)] public float damage = 5;
    [Min(0.5f)] public float attackRange = 2;
    [Min(0.1f)] public float attackCooldown = 1;   

    [Header("Schildträger")]
    public bool blocksProjectiles = false;
    [Min(0)] public float shieldRadius = 1.2f;
    [Range(0, 1)] public float lightningDamageFactor = 1;  

    [Header("Held (Sturmbrecher)")]
    [Min(0)] public float auraRadius = 4;
    [Range(0, 1)] public float auraReduction = 0.2f;      
    [Min(0)] public float lightningRadius = 4;   

    
    [Header("Wetter")]
    [Min(0)] public float rainSpeedMultiplier = 0.7f;          
}