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
            xc = transform.position.x - t.position.x;
            transform.SetPositionAndRotation(t.position, t.rotation);
            rigidbody.isKinematic = true;
            coll.isTrigger = true;
            if (is_load)
            {
                transform.Translate(new Vector3(0, 0, xc));
            }
            transform.SetParent(t);
        }
        else
        {
            rigidbody.isKinematic = false;
            coll.isTrigger = false;
        }
        EnableChildColliders();
    }
    // Update is called once per frame
    void Update()
    {
        Collider collider = GetComponent<Collider>();


        
    }
}
