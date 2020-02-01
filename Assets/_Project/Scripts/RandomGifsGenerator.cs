using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGifsGenerator : MonoBehaviour
{
    public Animation[] randomGifs;
    private bool gifIsPlaying = false;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < randomGifs.Length; i++)
        {
            randomGifs[i].Play();
            gifIsPlaying = true;
            if(gifIsPlaying)
            {
                randomGifs[i].PlayQueued(randomGifs[i].name, QueueMode.CompleteOthers, PlayMode.StopAll);
            }
            return;
        }
    }
}
