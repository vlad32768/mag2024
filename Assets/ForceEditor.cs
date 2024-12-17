using System;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using TMPro;

public class ForceEditor : MonoBehaviour
{
    public load_script ls;
    public bool picked_up = false;
    public float delta = 10;
    public TMP_Text force_text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ls = GetComponent<load_script>();
        force_text.text = ls.param.ToString();
    }

    public void On_Pickup()
    {
        picked_up=true;
    }
    public void On_Detach()
    {
        picked_up = false;
    }


    public void pressUp(SteamVR_Behaviour_Boolean sbb, SteamVR_Input_Sources sis, Boolean b)
    {
        if (picked_up)
        {
            ls.param += delta;
            force_text.text = ls.param.ToString();
        }
    }

    public void pressDown(SteamVR_Behaviour_Boolean sbb, SteamVR_Input_Sources sis, Boolean b)
    {
        if (picked_up)
        {
            ls.param -= delta;
            force_text.text = ls.param.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
