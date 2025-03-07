using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class testdata : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            GameObject p = GameObject.Find("testdeepseek");
            DataPlotter dp = p.GetComponent<DataPlotter>();
            List<float> Y = new List<float>();
            List<float> X = new List<float>();
            for (int i = 0; i < 100; ++i)
            {
                float x = 0.1f * (float)i;
                Y.Add(Mathf.Sin(x));
                X.Add(x);
            }
            dp.UpdateData(X,Y);
        }
    }
}
