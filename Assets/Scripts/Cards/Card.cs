using UnityEngine;

[RequireComponent(typeof(CardVisuliser))]
public class Card : MonoBehaviour
{
    CardVisuliser visuliser;
    CardData data;

    public void Init(CardData cd)
    {
        visuliser = GetComponent<CardVisuliser>();

        data = cd;
        UpdateUI();
    }


    public void UpdateUI()
    {
        visuliser.UpdateCard(data);
    }

    public bool CanPlace()
    {
        return data.placeCost <= CardManager.MUT;
    }

    public void PlaceAgents(Vector3 position)
    {
        for (int i = 0; i < data.troopCount; i++)
        {
            Instantiate(data.prefab, position, Quaternion.identity);
        }
    }
}