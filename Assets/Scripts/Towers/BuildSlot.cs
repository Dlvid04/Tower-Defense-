using UnityEngine;

public class BuildSlot : MonoBehaviour
{
    [SerializeField] GameObject testTower;
    [SerializeField] float buildCooldown = 3f;

    GameObject currentTower;

    float canBuildAgainAt;

    public bool IsFree()
    {
        return currentTower == null;
    }

    public bool CanBuild()
    {
        if (IsFree() == false)
        {
            return false;
        }

        return Time.time >= canBuildAgainAt;
    }

    public void Build(GameObject towerPrefab)
    {
        if (IsFree() == false)
        {
            Debug.LogWarning(name + " is already taken.");
            return;
        }

        currentTower = Instantiate(towerPrefab, transform.position, transform.rotation, transform);
        Debug.Log("Create tower on: " + name);
    }

    public void Clear()
    {
        currentTower = null;
        canBuildAgainAt = Time.time + buildCooldown;
        Debug.Log(name + " is blocked for " + buildCooldown + " s");
    }

    [ContextMenu("Test: Create tower")]
    void TestBuild()
    {
        Build(testTower);
    }
}