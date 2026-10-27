using UnityEngine;

[CreateAssetMenu(fileName = "LightningData", menuName = "Towers/LightningData")]
public class LightningData : ScriptableObject
{
    [Header("Impact")]
    [Min(0)] public float damage = 30;
    [Min(0.1f)] public float strikeInterval = 4;
    [Min(0)] public float searchRadius = 5;

    [Header("Placeholder")]
    [Min(0)] public float visualDuration = 0.3f;
}
