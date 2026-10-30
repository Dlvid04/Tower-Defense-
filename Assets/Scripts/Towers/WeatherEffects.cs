using System.Collections.Generic;
using UnityEngine;

public class WeatherEffects : MonoBehaviour
{
    [Header("Nebel")]
    public ParticleSystem fog;
    public float fogFadeOutSpeed = 3f;

    [Header("Regen")]
    public GameObject rainPrefab;           
    public float troopRainHeight = 6.5f;    
    public float towerRainHeight = 8f;      
    public float checkInterval = 0.5f;      

    const string rainName = "RainFX";       

    bool isRaining;
    bool effectsStopped;                    
    float checkTimer;
    List<GameObject> activeRain = new List<GameObject>();

    Dictionary<int, GameObject> groupRain = new Dictionary<int, GameObject>();

    void OnEnable()
    {
        WeatherManager.WetherChanged += HandleWeatherChanged;
    }

    void OnDisable()
    {
        WeatherManager.WetherChanged -= HandleWeatherChanged;
    }

    void Start()
    {
        fog.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void HandleWeatherChanged(WetterArt weather)
    {
        if (GameManager.GameIsRunning == false)
        {
            return;
        }

        UpdateFog(weather);
        UpdateRain(weather);
    }

    void Update()
    {
        if (GameManager.GameIsRunning == false)
        {
            if (effectsStopped == false)
            {
                StopAllEffects();
            }
            return;
        }

        if (isRaining == false)
        {
            return;
        }

        checkTimer = checkTimer + Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0;
            AddRainEverywhere();
        }
    }

    void StopAllEffects()
    {
        effectsStopped = true;
        UpdateFog(WetterArt.None);
        UpdateRain(WetterArt.None);
    }

    // ---------- Nebel ----------
    void UpdateFog(WetterArt weather)
    {
        ParticleSystem.MainModule fogMain = fog.main;

        if (weather == WetterArt.Nebel)
        {
            fogMain.simulationSpeed = 1f;
            fog.Play();
        }
        else
        {
            fogMain.simulationSpeed = fogFadeOutSpeed;
            fog.Stop();
        }
    }

    // ---------- Regen ----------
    void UpdateRain(WetterArt weather)
    {
        isRaining = weather == WetterArt.Regen;

        if (isRaining)
        {
            AddRainEverywhere();
        }
        else
        {
            RemoveAllRain();
        }
    }

    void AddRainEverywhere()
    {
        Tower[] allTowers = FindObjectsByType<Tower>(FindObjectsSortMode.None);
        foreach (Tower tower in allTowers)
        {
            if (tower.data.attackType == AttackType.Cone)
            {
                AddRain(tower.transform, towerRainHeight);
            }
        }

        TroopAgent[] allTroops = FindObjectsByType<TroopAgent>(FindObjectsSortMode.None);
        foreach (TroopAgent troop in allTroops)
        {
            if (troop.GroupID == 0)
            {
                AddRain(troop.transform, troopRainHeight);
            }
            else
            {
                AddGroupRain(troop);
            }
        }
    }

    void AddGroupRain(TroopAgent troop)
    {
        int id = troop.GroupID;

        if (groupRain.ContainsKey(id) && groupRain[id] != null)
        {
            return;
        }

        groupRain[id] = AddRain(troop.transform, troopRainHeight);
    }

    GameObject AddRain(Transform owner, float height)
    {
        Transform oldRain = owner.Find(rainName);
        if (oldRain != null)
        {
            return oldRain.gameObject;
        }

        GameObject rain = Instantiate(rainPrefab, owner);
        rain.name = rainName;
        rain.transform.localPosition = new Vector3(0, height, 0);
        activeRain.Add(rain);
        return rain;
    }

    void RemoveAllRain()
    {
        foreach (GameObject rain in activeRain)
        {
            if (rain != null)
            {
                Destroy(rain);
            }
        }
        activeRain.Clear();
        groupRain.Clear();
    }

    // ---------- Tests im Play-Modus ----------
    [ContextMenu("Test: Nebel an")]
    void TestFogOn()
    {
        UpdateFog(WetterArt.Nebel);
    }

    [ContextMenu("Test: Nebel aus")]
    void TestFogOff()
    {
        UpdateFog(WetterArt.None);
    }

    [ContextMenu("Test: Regen an")]
    void TestRainOn()
    {
        UpdateRain(WetterArt.Regen);
    }

    [ContextMenu("Test: Regen aus")]
    void TestRainOff()
    {
        UpdateRain(WetterArt.None);
    }
}