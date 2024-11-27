using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class balka_solver : MonoBehaviour
{
    List <load_data> loads;
    
    struct load_data
    {
        public load_data(float a, load_script b)
        {
            x = a;
            ls = b;
        }

        public float x;
        public load_script ls;
    }

    void Start()
    {
        loads = new List <load_data> ();
    }

    void ProcessChildren()
    {
        loads.Clear ();
        var num = transform.childCount;
        for (int i = 0; i < num; i++)
        {
            string s;
            var child = transform.GetChild(i);
            if (child != null)
            {
                var c = child.localPosition.x;
                
                var ls = child.gameObject.GetComponent<load_script>();
                loads.Add(new load_data(c,ls));
            }
        }

        loads=loads.OrderBy(a=>a.x).ToList();
        
        foreach (var item in loads)
        {
            print(string.Format("x={0},type= {2}, param={1}", item.x, item.ls.param, item.ls.loadType.ToString()));
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            ProcessChildren();
        }
        
    }
}
