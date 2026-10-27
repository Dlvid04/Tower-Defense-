using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public float hitEffectDuration = 0.4f;

    Transform target;        
    Vector3 targetPosition;  
    float speed;             
    float damage;            
    float areaRadius;        

    public void Launch(Transform newTarget, float newSpeed, float newDamage)
    {
        target = newTarget;
        targetPosition = newTarget.position;
        speed = newSpeed;
        damage = newDamage;
        areaRadius = 0;
    }

    public void LaunchArea(Vector3 position, float newSpeed, float newDamage, float newAreaRadius)
    {
        target = null;
        targetPosition = position;
        speed = newSpeed;
        damage = newDamage;
        areaRadius = newAreaRadius;
    }

    void Update()
    {
        if (target != null)
        {
            targetPosition = target.position;
        }

        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Hit();
        }
    }

    void Hit()
    {
        if (areaRadius > 0)
        {
            List<TroopAgent> troops = DamageHelper.GetTroopsInRadius(transform.position, areaRadius);
            foreach (TroopAgent troop in troops)
            {
                DamageHelper.DealDamage(troop, damage);
            }
        }
        else if (target != null)
        {
            TroopAgent troop = target.GetComponentInParent<TroopAgent>();
            if (troop != null)
            {
                DamageHelper.DealDamage(troop, damage);
            }
        }

        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, hitEffectDuration);
        }

        Destroy(gameObject);
    }
}