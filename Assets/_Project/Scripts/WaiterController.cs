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

    public FMODUnity.StudioEventEmitter scooterEvent;

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
        scooterEvent.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!navAgent.pathPending && !hasArrived)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || Mathf.Approximately(navAgent.velocity.sqrMagnitude, 0.0f))
                {
                    ArriveAtWaypoint();
                    scooterEvent.Stop();
                }
            }
        }
    }

    void ArriveAtWaypoint()
    {
        hasArrived = true;
        switch (currentWaypoint)
        {
            case WaiterWaypoint.Kitchen:
                foreach(var oList in Orders.Values)
                {
                    if (oList.Count > 0)
                    {
                        GoToWaypoint(WaiterWaypoint.PlayerTable);
                    }
                }
                break;
            case WaiterWaypoint.PlayerTable:
                StartCoroutine("PlaceOrders");
                break;
            default:
                break;
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
            Orders[OrderType].Add(spawn);
            GoToWaypoint(WaiterWaypoint.PlayerTable);
            scooterEvent.Play();
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

                GO = Instantiate<GameObject>(GO, _transform.position + Vector3.up, _transform.rotation);
                GO.transform.localScale = _transform.lossyScale;
                var wobble = GO.GetComponent<WobbleIt>();
                wobble?.Wobble();
                yield return new WaitForSeconds(Random.Range(0.3f,0.6f));
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
