#if UNITY_EDITOR
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeatherVisuliser))]
public class WetterForshau : Editor
{
    public override void OnInspectorGUI()
    {
        WeatherVisuliser v = target.GetComponent<WeatherVisuliser>();
        WeatherManager w = FindFirstObjectByType<WeatherManager>();
        base.OnInspectorGUI();

        foreach (WetterArt item in w.wetterAblauf)
        {
            GUILayout.Space(5);

            Texture2D texture = v.GetImage(item).texture;

            Rect rect = GUILayoutUtility.GetRect(194, 38);


            GUI.DrawTextureWithTexCoords(
                rect,
                texture,
                new Rect(0, 0, 1, 38f / 194f)
            );
        }
    }
}
#endif