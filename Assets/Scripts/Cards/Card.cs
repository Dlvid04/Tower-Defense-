using UnityEngine;

[RequireComponent(typeof(CardVisuliser)), RequireComponent(typeof(Canvas)), RequireComponent(typeof(RectTransform))]
public class Card : MonoBehaviour
{
    public Transform MovingTransform;

    RectTransform rt;
    Canvas canvas;
    CardVisuliser visuliser;
    [SerializeField] CardData data;

    Vector3 target = -Vector3.one;
    Vector3 scale = Vector3.one;
    bool draged = false;

    public void Init(CardData cd)
    {

        visuliser = GetComponent<CardVisuliser>();
        canvas = GetComponent<Canvas>();
        rt = GetComponent<RectTransform>();

        data = cd;
        UpdateUI();

        MovingTransform.position = CardManager.Instance.CartDeckRef.position;
        MovingTransform.localScale = Vector3.one * 0.25f;
    }


    public void UpdateUI()
    {
        visuliser.UpdateCard(data);
    }

    public bool CanPlace()
    {
        return GetCost() <= CardManager.MUT;
    }

    public int GetCost()
    {
        return data.placeCost;
    }

    public void PlaceAgents(Vector3 position)
    {
        int groupID = Random.Range(0, 999);
        for (int i = 0; i < data.troopCount; i++)
        {
            GameObject g = Instantiate(data.agent.gameObject, position, Quaternion.identity);
            TroopAgent ta = g.GetComponent<TroopAgent>();

            ta.Init(groupID);
        }
        Debug.Log("Spawned " + data.troopCount + " Troops with the id: " + groupID);
    }
    public float sdf;
    void Update()
    {
        Vector2 mp = GameManager.InputActions.UI.Point.ReadValue<Vector2>();

        Vector2 min = new(rt.position.x - (rt.rect.width * rt.lossyScale.x / 2), rt.position.y - (rt.rect.height * rt.lossyScale.y / 2));
        Vector2 max = new(rt.position.x + (rt.rect.width * rt.lossyScale.x / 2), rt.position.y + (rt.rect.height * rt.lossyScale.y / 2));

        if (GameManager.InputActions.UI.Click.IsPressed())
        {

            if (!draged && (mp.x > min.x && mp.x < max.x && mp.y > min.y && mp.y < max.y) && !CardManager.dragging)
            {
                CardManager.dragging = true;
                draged = true;
                scale = Vector3.one * 0.5f;
                canvas.sortingOrder = 1;
            }

            if (draged)
            {
                target = mp;
            }
        }
        else if(draged)
        {
            CardManager.dragging = false;
            if (!(mp.x > min.x && mp.x < max.x && mp.y > min.y && mp.y < max.y))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(mp), out RaycastHit hit))
                {
                    Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green, 5);
                    if(CardManager.Instance.PlaceCard(this))
                        return;
                }
            }

            target = -Vector3.one;
            draged = false;
            scale = Vector3.one;
            canvas.sortingOrder = 0;
        }
        if (GameManager.InputActions.UI.RightClick.IsPressed())
        {
            CardManager.dragging = false;
            target = -Vector3.one;
            draged = false;
            scale = Vector3.one;
            canvas.sortingOrder = 0;
        }

        if (target != -Vector3.one)
        {
            MovingTransform.position = Vector3.MoveTowards(MovingTransform.position, target, Time.deltaTime * Mathf.Min(Vector3.Distance(MovingTransform.position, target), 500) * 10);
        }
        else
        {
            MovingTransform.position = Vector3.MoveTowards(MovingTransform.position, transform.position, Time.deltaTime * Mathf.Min(Vector3.Distance(MovingTransform.position, transform.position), 500) * 10);
        }

        MovingTransform.localScale = Vector3.MoveTowards(MovingTransform.localScale, scale, Time.deltaTime * 10);
    }
    //void drawpoint(Vector3 p, Color c)
    //{
    //    Debug.DrawRay(p, Vector3.right * 10, c);
    //    Debug.DrawRay(p, Vector3.up * 10, c);
    //}
}