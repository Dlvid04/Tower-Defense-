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


    int lives = 3;
    public int Lives
    {
        get { return lives; }
        set { lives = value; }
    }



    void Start()
    {
        Application.targetFrameRate = 60;

        Cursor.lockState = CursorLockMode.Confined;

        InputActions.Enable();

        instance = this;
        StartGame();
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

    public void GameEnd(EndType endType)
    {
        switch (endType)
        {
            case EndType.Timeout:
                Debug.Log("Timeout");
                break;
            case EndType.Lose:
                Debug.Log("Lose");
                break;
            case EndType.Win:
                Debug.Log("Win");
                break;
        }
    }
}

public enum EndType { Timeout, Lose, Win }