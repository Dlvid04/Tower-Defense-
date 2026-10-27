using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    static CardManager instance;
    public static CardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<CardManager>();
            }
            return instance;
        }
    }

    static int mut;
    public static int MUT
    {
        get { return mut; }
        set { mut = Mathf.Clamp(value, 0, 10); }
    }

    [Header("Visuals")]
    [SerializeField] TMP_Text MutVisual;


    [Header("Setup")]
    [SerializeField] GameObject cardPrefeb;
    [SerializeField] Transform AgentStartPoint;
    [SerializeField] Transform CartParent;


    [Header("Settings")]
    [SerializeField] int StartCardCount = 4;
    [SerializeField] int MaxCardsInHand = 2;


    [Header("Debug")]
    public List<Card> cards = new();
    public List<CardData> cardsInDeck = new();

    void Start()
    {
        instance = this;
        MUT = 0;
        UpdateMUTDislpay();
    }

    public void GiveStarterCards()
    {
        CardData[] d = Resources.LoadAll<CardData>("");
        for (int i = 0; i < StartCardCount; i++)
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

    public void RefillCards()
    {
        if (cardsInDeck.Count >= 1)
        {
            GiveCard(cardsInDeck[0]);
            cardsInDeck.RemoveAt(0);
        }
    }

    public bool PlaceCard(Card card)
    {
        if (card != null)
        {
            if (card.CanPlace())
            {
                ChageMUT(-card.GetCost());

                card.PlaceAgents(AgentStartPoint.position);
                Destroy(card.gameObject);
                cards.Remove(card);
                RefillCards();
                return true;
            }
        }
        return false;
    }

    public void ChageMUT(int value)
    {
        MUT += value;
        UpdateMUTDislpay();
    }

    public void UpdateMUTDislpay()
    {
        MutVisual.text = "Mut: " + MUT;
    }
}
