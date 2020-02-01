using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WaiterController : MonoBehaviour
{

    private NavMeshAgent navAgent;

    [SerializeField]
    GameObject TableWaypoint;
    [SerializeField]
    GameObject KitchenWaypoint;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        GoToKitchen();
    }

    // Update is called once per frame
    void Update()
    {
        if((transform.position - TableWaypoint.transform.position).sqrMagnitude <= 2.0f)
        {
            GoToKitchen();
        }
        if ((transform.position - KitchenWaypoint.transform.position).sqrMagnitude <= 2.0f)
        {
            GoToTable();
        }
    }

    void GoToTable()
    {
        navAgent.SetDestination(TableWaypoint.transform.position);
    }

    void GoToKitchen()
    {
        navAgent.SetDestination(KitchenWaypoint.transform.position);
    }
}
