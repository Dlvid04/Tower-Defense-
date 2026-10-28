using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Health))]
public class TroopAgent : MonoBehaviour
{

    int groupID = 0;
    public int GroupID
    {
        get { return groupID; }
    }

    public Troop troop;

    [HideInInspector] public float speedMultiplier = 1;   

    protected Health health;
    protected WaypointPath path;
    protected int waypointIndex = 0;
    protected bool canMove = true;

    Vector3 laneOffset;    
    Animator anim;
    bool isWalking = false;
    bool initialized = false;
    Tower currentTarget;    
    float attackTimer;
    static List<int> finishedGroups = new List<int>();
    static Vector3[] formation =
    {
        new Vector3(0, 0, 0),
        new Vector3(-0.7f, 0, 0.7f),
        new Vector3(0.7f, 0, 0.7f),
        new Vector3(-0.7f, 0, -0.7f),
        new Vector3(0.7f, 0, -0.7f),
    };


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

        health.Init(Team.Player, troop.MaxHealth);
        health.OnDied += OnDied;

        path = FindFirstObjectByType<WaypointPath>();
        if (path == null)
        {
            Debug.LogWarning("Kein WaypointPath (Lane) in der Szene gefunden!");
        }

        laneOffset = GetFormationOffset();
        waypointIndex = 0;
        initialized = true;
    }

    Vector3 GetFormationOffset()
    {
        if (groupID == 0)
        {
            return Vector3.zero;   
        }

        int index = 0;
        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent other in allTroops)
        {
            if (other != this && other.GroupID == groupID && other.initialized)
            {
                index = index + 1;
            }
        }
        return formation[index % formation.Length];
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
            canMove = true;   
        }

        Move();
    }

    void Move()
    {
        if (canMove == false || path == null || waypointIndex >= path.points.Length)
        {
            SetWalking(false);
            return;
        }

        Vector3 target = path.points[waypointIndex].position + laneOffset;
        target.y = transform.position.y;

        Vector3 direction = target - transform.position;
        if (direction.magnitude < 0.1f)
        {
            waypointIndex = waypointIndex + 1;   
            return;
        }

        float speed = troop.Speed * speedMultiplier * GetWeatherSpeed();
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(direction);   
        SetWalking(true);
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
        Vector3 direction = currentTarget.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        attackTimer = attackTimer - Time.deltaTime;
        if (attackTimer <= 0)
        {
            PlayAnimation("Attack");
            DamageHelper.DealDamageToTower(currentTarget, troop.damage);
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
        Destroy(gameObject);
    }

    void ExitMap()
    {
        if (IsGroupFinished() == false)
        {
            FinishGroup();
            GameEvents.RaiseUnitExitedMap(this);
            Debug.Log(troop.name + " has left the map: AI loses 1 life");
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
}