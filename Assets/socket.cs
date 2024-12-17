using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class socket : MonoBehaviour
{
    public bool inside;
    public Collider coll;
    public Transform t;
    public bool is_load;
    public float xc;
    public bool is_joint;
    public Vector3 debug;
    public Vector3 debug2;
    //public float zr;
    public string tagToConnect; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //print("Stay");
        if (other.gameObject.transform.parent != transform)
        {
            t = other.gameObject.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        inside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inside= false;
    }

    public void DisableChildColliders()
    {
        for (int i = 0;i<transform.childCount;i++) {
            print(transform.GetChild(i).name);
            transform.GetChild(i).GetComponent<Collider>().enabled = false;
        }
    }
    public void EnableChildColliders()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            print(transform.GetChild(i).name);
            transform.GetChild(i).GetComponent<Collider>().enabled = true;
        }
    }
        public void connect()
    {
        print("Detached!");
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (inside && t.gameObject.tag == tagToConnect)
        {
            //Transform t = transform;
            var save_rot = transform.rotation;
            xc = transform.position.x - t.position.x;

            var dotp = Vector3.Dot(Vector3.up, transform.up); // Load gameobject orientation * global orientation!
            debug.x = dotp;
            var dotp2 = Vector3.Dot(new Vector3(1,0,0), transform.forward);

            transform.SetPositionAndRotation(t.position, t.rotation);

            //transform.position = t.position;

            rigidbody.isKinematic = true;
            coll.isTrigger = true;
            
            if (is_load)
            {
                var angles = save_rot.eulerAngles;
                print("Do" + angles.x);
                if (dotp < 0) angles.x = 0; else angles.x = 180;
                if (dotp2 > 0) angles.y = angles.x; else angles.y = 180-angles.x;
                //angles.y = 0;
                angles.z = 0;
                print("posle" + angles.x);
                transform.Translate(new Vector3(0, 0, xc));
                if (!is_joint) transform.Rotate(angles);
            }
            //else transform.SetPositionAndRotation(t.position, t.rotation);
            transform.SetParent(t);
            //CallCalcFunction();
            
        }
        else
        {
            rigidbody.isKinematic = false;
            coll.isTrigger = false;
        }
        EnableChildColliders();
        CallCalcFunction();
    }


    public void CallCalcFunction()
    {
        var w = GameObject.FindGameObjectWithTag("Wall");
        if (w != null) 
            {
                var b = w.transform.GetChild(0);
                if (b != null)
                {
                    var solver = b.gameObject.GetComponent<balka_solver>();
                    solver.Do_Solve();
                }

            }
    }

    // Update is called once per frame
    void Update()
    {
        Collider collider = GetComponent<Collider>();
        
        //if (transform.parent!=null && transform.parent.tag == "Balka")
        //{
        //    transform.Rotate(new Vector3(Time.deltaTime * 50f, 0, 0));
        //    debug = transform.eulerAngles;
        //    debug2 = transform.localEulerAngles;
        //}
        
    }
}
