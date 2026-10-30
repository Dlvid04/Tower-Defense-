using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(Health))]
public class TroopAgent : MonoBehaviour
{

    int groupID = 0;
    public int GroupID
    {
        get { return groupID; }
    }

    public float healthbarOffset = 0;
    public Transform firePoint;

    public Troop troop;

    [HideInInspector] public float speedMultiplier = 1;

    public float _health
    {
        get { return health.Current; }
        set { health.Current = Mathf.Max(value, 0); }
    }
    protected Health health;
    protected WaypointPath path;
    protected int waypointIndex = 0;
    protected bool canMove = true;

    Animator anim;
    bool isWalking = false;
    bool initialized = false;
    Tower currentTarget;
    float attackTimer;
    static List<int> finishedGroups = new List<int>();

    int spawnOrder = 0;
    Vector3 walkDirection = Vector3.forward;
    Vector3 anchorPosition;
    static float formationSpacing = 0.9f;
    static float catchUpFactor = 1.5f;

    [Header("Smooth movement")]
    public float turnSpeed = 360f;
    public float cornerRadius = 0.8f;
    public float acceleration = 8f;
    public float followSmoothTime = 0.2f;
    public float laneHalfWidth = 1.5f;

    float currentSpeed = 0;
    Vector3 followVelocity = Vector3.zero;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetGroups()
    {
        finishedGroups.Clear();
    }

    public virtual void Init(int groupID)
    {
        this.groupID = groupID;
        health = GetComponent<Health>();
        anim = GetComponentInChildren<Animator>();

        if (troop == null)
        {
            Debug.LogError(name + ": Am TroopAgent fehlt das Troop-Asset!");
            return;
        }

        health.Init(Team.Player, troop.MaxHealth, healthbarOffset);
        health.OnDied += OnDied;

        path = FindFirstObjectByType<WaypointPath>();
        if (path == null)
        {
            Debug.LogWarning("Kein WaypointPath (Lane) in der Szene gefunden!");
        }

        spawnOrder = GetSpawnOrder();
        anchorPosition = transform.position;
        waypointIndex = 0;
        initialized = true;
    }

