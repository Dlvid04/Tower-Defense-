using UnityEngine;

public class TroopProjectile : MonoBehaviour
{
    public float speed = 10f;
    public GameObject hitEffectPrefab;
    public float hitEffectDuration = 0.4f;

    Tower target;
    Transform targetPoint;
    float damage;

    public void Launch(Tower newTarget, float newDamage)
    {
        target = newTarget;
        damage = newDamage;

        targetPoint = newTarget.transform.Find("HitPoint");
        if (targetPoint == null)
        {
            targetPoint = newTarget.transform;
        }
    }

    void Update()
    {
        if (target == null || GameManager.GameIsRunning == false)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = targetPoint.position;
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
        DamageHelper.DealDamageToTower(target, damage);

        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, hitEffectPrefab.transform.rotation);
            Destroy(effect, hitEffectDuration);
        }

        Destroy(gameObject);
    }
}