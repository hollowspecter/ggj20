using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WaiterController : MonoBehaviour
{

    private NavMeshAgent navAgent;
    bool hasArrived = false;


    [SerializeField]
    GameObject TableWaypoint;
    [SerializeField]
    GameObject KitchenWaypoint;


    [SerializeField]
    List<GameObject> Drinks;

    [SerializeField]
    List<GameObject> Foods;

    [SerializeField]
    List<GameObject> LeftCutlery;

    [SerializeField]
    List<GameObject> RightCutlery;

    Dictionary<string, List<GameObject>> Orderables = new Dictionary<string, List<GameObject>>();


    List<KeyValuePair<string,Transform>> Orders;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        GoToKitchen();
        Orders = new List<KeyValuePair<string, Transform>>();

        Orderables.Add("Drink", new List<GameObject>());
        foreach(var d in Drinks)
        {
            Orderables["Drink"].Add(d);
        }

        Orderables.Add("Food", new List<GameObject>());
        foreach (var f in Foods)
        {
            Orderables["Food"].Add(f);
        }

        Orderables.Add("CutleryLeft", new List<GameObject>());
        foreach (var f in LeftCutlery)
        {
            Orderables["CutleryLeft"].Add(f);
        }

        Orderables.Add("CutleryRight", new List<GameObject>());
        foreach (var f in RightCutlery)
        {
            Orderables["CutleryRight"].Add(f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasArrived && (transform.position - TableWaypoint.transform.position).sqrMagnitude <= 2.0f)
        {
            ArriveAtTable();
        }
        if (!hasArrived && (transform.position - KitchenWaypoint.transform.position).sqrMagnitude <= 2.0f)
        {
            GoToTable();
        }
    }
    void ArriveAtTable()
    {
        hasArrived = true;
        StartCoroutine("PlaceOrders");
    }

    void GoToTable()
    {
        navAgent.SetDestination(TableWaypoint.transform.position);
        hasArrived = false;
    }

    void GoToKitchen()
    {
        navAgent.SetDestination(KitchenWaypoint.transform.position);
        hasArrived = false;
    }

    public void OrderItem(string OrderType, Transform spawn)
    {
        if (Orderables.ContainsKey(OrderType))
        {
            Orders.Add(new KeyValuePair<string, Transform>(OrderType, spawn));
            GoToTable();
        }
    }

    IEnumerator PlaceOrders()
    {
        foreach (var order in Orders)
        {
            var type = order.Key;
            if (Orderables.ContainsKey(type))
            {
                //Gets a random item of the list of orderables
                int max = Orderables[type].Count;
                GameObject pref = Orderables[type][Random.Range(0, max-1)];
                Transform spawnTransform = order.Value;
                var GO = Instantiate<GameObject>(pref, spawnTransform.position, spawnTransform.rotation);
                GO.transform.localScale = spawnTransform.lossyScale;
                yield return new WaitForSeconds(0.5f);
            }
        }
        Orders.Clear();
        GoToKitchen();
    }
}
