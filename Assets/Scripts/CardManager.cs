using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static int MUT;

    [SerializeField] GameObject cardPrefeb;

    public List<Card> cards = new();

    [SerializeField] Transform AgentStartPoint;

    void Start()
    {
        MUT = 0;
    }
    
    public void GiveCard(CardData card)
    {
        if (cardPrefeb == null)
        {
            Debug.LogError("Card Prefeb not asinged in inspector!");
            return;
        }   
        
        GameObject g = Instantiate(cardPrefeb);
        Card c = g.GetComponent<Card>();

        if (c == null)
        {
            Debug.LogError("Card Prefeb with missing [Card.cs] script!");
            return;
        }
        c.Init(card);
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
