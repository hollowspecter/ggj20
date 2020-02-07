using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voice : MonoBehaviour
{
    public static Dictionary<string, Voice> Registry = new Dictionary<string, Voice>();

    public string voiceName = "";
    public FMODUnity.StudioEventEmitter voiceEmitter;
    public Color voiceColor = Color.black;

    protected void Awake()
    {
        Registry.Add(voiceName, this);
    }

    public void Talk()
    {
        try
        {
            voiceEmitter?.Play();
        }
        catch (System.Exception e) { }
    }
}
