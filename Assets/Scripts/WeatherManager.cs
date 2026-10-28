using System;
using System.Threading;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static event Action<WetterArt> WetherChanged;

    public static WetterArt curentWeather = WetterArt.None;
    public static WetterArt nextWeather = WetterArt.None;

    float timer = 0;

    [SerializeField] float interval = 40;

    void Start()
    {
        timer = interval;
        nextWeather = TryGetNext();

        FindFirstObjectByType<WeatherVisuliser>().Init(nextWeather);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = interval;
            curentWeather = nextWeather;

            nextWeather = TryGetNext();

            WetherChanged.Invoke(curentWeather);
        }
    }
    WetterArt TryGetNext()
    {
        WetterArt w = WetterArt.None;

        for (int i = 0; i < 5; i++)
        {
            switch (UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    w = WetterArt.Regen;
                    break;
                case 1:
                    w = WetterArt.Nebel;
                    break;
                case 2:
                    w = WetterArt.Gewitter;
                    break;
                //case 0:
                //    w = WetterArt.Frost;
                //    break;
                //case 0:
                //    w = WetterArt.Sturm;
                //    break;
            }

            if (w != nextWeather)
                return w;
        }

        return nextWeather == WetterArt.None
            ? WetterArt.Regen
            : WetterArt.None;
    }
}

public enum WetterArt { None, Regen, Nebel, Gewitter, Frost, Sturm }