using System;
using UnityEngine;
using Valve.VR;

public class load_script : MonoBehaviour
{
    public void DoSomething(SteamVR_Behaviour_Boolean b, SteamVR_Input_Sources s, Boolean bo)
    {
        print(b);
        print(s);
        print(bo);

    }
    public enum LoadType
    {
        Force,
        Torque,
        Load,
        Joint1,
        Joint2,
        Console,
    }

    public LoadType loadType;
    public float param;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
