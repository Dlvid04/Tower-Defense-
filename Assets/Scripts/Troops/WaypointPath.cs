using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public Transform[] points;

    //[ContextMenu("Collect Children")]
    //void Collect()
    //{
    //    points = new Transform[transform.childCount];
    //    for (int i = 0; i < points.Length; i++) points[i] = transform.GetChild(i);
    //}

    void OnDrawGizmos()
    {
        if (points == null) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null) continue;
            Gizmos.DrawSphere(points[i].position, 0.3f);
            if (i > 0 && points[i - 1] != null)
                Gizmos.DrawLine(points[i - 1].position, points[i].position);
        }
    }

    static float Flat(Vector3 a, Vector3 b)
    {
        a.y = 0f; b.y = 0f;
        return Vector3.Distance(a, b);
    }

    public float TotalLength
    {
        get
        {
            float len = 0f;
            for (int i = 1; i < points.Length; i++) len += Flat(points[i - 1].position, points[i].position);
            return len;
        }
    }

    public float ProgressAt(Vector3 pos)
    {
        if (points == null || points.Length < 2) return 0f;
        pos.y = 0f;
        float bestSqr = float.MaxValue, bestProgress = 0f, cum = 0f;
        for (int i = 1; i < points.Length; i++)
        {
            Vector3 a = points[i - 1].position; a.y = 0f;
            Vector3 b = points[i].position; b.y = 0f;
            Vector3 ab = b - a;
            float len = ab.magnitude;
            float t = len > 0.0001f ? Mathf.Clamp01(Vector3.Dot(pos - a, ab) / (len * len)) : 0f;
            float sqr = (pos - (a + ab * t)).sqrMagnitude;
            if (sqr < bestSqr) { bestSqr = sqr; bestProgress = cum + t * len; }
            cum += len;
        }
        return bestProgress;
    }

    public Vector3 PointAt(float progress, out int nextIndex)
    {
        float cum = 0f;
        for (int i = 1; i < points.Length; i++)
        {
            Vector3 a = points[i - 1].position;
            Vector3 b = points[i].position;
            float len = Flat(a, b);
            if (progress <= cum + len || i == points.Length - 1)
            {
                float t = len > 0.0001f ? Mathf.Clamp01((progress - cum) / len) : 0f;
                nextIndex = i;
                return Vector3.Lerp(a, b, t);
            }
            cum += len;
        }
        nextIndex = 0;
        return points[0].position;
    }
}