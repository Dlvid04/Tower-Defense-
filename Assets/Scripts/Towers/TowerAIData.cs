using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CounterEntry
{
    public Troop troop;
    public Tower counterTower;
}

[CreateAssetMenu(fileName = "New TowerAIData", menuName = "Towers/TowerAIData")]
public class TowerAIData : ScriptableObject
{
    [Header("Mut")]
    [Min(0)] public int startMut = 0;
    [Min(0)] public int killReward = 2;

    [Header("Build")]
    [Min(1)] public int maxTowers = 4;
    [Min(2)] public int groupSize = 3;

    [Header("Wich Tower")]
    public Tower startTower;
    public Tower defaultTower;
    public Tower groupTower;
    public List<CounterEntry> counterTable = new List<CounterEntry>();
}