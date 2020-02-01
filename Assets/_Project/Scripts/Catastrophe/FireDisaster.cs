using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDisaster : MonoBehaviour
{
    public static int numberOfFires = 0;
    public SceneData sceneData;
    public float panicPerFirePerSecond = 0.01f;

    private void Update()
    {
        sceneData.disasterMeter = numberOfFires * panicPerFirePerSecond;
    }
}
