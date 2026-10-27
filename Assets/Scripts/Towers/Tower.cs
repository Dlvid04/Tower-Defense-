using System.Collections.Generic;
using UnityEngine;


public class Tower : MonoBehaviour
{

    public TowerData data;
    public Transform firePoint;
    public Projectile projectilePrefab;

    TroopAgent currentTarget; 
    Transform laneEnd;         
    float attackTimer;

    public static event System.Action<Tower> TowerDestroyed;

    void Start()
    {
        Renderer visualRenderer = GetComponentInChildren<Renderer>();
        if (visualRenderer != null)
        {
            visualRenderer.material.color = data.color;
        }

        LaneEnd end = FindFirstObjectByType<LaneEnd>();
        if (end != null)
        {
            laneEnd = end.transform;
        }
        else
        {
            Debug.LogWarning("Kein LaneEnd in der Szene gefunden!");
        }
    }

    void Update()
    {
        currentTarget = FindTarget();

        if (currentTarget == null)
        {
            return;
        }

        LookAtTarget();

        attackTimer = attackTimer - Time.deltaTime;
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = 1f / data.attacksPerSecond;
        }
    }

    TroopAgent FindTarget()
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
            float distanceToTower = Vector3.Distance(transform.position, troop.transform.position);
            if (distanceToTower > data.range)
            {
                continue;
            }

            float distanceToEnd = Vector3.Distance(troop.transform.position, laneEnd.position);
            if (distanceToEnd < bestDistanceToEnd)
            {
                best = troop;
                bestDistanceToEnd = distanceToEnd;
            }
        }

        return best;
    }

    void LookAtTarget()
    {
        Vector3 direction = currentTarget.transform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Attack()
    {
        switch (data.attackType)
        {
            case AttackType.Single:
                ShootArrow();
                break;
            case AttackType.Area:
                ShootCannonball();
                break;
            case AttackType.Cone:
                AttackFlame();
                break;
            case AttackType.Chain:
                AttackLightning();
                break;
        }
    }

    Transform GetAimPoint(TroopAgent troop)
    {
        Transform aimPoint = troop.transform.Find("HitPoint");
        if (aimPoint == null)
        {
            aimPoint = troop.transform;
        }
        return aimPoint;
    }

    void ShootArrow()
    {
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.Launch(GetAimPoint(currentTarget), data.projectileSpeed, data.damage);
    }

    void ShootCannonball()
    {
        Vector3 impactPoint = GetAimPoint(currentTarget).position;
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.LaunchArea(impactPoint, data.projectileSpeed, data.damage, data.areaRadius);
    }

    void AttackFlame()
    {
        List<TroopAgent> troopsInRange = DamageHelper.GetTroopsInRadius(transform.position, data.range);

        foreach (TroopAgent troop in troopsInRange)
        {
            Vector3 direction = troop.transform.position - transform.position;
            direction.y = 0;
            float angle = Vector3.Angle(transform.forward, direction);

            if (angle <= data.coneAngle / 2)
            {
                DamageHelper.DealDamage(troop, data.damage);
                Debug.DrawLine(firePoint.position, troop.transform.position, Color.red, 0.2f);
            }
        }
    }

    void AttackLightning()
    {
        List<TroopAgent> alreadyHit = new List<TroopAgent>();
        TroopAgent current = currentTarget;
        Vector3 lastPosition = firePoint.position;

        for (int i = 0; i < data.chainTargets; i++)
        {
            if (current == null)
            {
                break;
            }

            DamageHelper.DealDamage(current, data.damage);
            Debug.DrawLine(lastPosition, current.transform.position, Color.yellow, 0.3f);

            alreadyHit.Add(current);
            lastPosition = current.transform.position;
            current = FindNextChainTarget(lastPosition, alreadyHit);
        }
    }

    TroopAgent FindNextChainTarget(Vector3 from, List<TroopAgent> alreadyHit)
    {
        List<TroopAgent> troopsInRange = DamageHelper.GetTroopsInRadius(transform.position, data.range);

        TroopAgent best = null;
        float bestDistance = Mathf.Infinity;

        foreach (TroopAgent troop in troopsInRange)
        {
            if (alreadyHit.Contains(troop))
            {
                continue;
            }

            float distance = Vector3.Distance(from, troop.transform.position);
            if (distance < bestDistance)
            {
                best = troop;
                bestDistance = distance;
            }
        }
        return best;
    }

    [ContextMenu("Test: Tower destroy")]
    public void DestroyTower()
    {
        BuildSlot slot = GetComponentInParent<BuildSlot>();
        if (slot != null)
        {
            slot.Clear();
        }

        if (TowerDestroyed != null)
        {
            TowerDestroyed(this);
        }

        Debug.Log(data.towerName + " was destroyed");
        Destroy(gameObject);
    }
}