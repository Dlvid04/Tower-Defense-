using System.Collections.Generic;
using UnityEngine;


public static class DamageHelper
{
    public static void DealDamage(TroopAgent troop, float damage, bool canBeBlocked = true)
    {
        if (canBeBlocked)
        {
            troop = FindShieldBlocker(troop);
        }

        damage = damage * (1 - GetAuraReduction(troop));

        Health health = troop.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        Debug.Log("Hit: " + troop.name + " gets " + damage + " DMG!");
    }

    public static void DealDamageToTower(Tower tower, float damage)
    {
        Health health = tower.GetComponent<Health>();
        if (health == null)
        {
            health = tower.gameObject.AddComponent<Health>();
            health.Init(Team.AI, tower.data.maxHealth);
        }

        bool wasAlive = health.IsDead == false;
        health.TakeDamage(damage);

        if (wasAlive && health.IsDead)
        {
            CardManager.Instance.mutWallet.ChangeMut(5);
            CardManager.Instance.UpdateMUTDislpay();
            tower.DestroyTower();
        }
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

    static TroopAgent FindShieldBlocker(TroopAgent target)
    {
        Tower shooter = FindNearestTower(target.transform.position);
        if (shooter == null)
        {
            return target;
        }

        if (shooter.data.attackType != AttackType.Single && shooter.data.attackType != AttackType.Cone)
        {
            return target;
        }

        Vector3 from = shooter.transform.position;
        from.y = 0;
        Vector3 to = target.transform.position;
        to.y = 0;

        Vector3 direction = to - from;
        float length = direction.magnitude;
        if (length < 0.01f)
        {
            return target;
        }
        direction = direction / length;

        TroopAgent best = target;
        float bestAlong = length;

        TroopAgent[] allTroops = Object.FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent other in allTroops)
        {
            if (other == target || other.troop == null || other.troop.blocksProjectiles == false)
            {
                continue;
            }

            Health otherHealth = other.GetComponent<Health>();
            if (otherHealth == null || otherHealth.IsDead)
            {
                continue;
            }

            Vector3 toOther = other.transform.position - from;
            toOther.y = 0;

            float along = Vector3.Dot(toOther, direction);            
            if (along <= 0 || along >= bestAlong)
            {
                continue;                                           
            }

            float sideways = (toOther - direction * along).magnitude;  
            if (sideways <= other.troop.shieldRadius)
            {
                best = other;
                bestAlong = along;
            }
        }

        if (best != target)
        {
            Debug.Log(best.name + " damage block for " + target.name);
        }
        return best;
    }

    static float GetAuraReduction(TroopAgent target)
    {
        float best = 0;

        TroopAgent[] allTroops = Object.FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent other in allTroops)
        {
            if (other.troop == null || other.troop.isHero == false)
            {
                continue;
            }

            Health otherHealth = other.GetComponent<Health>();
            if (otherHealth == null || otherHealth.IsDead)
            {
                continue;
            }

            Vector3 offset = other.transform.position - target.transform.position;
            offset.y = 0;
            if (offset.magnitude <= other.troop.auraRadius)
            {
                best = Mathf.Max(best, other.troop.auraReduction);
            }
        }
        return best;
    }

    public static Tower FindNearestTower(Vector3 position)
    {
        Tower[] allTowers = Object.FindObjectsByType<Tower>(FindObjectsSortMode.None);

        Tower best = null;
        float bestDistance = Mathf.Infinity;
        foreach (Tower tower in allTowers)
        {
            float distance = Vector3.Distance(position, tower.transform.position);
            if (distance < bestDistance)
            {
                best = tower;
                bestDistance = distance;
            }
        }
        return best;
    }
}