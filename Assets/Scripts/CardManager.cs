using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static int MUT;

    [SerializeField] GameObject cardPrefeb;

    public List<Card> cards = new();
    public List<CardData> cardsInDeck = new();

    [SerializeField] Transform AgentStartPoint;

    [Header("Cards")]
    [SerializeField] int maxCardsInHand = 2;
    [SerializeField] Transform CartParent;

    public void Init()
    {
        MUT = 0;
    }

    public void GiveStarterCards(int count)
    {
        CardData[] c = Resources.LoadAll<CardData>("");
        for (int i = 0; i < count; i++)
        {
            GiveCard(c[Random.Range(0, c.Length)]);
        }
    }

    public void GiveCard(CardData card)
    { 
        if (cards.Count < maxCardsInHand)
        {
            if (cardPrefeb == null)
            {
                Debug.LogError("Card Prefeb not asinged in inspector!");
                return;
            }

            GameObject g = Instantiate(cardPrefeb, CartParent);
            Card c = g.GetComponent<Card>();

            if (c == null)
            {
                Debug.LogError("Card Prefeb with missing [Card.cs] script!");
                return;
            }
            cards.Add(c);
            c.Init(card);
        }
        else
        {
            cardsInDeck.Add(card);
        }
    }

    public void PlaceCard(Card card)
    {
        if (card != null)
        {
            if (card.CanPlace())
            {
                card.PlaceAgents(AgentStartPoint.position);
                Destroy(card);
                cards.Remove(card);
            }
        }
    }
}
