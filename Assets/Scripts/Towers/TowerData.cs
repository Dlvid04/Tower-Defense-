using UnityEngine;

public enum AttackType
{
    Single, //  Pfeil: ein Ziel
    Area,   //  Kanone: Flächenschaden
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

    [Header("Weather")]
    [Min(0)] public float fogRangeMultiplier = 0.6f;        // Nebel:       Reichweite  (0.6 = -40 %)
    [Min(0)] public float rainDamageMultiplier = 1;         // Regen:       Schaden     (Flamme: 0.5 = -50 %)
    [Min(0)] public float stormDamageMultiplier = 1;        // Gewitter:    Schaden     (Blitz: 1.5 = +50 %)
    public Troop swarmTroop;                            
    [Range(0, 1)] public float fogSwarmMissChance = 0.3f;   // Nebel: so oft verfehlt der Turm den Schwarm
}