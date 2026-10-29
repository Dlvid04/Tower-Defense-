using System;
using UnityEngine;
using UnityEngine.UI;

public class WeatherVisuliser : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Image imageA;
    [SerializeField] Image imageB;

    [Space(5)]
    [SerializeField] Animator animator;
    
    [Header("Images")]
    public Sprite none;
    public Sprite regen;
    public Sprite nebel;
    public Sprite gewitter;
    public Sprite frost;
    public Sprite sturm;

    Sprite NextIcon;
    bool imageStae = true;

    void Awake()
    {
        animator.SetBool("state", imageStae);
    }

    public void Init(WetterArt w)
    {
        imageA.sprite = none;
        imageB.sprite = GetImage(w);

        WeatherManager.NextWetherChanged += ChangeImage;
    }
    private void ChangeImage(WetterArt w)
    {
        NextIcon = GetImage(w);
        animator.SetBool("state", imageStae = !imageStae);
    }
    public Sprite GetImage(WetterArt w)
    {
        switch (w)
        {
            default:
                return none;
            case WetterArt.Regen:
                return regen;
            case WetterArt.Nebel:
                return nebel;
            case WetterArt.Gewitter:
                return gewitter;
            case WetterArt.Frost:
                return frost;
            case WetterArt.Sturm:
                return sturm;
        }
    }


    public void UpdateImage_A()
    {
        imageA.sprite = NextIcon;
    }

    public void UpdateImage_B()
    {
        imageB.sprite = NextIcon;
    }

    void OnDestroy()
    {
        WeatherManager.WetherChanged -= ChangeImage;
    }
}
