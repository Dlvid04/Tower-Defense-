using UnityEngine;

/// <summary>
/// Basis klasse für truppen
/// </summary>
[CreateAssetMenu(fileName = "New BasicTroop", menuName = "Troops/BasicTroop")]
public class Troop : ScriptableObject
{
    [SerializeField] GameObject prefab;

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
}
