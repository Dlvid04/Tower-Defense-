using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    #region statics
    public static float timer = 0;
    public static float timerMax = 0;

    public Wallet mutWallet;
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
    [SerializeField] Animator freeCardInfoBox;


    [Header("Settings")]
    [SerializeField] int StartCardCount = 4;
    [SerializeField] int MaxCardsInHand = 2;
    [SerializeField] float placeCooldown = 2;


    [Header("Debug")]
    public List<Card> cards = new();
    public List<CardData> cardsInDeck = new();


    GameObject partical;
    bool freeCardUsed = false;

    void Start()
    {
        timer = 0;

        freeCardUsed = false;

        timerMax = placeCooldown;

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
        if (!GameManager.GameIsRunning)
            return;

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
        if (!GameManager.GameIsRunning)
            return;

        if (cardsInDeck.Count >= 1)
        {
            int r = Random.Range(0, cardsInDeck.Count);
            GiveCard(cardsInDeck[r]);
            cardsInDeck.RemoveAt(r);
        }
    }

    public bool PlaceCard(Card card)
    {
        if (!GameManager.GameIsRunning || timer > 0 || GameManager.GropsOnMap >= 4)
            return false;

        if (card != null)
        {
            if (card.CanPlace() || !freeCardUsed)
            {
                if (!freeCardUsed)
                {
                    freeCardInfoBox.Play("FreeCard_Hide");
                    freeCardUsed = true;
                }
                else
                {
                    ChageMUT(-card.GetCost());
                }

                card.PlaceAgents(AgentStartPoint.position);
                Destroy(card.gameObject);
                cardsInDeck.Add(card.Data);
                cards.Remove(card);
                RefillCards();

                Instantiate(partical, AgentStartPoint.position, partical.transform.rotation);

                timer = placeCooldown;
                return true;
            }
        }
        return false;
    }

    public void ChageMUT(int value)
    {
        if (!GameManager.GameIsRunning)
            return;

        if (mutWallet.ChangeMut(value))
            UpdateMUTDislpay();
    }

    public void UpdateMUTDislpay()
    {
        MutVisual.text = mutWallet.MUT + "/" + mutWallet.Max;
        TimeManager.timeManager.SetMutDisplay();
    }

    void OnDestroy()
    {
        TimeManager.MutUpdate -= UpdateMUTDislpay;
    }
}
