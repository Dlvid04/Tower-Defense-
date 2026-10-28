using System.Threading;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WetterArt curentWeather = WetterArt.None;
    public static WetterArt nextWeather = WetterArt.None;

    float timer = 0;

    [SerializeField] float interval = 40;

    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = interval;
            curentWeather = nextWeather;

            for (int i = 0; i < 5; i++)
            {
                switch (Random.Range(0, 2 +1))
                {
                    case 0:
                        nextWeather = WetterArt.Regen;
                        break;
                    case 1:
                        nextWeather = WetterArt.Nebel;
                        break;
                    case 2:
                        nextWeather = WetterArt.Gewitter;
                        break;
                    //case 0:
                    //    nextWeather = WetterArt.Frost;
                    //    break;
                    //case 0:
                    //    nextWeather = WetterArt.Sturm;
                    //    break;
                    default:
                        nextWeather = WetterArt.None;
                        break;
                }
                if (curentWeather != nextWeather)
                    break;
            }
            if (curentWeather == nextWeather)
            {
                nextWeather = WetterArt.None;
            }
        }
    }
}

public enum WetterArt { None, Regen, Nebel, Gewitter, Frost, Sturm }