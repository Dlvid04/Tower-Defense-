using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages timers and the UI of them.
/// </summary>
public class TimeManager : MonoBehaviour
{
    public static event Action MutUpdate;

    #region statics
    public static TimeManager timeManager;
    public static float RoundTimer
    {
        get { return roundTimer; }
        set
        {
            roundTimer = Mathf.Max(value, 0);
            //roundTimer = (value < -1 ? -1 : (value <= 0 ? 0 : value));
        }
    }
    static float mutTimer = 0;
    public static float MutTimer
    {
        get { return mutTimer; }
        set
        {
            mutTimer = (value < -1 ? -1 : (value <= 0 ? 0 : value));
        }
    }

    static float roundTimer = 0;
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
        MutTimer = mutInterval;

        TimerActive = true;

    }


    void Awake()
    {
        timeManager = this;
    }

    void Update()
    {
        if (!GameManager.GameIsRunning)
            return;

        updateTime();
        updateMut();

        if (GameManager.GropsOnMap >= 4)
        {
            CardManager.timer = CardManager.timerMax;
        }
        else if(CardManager.timer > 0)
        {
            CardManager.timer -= Time.deltaTime;
        }
    }
    void updateTime()
    {
        if (!TimerActive)
            return;

        RoundTimer -= (Time.deltaTime / 60);
        if (RoundTimer > 0.015f)
        {
            float minuts = Mathf.Floor(RoundTimer);
            TimerText.text = format(minuts) + ":" + format(Mathf.Floor((RoundTimer - minuts) * 60));
        }
        else
        {
            TimerActive = false;
            TimerText.text = "00:00";

            Debug.Log("Round End");

            GameManager.Instance.GameEnd(EndType.Timeout);
        }
    }

    public void SetMutDisplay()
    {
        mutIntervalVisualiser.gameObject.SetActive(CardManager.MUT < 10 && TimerActive);
    }

    void updateMut()
    {
        if (TimerActive)
        {
            if (CardManager.MUT < 10)
            {
                MutTimer -= Time.deltaTime;
                if (MutTimer <= 0)
                {
                    MutUpdate.Invoke();
                    MutTimer += mutInterval;
                }
                SetMutDisplay();
                mutIntervalVisualiser.value = 1 - (MutTimer / mutInterval);
            }
            else
            {
                MutTimer = mutInterval;
                SetMutDisplay();
            }
        }
        else
        {
            SetMutDisplay();
        }
    }

    string format(float f)
    {
        return (f < 10 ? "0"+f : f.ToString());
    }
}
