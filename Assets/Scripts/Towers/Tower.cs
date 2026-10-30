using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerData data;
    public Transform firePoint;
    public Projectile projectilePrefab;
    public GameObject attackEffectPrefab;
    public float attackEffectDuration = 0.2f;
    public GameObject destroyEffectPrefab;
    public float destroyEffectDuration = 1f;

    public Transform rotatingPart;
    public float healthbarOffset = 0;

    TroopAgent currentTarget;
    WaypointPath path;
    float attackTimer;
    Quaternion rotatingPartStart;

    public static event System.Action<Tower> TowerDestroyed;

    void Start()
    {
        path = FindFirstObjectByType<WaypointPath>();
        if (path == null)
        {
            Debug.LogWarning("WaypointPath is nowhere to be found in the scene!");
        }
        if (rotatingPart != null)
        {
            rotatingPartStart = Quaternion.Inverse(transform.rotation) * rotatingPart.rotation;
        }
    }

    void Update()
    {
        if (GameManager.GameIsRunning == false)
        {
            return;
        }

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
            attackTimer = 1f / data.attackPerSecond;
        }
    }

    TroopAgent FindTarget()
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
            float distanceToTower = Vector3.Distance(transform.position, troop.transform.position);
            if (distanceToTower > GetRange())
            {
                continue;
            }

            float progress = path.ProgressAt(troop.transform.position);
            if (progress > bestProgress)
            {
                best = troop;
                bestProgress = progress;
            }
        }

        return best;
    }

    void LookAtTarget()
    {
        if (rotatingPart == null)
        {
            return;
        }

        Vector3 direction = currentTarget.transform.position - rotatingPart.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            rotatingPart.rotation = Quaternion.LookRotation(direction) * rotatingPartStart;
        }
    }

    void Attack()
    {
        if (MissesInFog(currentTarget))
        {
            Debug.Log(data.towerName + " misses " + currentTarget.name + " in the fog");
            DamageHelper.DealDamage(currentTarget, -1);
            return;
        }

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
        projectile.Launch(GetAimPoint(currentTarget), data.projectileSpeed, GetDamage());
    }

    void ShootCannonball()
    {
        Vector3 impactPoint = GetAimPoint(currentTarget).position;
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.LaunchArea(impactPoint, data.projectileSpeed, GetDamage(), data.areaRadius);
    }

    void AttackFlame()
    {
        Vector3 aimDirection = currentTarget.transform.position - transform.position;
        aimDirection.y = 0;
        if (aimDirection == Vector3.zero)
        {
            aimDirection = transform.forward;
        }

        SpawnAttackEffect(firePoint.position, Quaternion.LookRotation(aimDirection));

        List<TroopAgent> troopsInRange = DamageHelper.GetTroopsInRadius(transform.position, GetRange());

        foreach (TroopAgent troop in troopsInRange)
        {
            Vector3 direction = troop.transform.position - transform.position;
            direction.y = 0;
            float angle = Vector3.Angle(aimDirection, direction);

            if (angle <= data.coneAngle / 2)
            {
                DamageHelper.DealDamage(troop, GetDamage());
                Debug.DrawLine(firePoint.position, troop.transform.position, Color.red, 0.2f);
            }
        }
    }

    void AttackLightning()
    {
        List<TroopAgent> alreadyHit = new List<TroopAgent>();
        TroopAgent current = currentTarget;
        Transform start = firePoint;

        for (int i = 0; i < data.chainTargets; i++)
        {
            if (current == null)
            {
                break;
            }

            Transform hitPoint = GetAimPoint(current);

            DamageHelper.DealDamage(current, GetDamage());
            SpawnChainEffect(start, hitPoint);

            alreadyHit.Add(current);
            start = hitPoint;
            current = FindNextChainTarget(current.transform.position, alreadyHit);
        }
    }

    void SpawnChainEffect(Transform from, Transform to)
    {
        if (attackEffectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(attackEffectPrefab, from.position, Quaternion.identity);

        Laser laser = effect.GetComponentInChildren<Laser>();
        if (laser != null)
        {
            laser.SetChain(from, to);
        }

        Destroy(effect, attackEffectDuration);
    }

    TroopAgent FindNextChainTarget(Vector3 from, List<TroopAgent> alreadyHit)
    {
        List<TroopAgent> troopsInRange = DamageHelper.GetTroopsInRadius(from, data.chainRadius);

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

    // ---------- Wetter ----------
    float GetRange()
    {
        float range = data.range;
        if (WeatherManager.curentWeather == WetterArt.Nebel)
        {
            range = range * data.fogRangeMultiplier;
        }
        return range;
    }

    float GetDamage()
    {
        float damage = data.damage;
        if (WeatherManager.curentWeather == WetterArt.Regen)
        {
            damage = damage * data.rainDamageMultiplier;
        }
        if (WeatherManager.curentWeather == WetterArt.Gewitter)
        {
            damage = damage * data.stormDamageMultiplier;
        }
        return damage;
    }

    bool MissesInFog(TroopAgent troop)
    {
        if (WeatherManager.curentWeather != WetterArt.Nebel)
        {
            return false;
        }
        if (data.swarmTroop == null || troop.troop != data.swarmTroop)
        {
            return false;
        }
        return Random.value < data.fogSwarmMissChance;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up * healthbarOffset, 0.1f);

        if (data == null)
        {
            return;
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetRange());
    }
#endif

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

        if (destroyEffectPrefab != null)
        {
            GameObject effect = Instantiate(destroyEffectPrefab, transform.position, destroyEffectPrefab.transform.rotation);
            Destroy(effect, destroyEffectDuration);
        }
        Debug.Log(data.towerName + " was destroyed");
        Destroy(gameObject);
    }

    void SpawnAttackEffect(Vector3 position, Quaternion rotation)
    {

        if (attackEffectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(attackEffectPrefab, position, rotation);
        Destroy(effect, attackEffectDuration);

    }
}