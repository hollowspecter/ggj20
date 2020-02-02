using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDisaster : MonoBehaviour
{
    public static List<Fire> activeFires = new List<Fire>();
    public SceneData sceneData;
    public float panicPerFirePerSecond = 0.01f;

    private void Update()
    {
        sceneData.disasterMeter = activeFires.Count * panicPerFirePerSecond;
    }

    public void KillAllFires()
    {
        foreach (var item in activeFires)
        {
            Destroy(item.gameObject);
        }
    }
}