    int GetSpawnOrder()
    {
        int order = 0;
        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent other in allTroops)
        {
            if (other != this && other.GroupID == groupID && other.initialized)
            {
                order = order + 1;
            }
        }
        return order;
    }

    TroopAgent GetGroupLeader()
    {
        if (groupID == 0)
        {
            return null;
        }

        TroopAgent leader = null;
        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent other in allTroops)
        {
            if (other.GroupID != groupID || other.initialized == false)
            {
                continue;
            }
            if (other.health == null || other.health.IsDead)
            {
                continue;
            }

            if (leader == null || other.spawnOrder < leader.spawnOrder)
            {
                leader = other;
            }
        }
        return leader;
    }

    Vector3 GetFormationOffset()
    {
        int count = 0;
        int rank = 0;
        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent other in allTroops)
        {
            if (other.GroupID != groupID || other.initialized == false)
            {
                continue;
            }
            if (other.health == null || other.health.IsDead)
            {
                continue;
            }

            count = count + 1;
            if (other.spawnOrder < spawnOrder)
            {
                rank = rank + 1;
            }
        }
        return GetSlot(rank, count);
    }

    Vector3 GetSlot(int rank, int count)
    {
        float side = formationSpacing / 2;
        float row = formationSpacing / 2;

        if (count == 2)
        {
            if (rank == 0) return new Vector3(-side, 0, 0);
            return new Vector3(side, 0, 0);
        }

        if (count == 3)
        {
            if (rank == 0) return new Vector3(0, 0, row);
            if (rank == 1) return new Vector3(-side, 0, -row);
            return new Vector3(side, 0, -row);
        }

        if (count == 4)
        {
            if (rank == 0) return new Vector3(-side, 0, row);
            if (rank == 1) return new Vector3(side, 0, row);
            if (rank == 2) return new Vector3(-side, 0, -row);
            return new Vector3(side, 0, -row);
        }

        if (count >= 5)
        {
            if (rank == 0) return new Vector3(0, 0, formationSpacing);
            if (rank == 1) return new Vector3(-side, 0, 0);
            if (rank == 2) return new Vector3(side, 0, 0);
            if (rank == 3) return new Vector3(-side, 0, -formationSpacing);
            if (rank == 4) return new Vector3(side, 0, -formationSpacing);
        }

        return Vector3.zero;
    }

    void Start()
    {
        if (initialized == false)
        {
            Init(0);
        }
    }

    void Update()
    {
        if (GameManager.GameIsRunning)
            TroopUpdate();
    }


    public virtual void TroopUpdate()
    {
        if (initialized == false)
        {
            return;
        }

        if (path != null && waypointIndex >= path.points.Length)
        {
            ExitMap();
            return;
        }
        currentTarget = FindTarget();

        if (currentTarget != null)
        {
            canMove = false;
            Attack();
        }
        else
        {
            Tower groupTarget = GetGroupTarget();
            if (groupTarget != null)
            {
                MoveTowardTower(groupTarget);
                return;
            }
            canMove = true;
        }

        Move();
    }

    void Move()
    {
        if (canMove == false || path == null || waypointIndex >= path.points.Length)
        {
            currentSpeed = 0;
            followVelocity = Vector3.zero;
            SetWalking(false);
            return;
        }

        TroopAgent leader = GetGroupLeader();
        if (leader == null)
        {
            MoveAlongLane();
            return;
        }

        if (leader == this)
        {
            MoveAnchor();
        }
        else
        {
            anchorPosition = leader.anchorPosition;
            walkDirection = leader.walkDirection;
            waypointIndex = leader.waypointIndex;
        }

        MoveToSlot();
    }

    void MoveAlongLane()
    {
        Vector3 target = path.points[waypointIndex].position;
        target.y = transform.position.y;

        Vector3 direction = target - transform.position;
        if (direction.magnitude < GetSwitchDistance())
        {
            waypointIndex = waypointIndex + 1;
            return;
        }

        walkDirection = direction.normalized;
        transform.position = Vector3.MoveTowards(transform.position, target, GetSmoothSpeed() * Time.deltaTime);
        TurnTowards(direction);
        SetWalking(true);
    }

    void MoveAnchor()
    {
        Vector3 target = path.points[waypointIndex].position;
        target.y = anchorPosition.y;

        Vector3 direction = target - anchorPosition;
        if (direction.magnitude < GetSwitchDistance())
        {
            waypointIndex = waypointIndex + 1;
            return;
        }

        walkDirection = Vector3.RotateTowards(walkDirection, direction.normalized, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0);
        anchorPosition = Vector3.MoveTowards(anchorPosition, target, GetSmoothSpeed() * Time.deltaTime);
    }

    void MoveToSlot()
    {
        Quaternion turn = Quaternion.LookRotation(walkDirection);
        Vector3 target = anchorPosition + turn * GetFormationOffset();
        target.y = transform.position.y;

        float maxSpeed = troop.Speed * speedMultiplier * GetWeatherSpeed() * catchUpFactor;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref followVelocity, followSmoothTime, maxSpeed);
        TurnTowards(walkDirection);
        SetWalking(followVelocity.magnitude > 0.05f);
    }

    float GetSwitchDistance()
    {
        if (waypointIndex >= path.points.Length - 1)
        {
            return 0.1f;
        }
        return cornerRadius;
    }

    float GetSmoothSpeed()
    {
        float speed = troop.Speed * speedMultiplier * GetWeatherSpeed();
        currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);
        return currentSpeed;
    }

    void TurnTowards(Vector3 direction)
    {
        direction.y = 0;
        if (direction == Vector3.zero)
        {
            return;
        }
        Quaternion goal = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, turnSpeed * Time.deltaTime);
    }

    Tower GetGroupTarget()
    {
        if (groupID == 0)
        {
            return null;
        }

        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent other in allTroops)
        {
            if (other == this || other.GroupID != groupID || other.initialized == false)
            {
                continue;
            }
            if (other.health == null || other.health.IsDead)
            {
                continue;
            }
            if (other.currentTarget != null)
            {
                return other.currentTarget;
            }
        }
        return null;
    }

    void MoveTowardTower(Tower tower)
    {
        Quaternion turn = Quaternion.LookRotation(walkDirection);
        Vector3 target = tower.transform.position + turn * (GetFormationOffset() * 0.5f);
        target = ClampToLane(target);
        target.y = transform.position.y;

        currentSpeed = 0;
        float speed = troop.Speed * speedMultiplier * GetWeatherSpeed();
        transform.position = Vector3.SmoothDamp(transform.position, target, ref followVelocity, followSmoothTime, speed);
        TurnTowards(target - transform.position);
        SetWalking(true);
    }

    Vector3 ClampToLane(Vector3 point)
    {
        if (path == null)
        {
            return point;
        }

        int nextIndex;
        Vector3 middle = path.PointAt(path.ProgressAt(point), out nextIndex);
        Vector3 side = point - middle;
        side.y = 0;
        side = Vector3.ClampMagnitude(side, laneHalfWidth);
        return new Vector3(middle.x + side.x, point.y, middle.z + side.z);
    }

    float GetWeatherSpeed()
    {
        if (WeatherManager.curentWeather == WetterArt.Regen)
        {
            return troop.rainSpeedMultiplier;
        }
        return 1;
    }

    Tower FindTarget()
    {
        Tower[] allTowers = FindObjectsByType<Tower>(FindObjectsSortMode.None);

        Tower best = null;
        float bestDistance = Mathf.Infinity;

        foreach (Tower tower in allTowers)
        {
            float distance = DistanceTo(tower);
            if (distance > troop.attackRange)
            {
                continue;
            }

            if (distance < bestDistance)
            {
                best = tower;
                bestDistance = distance;
            }
        }
        return best;
    }

    float DistanceTo(Tower tower)
    {
        Vector3 point = tower.transform.position;

        Collider towerCollider = tower.GetComponent<Collider>();
        if (towerCollider != null)
        {
            point = towerCollider.ClosestPoint(transform.position);
        }

        point.y = transform.position.y;
        return Vector3.Distance(transform.position, point);
    }

    void Attack()
    {
        TurnTowards(currentTarget.transform.position - transform.position);

        attackTimer = attackTimer - Time.deltaTime;
        if (attackTimer <= 0)
        {
            PlayAnimation("Attack");
            if (troop.projectilePrefab != null)
            {
                Vector3 startPosition = transform.position;
                if (firePoint != null)
                {
                    startPosition = firePoint.position;
                }

                TroopProjectile projectile = Instantiate(troop.projectilePrefab, startPosition, Quaternion.identity);
                projectile.Launch(currentTarget, troop.damage);
            }
            else
            {
                DamageHelper.DealDamageToTower(currentTarget, troop.damage);
            }
            attackTimer = troop.attackCooldown;
        }
    }

    void OnDied(Health h)
    {
        PlayAnimation("Die");

        if (IsGroupFinished() == false && OthersOfGroupAlive() == false)
        {
            FinishGroup();
            GameEvents.RaiseUnitKilled(this);
            Debug.Log(troop.name + " was defeated");
        }

        if (troop.deathEffect != null)
        {
            Instantiate(troop.deathEffect, transform.position, troop.deathEffect.transform.rotation);
        }

        Destroy(gameObject);
    }

    void ExitMap()
    {
        if (IsGroupFinished() == false)
        {
            FinishGroup();
            GameEvents.RaiseUnitExitedMap(this);
            //Debug.Log(troop.name + " has left the map: AI loses 1 life");
        }

        initialized = false;
        Destroy(gameObject);
    }

    bool IsGroupFinished()
    {
        return groupID != 0 && finishedGroups.Contains(groupID);
    }

    void FinishGroup()
    {
        if (groupID != 0)
        {
            finishedGroups.Add(groupID);
        }
    }

    bool OthersOfGroupAlive()
    {
        if (groupID == 0)
        {
            return false;
        }

        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent other in allTroops)
        {
            if (other == this || other.GroupID != groupID)
            {
                continue;
            }

            if (other.health != null && other.health.IsDead == false)
            {
                return true;
            }
        }
        return false;
    }

    protected void PlayAnimation(string trigger)
    {
        if (anim != null)
        {
            anim.SetTrigger(trigger);
        }
    }

    void SetWalking(bool walking)
    {
        if (walking && isWalking == false)
        {
            PlayAnimation("Walk");
        }
        isWalking = walking;
    }

    public static TroopAgent FindLightningRod(Vector3 point)
    {
        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent agent in allTroops)
        {
            if (agent.troop == null || agent.troop.isHero == false)
            {
                continue;
            }

            if (agent.health == null || agent.health.IsDead)
            {
                continue;
            }

            if (Vector3.Distance(point, agent.transform.position) <= agent.troop.lightningRadius)
            {
                return agent;
            }
        }
        return null;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up * healthbarOffset, 0.1f);
    }
#endif
}