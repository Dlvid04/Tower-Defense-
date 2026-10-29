using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    LineRenderer line;

    public float distance;
    [SerializeField] float noise = 0.1f;

    bool followChain;
    Transform chainStart;
    Transform chainEnd;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (followChain)
        {
            DrawChain();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            distance = hit.distance;
            line.SetPositions(AddNoiseLine(CreateLine()));
        }
        else
        {
            distance = 25;
            line.SetPositions(AddNoiseLine(CreateLine()));
        }
    }

    public void SetChain(Transform start, Transform end)
    {
        line = GetComponent<LineRenderer>();
        chainStart = start;
        chainEnd = end;
        followChain = true;

        DrawChain();
    }

    void DrawChain()
    {
        if (chainStart != null && chainEnd != null)
        {
            Vector3 direction = chainEnd.position - chainStart.position;
            if (direction != Vector3.zero)
            {
                transform.position = chainStart.position;
                transform.rotation = Quaternion.LookRotation(direction);
                distance = direction.magnitude;
            }
        }

        line.SetPositions(AddNoiseLine(CreateLine()));
    }

    public Vector3[] CreateLine()
    {
        float dist = Vector3.Distance(Vector3.zero, Vector3.forward * distance);
        int sections = Mathf.Max(1, Mathf.CeilToInt(dist * 5f));

        Vector3[] points = new Vector3[sections + 1];

        for (int i = 0; i <= sections; i++)
        {
            float t = (float)i / sections;
            points[i] = Vector3.Lerp(Vector3.zero, Vector3.forward * distance, t);
        }

        line.positionCount = points.Length;

        return points;
    }
    public Vector3[] AddNoiseLine(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = points[i] + new Vector3(Random.Range(-noise, noise), Random.Range(-noise, noise), 0);
        }

        return points;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

#endif
}