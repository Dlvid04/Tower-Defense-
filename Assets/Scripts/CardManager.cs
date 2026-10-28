using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    #region statics
    public Wallet mutWallet = new();
    public static int MUT
    {
        get
        {
            if(instance == null)
                return 0;
            return instance.mutWallet.MUT;
        }
    }

    public static bool dragging = false;

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
    #endregion

    [Header("Visuals")]
    [SerializeField] TMP_Text MutVisual;


    [Header("Setup")]
    [SerializeField] GameObject cardPrefeb;
    [SerializeField] Transform AgentStartPoint;
    [SerializeField] Transform CartParent;
    public Transform CartDeckRef;


    [Header("Settings")]
    [SerializeField] int StartCardCount = 4;
    [SerializeField] int MaxCardsInHand = 2;


    [Header("Debug")]
    public List<Card> cards = new();
    public List<CardData> cardsInDeck = new();

    GameObject partical;

    void Start()
    {
        instance = this;
        mutWallet = new();
        UpdateMUTDislpay();
        dragging = false;
        partical = Resources.Load<GameObject>("Particals/Spawn Particles");

        TimeManager.MutUpdate += UpdateMUTDislpay;
    }

    public void GiveStarterCards()
    {
        List<CardData> d = Resources.LoadAll<CardData>("").ToList();

        if (d.Count < StartCardCount)
        {
            Debug.LogError("To Little cards availebel to fill deck");
            return;
        }

        for (int i = 0; i < StartCardCount; i++)
        {
            int r = Random.Range(0, d.Count);
            GiveCard(d[r]);
            d.RemoveAt(r);
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

            if (g == null)
                return;

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
                cardsInDeck.Add(card.Data);
                cards.Remove(card);
                RefillCards();

                Instantiate(partical, AgentStartPoint.position, partical.transform.rotation);
                return true;
            }
        }
        return false;
    }

    public void ChageMUT(int value)
    {
        if (mutWallet.ChangeMut(value))
            UpdateMUTDislpay();
    }

    public void UpdateMUTDislpay()
    {
        MutVisual.text = "Mut: " + mutWallet.MUT;
    }
}
