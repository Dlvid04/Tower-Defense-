using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup c;
    float target;
    private void Start()
    {
        Application.targetFrameRate = 30;
        StartCoroutine(FadeIn());
    }
    public void StartGame()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.5f);
        while (c.alpha > 0)
        {
            c.alpha -= 0.05f;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (c.alpha <1)
        {
            c.alpha += 0.1f;
            yield return null;
        }
        SceneManager.LoadSceneAsync("Game Scene");
    }
}
