using UnityEngine;
using TMPro;

public class CardVisuliser : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;

    public void UpdateCard(CardData c)
    {
        title.text = c.cardName;
        description.text = c.cardDescription;
    }
}
