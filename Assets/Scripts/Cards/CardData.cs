using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    [Header("Visuals")]
    //public string cardName = "Card Name";
    public Sprite image;

    [Header("Data")]
    [Range(0, 10)] public int placeCost = 1;
    [Header("Troop")]
    public TroopAgent agent;
    [Min(1)] public int troopCount = 1;
}