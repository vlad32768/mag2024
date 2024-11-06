using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copy : MonoBehaviour 
{
    public GameObject source;
    public float time;
    public bool pause;
    public float startPause;
    public BoxCollider coll;
    public int nobjects = 0;
    // Start is called before the first frame update
    void Start()
    {
       coll = GetComponent<BoxCollider>();
    }


    // Update is called once per frame
    void Update()
    {
        
        //time = Time.time;
        if (Input.GetKeyDown(KeyCode.A))
        {
            Instantiate(source, transform.position, transform.rotation);
        }
        if (pause && Time.time-startPause>2 && nobjects==0)
        {
            pause = false;
            Instantiate(source, transform.position, transform.rotation);
        }
    }
    
    
    private void OnTriggerExit(Collider other)
    {
        nobjects--;
        if (other.gameObject.CompareTag("Load"))
            Delay();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        nobjects++;
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    print("Stay");
    //}
    void Delay ()
    {
        pause = true;
        startPause = Time.time;
        
    }
}
