using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(CardVisuliser))]
public class Card : MonoBehaviour
{
    public Transform MovingTransform;

    RectTransform rt;
    CardVisuliser visuliser;
    CardData data;

    Vector3 target = -Vector3.one;
    Vector3 scale = Vector3.one;
    bool draged = false;

    public void Init(CardData cd)
    {

        visuliser = GetComponent<CardVisuliser>();
        rt = GetComponent<RectTransform>();

        data = cd;
        UpdateUI();
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
        for (int i = 0; i < data.troopCount; i++)
        {
            Instantiate(data.prefab, position, Quaternion.identity);
        }
    }
    public float sdf;
    void Update()
    {
        if (GameManager.InputActions.UI.Click.IsPressed())
        {
            Vector2 mp = GameManager.InputActions.UI.Point.ReadValue<Vector2>();

            Vector2 min = new Vector2(rt.position.x - (rt.rect.width * rt.lossyScale.x / 2), rt.position.y - (rt.rect.height * rt.lossyScale.y / 2));
            Vector2 max = new Vector2(rt.position.x + (rt.rect.width * rt.lossyScale.x / 2), rt.position.y + (rt.rect.height * rt.lossyScale.y / 2));

            if (!draged && (mp.x > min.x && mp.x < max.x && mp.y > min.y && mp.y < max.y))
            {
                draged = true;
                scale = Vector3.one * 0.5f;
            }

            if (draged)
            {
                target = mp;
            }
        }
        else if(draged)
        {
            Vector2 mp = GameManager.InputActions.UI.Point.ReadValue<Vector2>();

            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(mp), out hit))
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green, 5);
            }

            target = transform.position;
            draged = false;
            scale = Vector3.one;
        }

        if(target != -Vector3.one)
        MovingTransform.position = Vector3.MoveTowards(MovingTransform.position, target, Time.deltaTime * Mathf.Min(Vector3.Distance(MovingTransform.position, target), 500) * 10);
        MovingTransform.localScale = Vector3.MoveTowards(MovingTransform.localScale, scale, Time.deltaTime * 10);
    }
    //void drawpoint(Vector3 p, Color c)
    //{
    //    Debug.DrawRay(p, Vector3.right * 10, c);
    //    Debug.DrawRay(p, Vector3.up * 10, c);
    //}
}