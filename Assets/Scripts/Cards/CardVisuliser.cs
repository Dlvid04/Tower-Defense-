using UnityEngine;
using TMPro;

public class CardVisuliser : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text cost;

    public void UpdateCard(CardData c)
    {
        title.text = c.cardName;
        description.text = c.cardDescription;
        cost.text = ">> " + c.placeCost + " <<";
    }
}
