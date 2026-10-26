using UnityEngine;

public class GameManager : MonoBehaviour
{
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

        // make 4 cards
        cardManager.GiveStarterCards(4);
    }
}
