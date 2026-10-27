using UnityEngine;
using UnityEngine.InputSystem;

public class TroopTestSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public Troop troop;
        [Min(1)] public int count = 1;   
    }

    public Transform startPoint;
    public SpawnEntry[] entries;

    int nextGroupID = 1;

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        for (int i = 0; i < entries.Length && i < 4; i++)
        {
            if (keyboard[Key.Digit1 + i].wasPressedThisFrame)
            {
                Spawn(entries[i]);
            }
        }
    }

    void Spawn(SpawnEntry entry)
    {
        if (entry.troop == null || entry.troop.Prefab == null)
        {
            Debug.LogError("Troop oder Prefab fehlt im TroopTestSpawner!");
            return;
        }

        for (int i = 0; i < entry.count; i++)
        {
            GameObject g = Instantiate(entry.troop.Prefab, startPoint.position, startPoint.rotation);
            g.GetComponent<TroopAgent>().Init(nextGroupID);
        }
        nextGroupID = nextGroupID + 1;
    }
}