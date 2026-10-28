using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 30;
    }
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("Game Scene");
    }
}
