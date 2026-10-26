using UnityEditor;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class TroopAgent : MonoBehaviour
{
    /// <summary>
    /// Damit mehere truppen von einer karte nur ein leben abziehen
    /// </summary>
    int groupID = 0;
    public int GroupID
    {
        get { return groupID; }
    }

    public Troop troop;
    NavMeshAgent agent;
    public virtual void Init(int groupID)
    {
        agent = GetComponent<NavMeshAgent>();


        this.groupID = groupID;
    }

    void Update()
    {
        TroopUpdate();
    }


    /// <summary>
    /// Setup von variablen und mehr
    /// </summary>
    public virtual void TroopUpdate()
    {

    }
}