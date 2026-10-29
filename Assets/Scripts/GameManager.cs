using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        aiHealth.TakeDamage(1);
        AIHealthChanged.Invoke(Mathf.RoundToInt(aiHealth.Current));
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
            case EndType.Lose:
                Debug.Log("Lose");
                endAnimation.Play("LOSE");
                break;
            case EndType.Win:
                Debug.Log("Win");
                endAnimation.Play("WIN");
                break;
        }
    }
}

public enum EndType { Timeout, Lose, Win }