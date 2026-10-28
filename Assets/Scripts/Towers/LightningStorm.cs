using System.Collections.Generic;
using UnityEngine;

public class LightningStorm : MonoBehaviour
{
    public LightningData data;
    public GameObject strikeVisualPrefab;
    public bool isStormActive;
    LightningPoint[] allPoints;
    float strikeTimer;

    void OnEnable()
    {
        WeatherManager.WetherChanged += HandleWeatherChanged;
    }

    void OnDisable()
    {
        WeatherManager.WetherChanged -= HandleWeatherChanged;
    }

    void HandleWeatherChanged(WetterArt weather)
    {
        isStormActive = weather == WetterArt.Gewitter;
    }

    void Start()
    {
        allPoints = FindObjectsByType<LightningPoint>(FindObjectsSortMode.None);
        if (allPoints.Length == 0)
        {
            Debug.LogWarning("LightningPoint is nowhere to be found in the scene!");
        }
    }

    void Update()
    {
        if (isStormActive == false)
        {
            strikeTimer = 0;
            return;
        }

        strikeTimer = strikeTimer + Time.deltaTime;
        if (strikeTimer >= data.strikeInterval)
        {
            strikeTimer = 0;
            StrikeRandomPoint();
        }
    }

    [ContextMenu("Test: Lightning strike")]
    public void StrikeRandomPoint()
    {
        if (allPoints == null || allPoints.Length == 0)
        {
            return;
        }

        LightningPoint point = allPoints[Random.Range(0, allPoints.Length)];
        Strike(point.transform.position);
    }

    void Strike(Vector3 point)
    {
        TroopAgent lightningRod = TroopAgent.FindLightningRod(point);
        if (lightningRod != null)
        {
            Tower nearestTower = DamageHelper.FindNearestTower(lightningRod.transform.position);
            if (nearestTower != null)
            {
                Debug.DrawLine(lightningRod.transform.position, nearestTower.transform.position, Color.yellow, 1f);
                Debug.Log("Sturmbrecher leitet den Blitz auf " + nearestTower.data.towerName + " um");

                GameObject rodVisual = Instantiate(strikeVisualPrefab, nearestTower.transform.position, Quaternion.identity);
                Destroy(rodVisual, data.visualDuration);

                DamageHelper.DealDamageToTower(nearestTower, data.damage);
                return;
            }
        }
        List<GameObject> targets = new List<GameObject>();

        Tower[] allTowers = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        foreach (Tower tower in allTowers)
        {
            if (Vector3.Distance(point, tower.transform.position) <= data.searchRadius)
            {
                targets.Add(tower.gameObject);
            }
        }

        List<TroopAgent> troops = DamageHelper.GetTroopsInRadius(point, data.searchRadius);
        foreach (TroopAgent troop in troops)
        {
            targets.Add(troop.gameObject);
        }

        Vector3 hitPosition = point;
        if (targets.Count == 0)
        {
            Debug.Log("Lightning strikes the ground!");
        }
        else
        {
            GameObject target = targets[Random.Range(0, targets.Count)];
            hitPosition = target.transform.position;

            Tower hitTower = target.GetComponent<Tower>();
            if (hitTower != null)
            {
                DamageHelper.DealDamageToTower(hitTower, data.damage);
            }
            else
            {
                TroopAgent hitTroop = target.GetComponent<TroopAgent>();
                float damage = data.damage * hitTroop.troop.lightningDamageFactor;
                DamageHelper.DealDamage(hitTroop, damage, false);   
            }
        }

        GameObject visual = Instantiate(strikeVisualPrefab, hitPosition, Quaternion.identity);
        Destroy(visual, data.visualDuration);
    }
}