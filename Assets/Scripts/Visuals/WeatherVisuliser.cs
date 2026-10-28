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
    [SerializeField] Sprite none;
    [SerializeField] Sprite regen;
    [SerializeField] Sprite nebel;
    [SerializeField] Sprite gewitter;
    [SerializeField] Sprite frost;
    [SerializeField] Sprite sturm;

    Sprite NextIcon;
    bool imageStae = false;

    public void Init(WetterArt w)
    {
        imageA.sprite = GetImage(w);
        WeatherManager.WetherChanged += ChangeImage;
    }
    private void ChangeImage(WetterArt w)
    {
        NextIcon = GetImage(w);
        animator.SetBool("state", imageStae = !imageStae);
    }
    Sprite GetImage(WetterArt w)
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
}
