using System.Collections.Generic;
using UnityEngine;


public static class DamageHelper
{
    public static void DealDamage(TroopAgent troop, float damage)
    {
        // SPÄTER, wenn Dev 1 Health fertig hat:
        // Health health = troop.GetComponent<Health>();
        // if (health != null) health.TakeDamage(damage);
        Debug.Log("Hit: " + troop.name + " gets " + damage + " DMG!");
    }

    public static void DealDamageToTower(Tower tower, float damage)
    {
        // LATER: when Dev 1 finished Health
        // Health health = tower.GetComponent<Health>();
        // if (health != null) health.TakeDamage(damage);
        Debug.Log("Hit: " + tower.data.towerName + " gets " + damage + " DMG!");
    }

    public static List<TroopAgent> GetTroopsInRadius(Vector3 center, float radius)
    {
        List<TroopAgent> result = new List<TroopAgent>();
        TroopAgent[] allTroops = Object.FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);

        foreach (TroopAgent troop in allTroops)
        {
            if (Vector3.Distance(center, troop.transform.position) <= radius)
            {
                result.Add(troop);
            }
        }
        return result;
    }
}