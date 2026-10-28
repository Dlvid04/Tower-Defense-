using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
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

    public static GameManager instance;

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
        Debug.Log("Game Start");
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log("Starting new round");

        cardManager.GiveStarterCards();

        TimeManager.StartRoundTimer();
    }

    void AIDamage(TroopAgent a) { aiHealth.TakeDamage(1); }
    void AIDeath(Health h) { GameEnd(EndType.Win); }

    public void GameEnd(EndType endType)
    {
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