using System.Collections.Generic;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    public TowerAIData data;
    public bool buildStartTowerOnStart = true;

    // Nur zum Zuschauen im Inspector. Der echte Mut liegt in der Wallet.
    [SerializeField] int mut;

    Wallet wallet;
    BuildSlot[] allSlots;
    WaypointPath path;
    float incomeTimer;
    float incomeInterval = 3;

    void OnEnable()
    {
        GameEvents.UnitKilled += HandleUnitKilled;
    }

    void OnDisable()
    {
        GameEvents.UnitKilled -= HandleUnitKilled;
    }

    void Start()
    {
        allSlots = FindObjectsByType<BuildSlot>(FindObjectsSortMode.None);

        path = FindFirstObjectByType<WaypointPath>();
        if (path == null)
        {
            Debug.LogWarning("WaypointPath is nowhere to be found in the scene!");
        }

        if (TimeManager.timeManager != null)
        {
            incomeInterval = TimeManager.timeManager.mutInterval;
        }

        wallet = new Wallet(data.startMut, false);

        if (buildStartTowerOnStart)
        {
            BuildStartTower();
        }
    }

    void Update()
    {
        UpdateIncome();
        mut = wallet.MUT;

        TryBuild();
    }

    void UpdateIncome()
    {
        if (wallet.MUT >= wallet.Max)
        {
            incomeTimer = 0;
            return;
        }

        incomeTimer = incomeTimer + Time.deltaTime;
        if (incomeTimer >= incomeInterval)
        {
            incomeTimer = 0;
            wallet.ChangeMut(1);
        }
    }

    void HandleUnitKilled(TroopAgent troop)
    {
        OnTroopKilled();
    }

    [ContextMenu("Test: Enemy killed")]
    public void OnTroopKilled()
    {
        wallet.ChangeMut(data.killReward);
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

        if (wallet.MUT < towerToBuild.data.cost)
        {
            return;
        }

        BuildSlot slot = ChooseSlot(frontTroop);
        if (slot == null)
        {
            return;
        }

        slot.Build(towerToBuild.gameObject);
        wallet.ChangeMut(-towerToBuild.data.cost);
        Debug.Log("KI build " + towerToBuild.data.towerName + " on " + slot.name + " (Mut: " + wallet.MUT + ")");
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
        if (path == null)
        {
            return null;
        }

        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);

        TroopAgent best = null;
        float bestProgress = -1;

        foreach (TroopAgent troop in allTroops)
        {
            float progress = path.ProgressAt(troop.transform.position);
            if (progress > bestProgress)
            {
                best = troop;
                bestProgress = progress;
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
        float troopProgress = path.ProgressAt(frontTroop.transform.position);

        BuildSlot best = null;
        float bestProgress = Mathf.Infinity;

        foreach (BuildSlot slot in allSlots)
        {
            if (slot.IsFree() == false)
            {
                continue;
            }

            float slotProgress = path.ProgressAt(slot.transform.position);

            if (slotProgress <= troopProgress)
            {
                continue;
            }

            if (slotProgress < bestProgress)
            {
                best = slot;
                bestProgress = slotProgress;
            }
        }
        return best;
    }
}