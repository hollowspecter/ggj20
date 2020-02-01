using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSide : MonoBehaviour
{
    Dictionary<string, Transform> positions = new Dictionary<string, Transform>();

    [SerializeField]
    List<string> tags;

    [SerializeField]
    List<Transform> transforms;

    [SerializeField]
    WaiterController waiter;


    private void Start()
    {
        if(tags.Count != transforms.Count)
        {
            Debug.LogError("Non matching number of positions and tags!");
        }
        else
        {
            for(int i = 0; i < tags.Count; ++i)
            {
                positions.Add(tags[i], transforms[i]);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(positions.ContainsKey(other.tag))
        {
            Debug.Log("Yes"+other.name);
            waiter.OrderItem(other.tag, positions[other.tag]);
        }
        else
        {
            Debug.Log("No"+other.name);
        }
    }
}
