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
    [SerializeField] Transform CartParent;

    [SerializeField] int MaxCardsInHand = 2;

    void Start()
    {
        MUT = 0;
    }

    public void GiveStarterCards(int count)
    {
        CardData[] d = Resources.LoadAll<CardData>("");
        for (int i = 0; i < count; i++)
        {
            GiveCard(d[Random.Range(0, d.Length)]);
        }
    }

    public void GiveCard(CardData card)
    {
        if (cards.Count < MaxCardsInHand)
        {
            if (cardPrefeb == null)
            {
                Debug.LogError("Card Prefeb not asinged in inspector!");
                return;
            }

            GameObject g = Instantiate(cardPrefeb, CartParent);
            g.name = card.name;
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
