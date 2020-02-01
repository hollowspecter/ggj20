using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WaiterController : MonoBehaviour
{

    public enum WaiterWaypoint
    {
        None,
        Kitchen,
        PlayerTable
    }

    private NavMeshAgent navAgent;
    bool hasArrived = true;


    [SerializeField]
    GameObject TableWaypoint;
    [SerializeField]
    GameObject KitchenWaypoint;

    WaiterWaypoint currentWaypoint;




    [SerializeField]
    List<GameObject> Drinks;

    [SerializeField]
    List<GameObject> Foods;

    [SerializeField]
    List<GameObject> LeftCutlery;

    [SerializeField]
    List<GameObject> RightCutlery;

    Dictionary<string, List<GameObject>> Orderables = new Dictionary<string, List<GameObject>>();


    Dictionary<string, List<Transform>> Orders = new Dictionary<string, List<Transform>>();
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        GoToWaypoint(WaiterWaypoint.Kitchen);

        Orderables.Add("Drink", new List<GameObject>());
        Orders.Add("Drink", new List<Transform>());
        foreach (var d in Drinks)
        {
            Orderables["Drink"].Add(d);
        }

        Orderables.Add("Food", new List<GameObject>());
        Orders.Add("Food", new List<Transform>());
        foreach (var f in Foods)
        {
            Orderables["Food"].Add(f);
        }

        Orderables.Add("CutleryLeft", new List<GameObject>());
        Orders.Add("CutleryLeft", new List<Transform>());
        foreach (var leftCutlery in LeftCutlery)
        {
            Orderables["CutleryLeft"].Add(leftCutlery);
        }

        Orderables.Add("CutleryRight", new List<GameObject>());
        Orders.Add("CutleryRight", new List<Transform>());
        foreach (var rightCutlery in RightCutlery)
        {
            Orderables["CutleryRight"].Add(rightCutlery);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!navAgent.pathPending)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || Mathf.Approximately(navAgent.velocity.sqrMagnitude, 0.0f))
                {
                    switch (currentWaypoint)
                    {
                        case WaiterWaypoint.Kitchen:
                            hasArrived = true;
                            break;
                        case WaiterWaypoint.PlayerTable:
                            ArriveAtTable();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }


    void ArriveAtTable()
    {
        if (!hasArrived)
        {
            hasArrived = true;
            StartCoroutine("PlaceOrders");
        }
    }

    void GoToWaypoint(WaiterWaypoint _waypoint)
    {
        if (_waypoint == currentWaypoint || !hasArrived)
        {
            return;
        }
        hasArrived = false;
        switch (_waypoint)
        {
            case WaiterWaypoint.Kitchen:
                navAgent.SetDestination(KitchenWaypoint.transform.position);
                break;
            case WaiterWaypoint.PlayerTable:
                navAgent.SetDestination(TableWaypoint.transform.position);
                break;
            default:
                break;
        }
        currentWaypoint = _waypoint;
    }

    public void OrderItem(string OrderType, Transform spawn)
    {
        if (!Orders[OrderType].Contains(spawn))
        {
            Debug.Log("NEW ORDER");
            Orders[OrderType].Add(spawn);
            GoToWaypoint(WaiterWaypoint.PlayerTable);
        }
    }

    IEnumerator PlaceOrders()
    {
        //Durch alle Bestelllisten durchgehen
        foreach (var orderType in Orders.Keys)
        {
            foreach (Transform _transform in Orders[orderType])
            {
                int max = Orderables[orderType].Count;
                int random = Random.Range(0, max);
                GameObject GO = Orderables[orderType][random];

                GO = Instantiate<GameObject>(GO, _transform.position, _transform.rotation);
                GO.transform.localScale = _transform.lossyScale;
                yield return new WaitForSeconds(0.5f);
            }
        }
        foreach (var orderList in Orders.Values)
        {
            orderList.Clear();
        }
        yield return new WaitForSeconds(3.0f);
        GoToWaypoint(WaiterWaypoint.Kitchen);
    }
}
