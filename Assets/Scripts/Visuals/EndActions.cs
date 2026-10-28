using UnityEngine;
using UnityEngine.SceneManagement;

public class EndActions : MonoBehaviour
{
    bool toMenu = false;
    public void BackToMenu()
    {
        toMenu = true;
    }
    public void Retry()
    {
        toMenu = false;
    }
    public void DoAction()
    {
        if(toMenu)
        {
            SceneManager.LoadSceneAsync("Main Menu");
        }
        else
        {
            SceneManager.LoadSceneAsync("Game Scene");
        }
    }
}
