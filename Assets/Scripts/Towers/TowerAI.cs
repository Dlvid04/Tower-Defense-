using System.Collections.Generic;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    public TowerAIData data;
    public bool buildStartTowerOnStart = true;

    [SerializeField] int mut;

    BuildSlot[] allSlots;
    Transform laneEnd;
    float incomeTimer;   

    void Start()
    {
        allSlots = FindObjectsByType<BuildSlot>(FindObjectsSortMode.None);

        LaneEnd end = FindFirstObjectByType<LaneEnd>();
        if (end != null)
        {
            laneEnd = end.transform;
        }
        else
        {
            Debug.LogWarning("LaneEnd is nowhere to be found in the scene!");
        }

        mut = data.startMut;

        if (buildStartTowerOnStart)
        {
            BuildStartTower();
        }
    }

    void Update()
    {
        incomeTimer = incomeTimer + Time.deltaTime;
        if (incomeTimer >= data.incomeInterval)
        {
            incomeTimer = 0;
            AddMut(data.incomeAmount);
        }

        TryBuild();
    }

    public void AddMut(int amount)
    {
        mut = Mathf.Min(mut + amount, data.maxMut);
    }

    [ContextMenu("Test: Enemy killed")]
    public void OnTroopKilled()
    {
        AddMut(data.killReward);
    }

    [ContextMenu("Test: Build a Start Tower")]
    public void BuildStartTower()
    {
        List<BuildSlot> freeSlots = GetFreeSlots();
        if (freeSlots.Count == 0)
        {
            return;
        }

        BuildSlot slot = freeSlots[Random.Range(0, freeSlots.Count)];
        slot.Build(data.startTower.gameObject);
        Debug.Log("KI: Start Tower! " + data.startTower.data.towerName + " on " + slot.name);
    }

    void TryBuild()
    {
        if (CountTowers() >= data.maxTowers)
        {
            return;
        }

        TroopAgent frontTroop = FindFrontTroop();
        if (frontTroop == null)
        {
            return;
        }

        Tower towerToBuild = ChooseTower(frontTroop);

        if (mut < towerToBuild.data.cost)
        {
            return;
        }

        BuildSlot slot = ChooseSlot(frontTroop);
        if (slot == null)
        {
            return;
        }

        slot.Build(towerToBuild.gameObject);
        mut = mut - towerToBuild.data.cost;
        Debug.Log("KI baut " + towerToBuild.data.towerName + " auf " + slot.name + " (Mut übrig: " + mut + ")");
    }

    int CountTowers()
    {
        int count = 0;
        foreach (BuildSlot slot in allSlots)
        {
            if (slot.IsFree() == false)
            {
                count = count + 1;
            }
        }
        return count;
    }

    List<BuildSlot> GetFreeSlots()
    {
        List<BuildSlot> freeSlots = new List<BuildSlot>();
        foreach (BuildSlot slot in allSlots)
        {
            if (slot.IsFree())
            {
                freeSlots.Add(slot);
            }
        }
        return freeSlots;
    }

    TroopAgent FindFrontTroop()
    {
        if (laneEnd == null)
        {
            return null;
        }

        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);

        TroopAgent best = null;
        float bestDistanceToEnd = Mathf.Infinity;

        foreach (TroopAgent troop in allTroops)
        {
            float distanceToEnd = Vector3.Distance(troop.transform.position, laneEnd.position);
            if (distanceToEnd < bestDistanceToEnd)
            {
                best = troop;
                bestDistanceToEnd = distanceToEnd;
            }
        }

        return best;
    }

    Tower ChooseTower(TroopAgent frontTroop)
    {
        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        if (allTroops.Length >= data.groupSize)
        {
            return data.groupTower;
        }

        foreach (CounterEntry entry in data.counterTable)
        {
            if (entry.troop == frontTroop.troop)
            {
                return entry.counterTower;
            }
        }
        return data.defaultTower;
    }


    BuildSlot ChooseSlot(TroopAgent frontTroop)
    {
        float troopDistanceToEnd = Vector3.Distance(frontTroop.transform.position, laneEnd.position);

        BuildSlot best = null;
        float bestDistanceToEnd = -1;

        foreach (BuildSlot slot in allSlots)
        {
            if (slot.IsFree() == false)
            {
                continue;
            }

            float slotDistanceToEnd = Vector3.Distance(slot.transform.position, laneEnd.position);

            if (slotDistanceToEnd >= troopDistanceToEnd)
            {
                continue;
            }

            if (slotDistanceToEnd > bestDistanceToEnd)
            {
                best = slot;
                bestDistanceToEnd = slotDistanceToEnd;
            }
        }
        return best;
    }
}