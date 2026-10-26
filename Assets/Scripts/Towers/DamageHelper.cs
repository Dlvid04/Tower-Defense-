using System.Collections.Generic;
using UnityEngine;


public static class DamageHelper
{
    public static void DealDamage(TroopAgent troop, float damage)
    {
        // SPÄTER, wenn Dev 1 Health fertig hat:
        // Health health = troop.GetComponent<Health>();
        // if (health != null) health.TakeDamage(damage);
        Debug.Log("Treffer: " + troop.name + " bekommt " + damage + " Schaden");
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