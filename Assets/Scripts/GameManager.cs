using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int GropsOnMap;
    static List<int> pasedID = new();

    public static bool GameIsRunning = false;

    public static event Action<int> AIHealthChanged;

    static Input_TowerDefence inputActions;
    public static Input_TowerDefence InputActions
    {
        get
        {
            if (inputActions == null)
                inputActions = new();
            return inputActions;
        }
    }

    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance != null)
                return instance;
            return FindFirstObjectByType<GameManager>();
        }
    }

    [SerializeField] CardManager cardManager;

    [SerializeField] Health aiHealth;
    [SerializeField] Animator endAnimation;
    [SerializeField] GameObject healthbarPrefab;

    void Start()
    {
        pasedID.Clear();


        GameEvents.UnitKilled += TrropDeath;

        Application.targetFrameRate = 60;

        Cursor.lockState = CursorLockMode.Confined;

        InputActions.Enable();

        instance = this;
        StartGame();

        aiHealth.Init(Team.AI, 3, spawnHealthBar: false);
        aiHealth.OnDied += AIDeath;

        GameEvents.UnitExitedMap += AIDamage;
    }

    public void StartGame()
    {
        GameIsRunning = true;

        Debug.Log("Game Start");
        StartRound();
    }

    public void StartRound()
    {
        UpdateTroopCount();

        Debug.Log("Starting new round");

        cardManager.GiveStarterCards();

        TimeManager.StartRoundTimer();
    }

    void AIDamage(TroopAgent a)
    {
        if (!pasedID.Contains(a.GroupID))
        {
            pasedID.Add(a.GroupID);

            aiHealth.TakeDamage(1);
            AIHealthChanged.Invoke(Mathf.RoundToInt(aiHealth.Current));
        }
        UpdateTroopCount();
    }
    void AIDeath(Health h) { GameEnd(EndType.Win); }

    public void GameEnd(EndType endType)
    {
        GameIsRunning = false;
        switch (endType)
        {
            case EndType.Timeout:
                Debug.Log("Timeout");
                endAnimation.Play("TIME");
                break;
            case EndType.Lose: // Erstmal nicht benutzt. kann aber nützlich für erweiterungen sein
                Debug.Log("Lose");
                endAnimation.Play("LOSE");
                break;
            case EndType.Win:
                Debug.Log("Win");
                endAnimation.Play("WIN");
                break;
        }
    }

    void TrropDeath(TroopAgent t)
    {
        t._health = 0;
        UpdateTroopCount();
    }

    public static void UpdateTroopCount()
    {
        print("update troop count");
        List<int> c = new();
        foreach (TroopAgent t in FindObjectsByType<TroopAgent>(FindObjectsSortMode.InstanceID))
        {
            if(t._health > 0 && !c.Contains(t.GroupID) && !pasedID.Contains(t.GroupID))
                c.Add(t.GroupID);
        }
        GropsOnMap = c.Count;
        print(GropsOnMap);
    }

    void OnDestroy()
    {
        GameEvents.UnitKilled -= TrropDeath;
        GameEvents.UnitExitedMap -= AIDamage;
    }

    public HealthBar AddHealthbar(Health h, float offset = 0)
    {
        if (h == null)
            return null;

        GameObject g = Instantiate(healthbarPrefab, h.transform);
        g.transform.localPosition = Vector3.up * offset;

        return g.GetComponent<HealthBar>();
    }
}

public enum EndType { Timeout, Lose, Win }