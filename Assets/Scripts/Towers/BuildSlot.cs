using UnityEngine;

public class BuildSlot : MonoBehaviour
{
    [SerializeField] GameObject testTower;
    [SerializeField] float buildCooldown = 3f;
    [SerializeField] GameObject slotMesh;
    [SerializeField] GameObject buildEffect;

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
        if (slotMesh != null)
        {
            slotMesh.SetActive(false);
        }

        if (buildEffect != null)
        {
            Instantiate(buildEffect, transform.position, Quaternion.identity);
        }
        Debug.Log("Create tower on: " + name);
    }

    public void Clear()
    {
        currentTower = null;
        if (slotMesh != null)
        {
            slotMesh.SetActive(true);
        }
        canBuildAgainAt = Time.time + buildCooldown;
        Debug.Log(name + " is blocked for " + buildCooldown + " s");
    }

    [ContextMenu("Test: Create tower")]
    void TestBuild()
    {
        Build(testTower);
    }
}