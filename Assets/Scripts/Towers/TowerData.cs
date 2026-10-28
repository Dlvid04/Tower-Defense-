using UnityEngine;

public enum AttackType
{
    Single, //  Pfeil: ein Ziel
    Area,   //  Kanone: Flächenschade
    Cone,   //  Flamme: Kegel vor dem Turm
    Chain   //  Blitz: springt auf mehrere Ziele
}

[CreateAssetMenu(fileName = "New TowerData", menuName = "Towers/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("General")]
    public string towerName = "Turm";
    [Min(0)] public int cost = 3;
    [Min(1)] public float maxHealth = 300;
    public Color color = Color.white;

    [Header("Attack")]
    public AttackType attackType = AttackType.Single;
    [Min(0)] public float damage = 10;
    [Min(0.5f)] public float range = 8;
    [Min(0.1f)] public float attackPerSecond = 1;
    [Min(1)] public float projectileSpeed = 15;

    [Header("Different types of attacks")]
    [Min(0)] public float areaRadius = 2.5f;            // Kanone: Radius der Explosion
    [Range(10, 180)] public float coneAngle = 60;       // Flamme: Breite des Kegels in Grad
    [Min(0)] public int chainTargets = 3;               // Blitz: Anzahl der Ziele
}
