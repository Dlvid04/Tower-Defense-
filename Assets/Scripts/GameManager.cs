using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int GropsOnMap;
    List<int> pasedID = new();

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

    void Start()
    {
        GameEvents.UnitKilled += TrropDeath;

        Application.targetFrameRate = 60;

        Cursor.lockState = CursorLockMode.Confined;

        InputActions.Enable();

        instance = this;
        StartGame();

        aiHealth.Init(Team.AI, 3);
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
        UpdateTroopCount();
    }

    public static void UpdateTroopCount()
    {
        List<int> c = new();
        foreach (TroopAgent t in FindObjectsByType<TroopAgent>(FindObjectsSortMode.InstanceID))
        {
            if(t._health > 0 && !c.Contains(t.GroupID))
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
}

public enum EndType { Timeout, Lose, Win }