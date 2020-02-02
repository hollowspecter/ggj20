using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireDisaster : MonoBehaviour
{
    public GameObject firePrefab;
    public static List<Fire> activeFires = new List<Fire>();
    public SceneData sceneData;
    public float panicPerFirePerSecond = 0.01f;

    private void Update()
    {
        sceneData.disasterMeter = activeFires.Count * panicPerFirePerSecond;
    }

    public void KillAllFires()
    {
        //for (int i = 0; i < activeFires.Count; i++)
        //{
        //    Destroy(activeFires[i].gameObject);
        //}

        var fires = FindObjectsOfType<Fire>();
        for (int i = 0; i < fires.Length; i++)
        {
            Destroy(fires[i].gameObject);
        }
    }
}
