using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    LineRenderer line;

    public float distance;
    [SerializeField] float noise = 0.1f;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }
    void Update()
    {
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

    public Vector3[] CreateLine()
    {
        float dist = Vector3.Distance(Vector3.zero, Vector3.forward * distance);

        // 5 points per meter
        int sections = Mathf.Max(1, Mathf.CeilToInt(dist * 5f));

        Vector3[] points = new Vector3[sections + 1];

        for (int i = 0; i <= sections; i++)
        {
            float t = (float)i / sections;
            points[i] = Vector3.Lerp(Vector3.zero, Vector3.forward * distance, t);
        }

        line.positionCount = sections;

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
