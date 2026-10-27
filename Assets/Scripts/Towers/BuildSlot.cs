using UnityEngine;

public class BuildSlot : MonoBehaviour
{
    [SerializeField] GameObject testTower;

    GameObject currentTower;

    public bool IsFree()
    {
        return currentTower == null;
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
    }

    [ContextMenu("Test: Create tower")]
    void TestBuild()
    {
        Build(testTower);
    }
}
