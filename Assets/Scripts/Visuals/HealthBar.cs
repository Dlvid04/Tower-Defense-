using UnityEngine;
using UnityEngine.UI;

public class HealthBar : Billboard
{
    [SerializeField] Image fillValue;

    public float Value
    {
        set
        {
            fillValue.fillAmount = value;
        }
    }
}
