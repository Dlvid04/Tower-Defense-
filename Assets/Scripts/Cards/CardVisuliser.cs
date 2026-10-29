using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardVisuliser : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text cost;

    public void UpdateCard(CardData c)
    {
        title.text = c.cardName;
        cost.text = ">> " + c.placeCost + " <<";
    }
    private void Update()
    {
        if(GameManager.GameIsRunning && CardManager.timer > 0)
        {
            background.fillAmount = 1 - (CardManager.timer / CardManager.timerMax);
        }
        else
        {
            background.fillAmount = 1;
        }
    }
}
