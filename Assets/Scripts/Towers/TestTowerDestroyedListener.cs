using UnityEngine;

public class TestTowerDestroyedListener : MonoBehaviour
{
    public int rewardForPlayer = 5;

    void OnEnable()
    {
        Tower.TowerDestroyed += HandleTowerDestroyed;
    }

    void OnDisable()
    {
        Tower.TowerDestroyed -= HandleTowerDestroyed;
    }

    void HandleTowerDestroyed(Tower tower)
    {
        Debug.Log("Event angekommen: " + tower.data.towerName + " zerstört. Spieler bekommt +" + rewardForPlayer + " Mut.");
    }
}