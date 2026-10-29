using System;
using System.Threading;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public WetterArt[] wetterAblauf = new WetterArt[] { WetterArt.None, WetterArt.Nebel, WetterArt.None, WetterArt.Regen, WetterArt.None, WetterArt.Gewitter };
    public static event Action<WetterArt> WetherChanged;
    public static event Action<WetterArt> NextWetherChanged;

    public static WetterArt curentWeather = WetterArt.None;
    public static WetterArt nextWeather = WetterArt.None;

    float timer = 0;

    int I = 0;

    [SerializeField] float interval = 40;

    void Start()
    {
        curentWeather = WetterArt.None;

        timer = interval;
        nextWeather = TryGetNext();

        FindFirstObjectByType<WeatherVisuliser>().Init(nextWeather);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.GameIsRunning)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = interval;
            curentWeather = nextWeather;

            nextWeather = TryGetNext();

            WetherChanged.Invoke(curentWeather);
            NextWetherChanged.Invoke(nextWeather);
        }
    }
    WetterArt TryGetNext()
    {
        I++;
        if (I >= wetterAblauf.Length)
        {
            I = 0;
        }

        return wetterAblauf[I];
    }
}

/// <summary>
/// Frost und Sturm werden nicht im MVP genutzt
/// </summary>
public enum WetterArt { None, Regen, Nebel, Gewitter, Frost, Sturm }