using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages timers and the UI of them.
/// </summary>
public class TimeManager : MonoBehaviour
{
    #region statics
    public static TimeManager timeManager;
    public static float RoundTimer
    {
        get { return roundTimer; }
        set
        {
            roundTimer = (value < -1 ? -1 : (value <= 0 ? 0 : value));
        }
    }
    static float mutTimer = -1;
    public static float MutTimer
    {
        get { return mutTimer; }
        set
        {
            mutTimer = (value < -1 ? -1 : (value <= 0 ? 0 : value));
        }
    }
    static float roundTimer = -1;
    static bool TimerActive = true;

    #endregion

    [Header("Settings")]
    [Tooltip("In Minuts")] public float roundLength = 4;
    [Tooltip("In seconds")] public float mutInterval = 3;

    [Header("UI")]
    [SerializeField] TMP_Text TimerText;
    [SerializeField] Slider mutIntervalVisualiser;
    public static void StartRoundTimer()
    {
        timeManager.TimeSetup();
    }
    public void TimeSetup()
    {
        RoundTimer = roundLength;
        TimerActive = true;

        MutTimer = mutInterval;
    }


    void Awake()
    {
        timeManager = this;
    }

    void Update()
    {
        updateTime();
        updateMut();

    }
    void updateTime()
    {
        if (TimerActive)
        {
            RoundTimer -= (Time.deltaTime / 60);

            float minuts = Mathf.Floor(RoundTimer);
            TimerText.text = format(minuts) + ":" + format(Mathf.Floor((RoundTimer - minuts) * 60));
        }
        else
        {
            TimerActive = false;
            TimerText.text = "00:00";

            Debug.Log("Round End");

            GameManager.instance.GameEnd(EndType.Timeout);
        }
    }
    void updateMut()
    {
        if (CardManager.MUT < 10)
        {
            MutTimer -= Time.deltaTime;
            if (MutTimer <= 0)
            {
                CardManager.Instance.ChageMUT(1);
                MutTimer += mutInterval;
            }
            mutIntervalVisualiser.gameObject.SetActive(true);
            mutIntervalVisualiser.value = 1 - (MutTimer / mutInterval);
        }
        else
        {
            mutIntervalVisualiser.gameObject.SetActive(false);
        }
    }

    string format(float f)
    {
        return (f < 10 ? "0"+f : f.ToString());
    }
}
