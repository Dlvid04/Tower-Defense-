using UnityEngine;

[System.Serializable]
public class Wallet
{
    [SerializeField] int max = 10;

    [SerializeField] int mut;
    public int MUT
    {
        get { return mut; }
    }

    public Wallet(int value = 0, bool useTimeManager = true)
    {
        mut = value;

        if (useTimeManager)
        {
            TimeManager.MutUpdate += ChangeMutUpdate;
        }
    }

    public int Max
    {
        get { return max; }
    }

    // not used in CardManager.cs to reduce instance calls
    public bool Buy(int cost)
    {
        return cost <= MUT;
    }

    void ChangeMutUpdate()
    {
        ChangeMut(1);
    }

    public bool ChangeMut(int value)
    {
        if ((value > 0 && MUT >= max) || (value < 0 && MUT <= 0) || value == 0)
        {
            return false;
        }

        mut = Mathf.Clamp(mut + value, 0, max);
        return true;
    }

    public void Unsubscribe()
    {
        TimeManager.MutUpdate -= ChangeMutUpdate;
    }
}
