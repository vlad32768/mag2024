using System.Collections.Generic;
using UnityEngine;

public class test_generate : MonoBehaviour
{
    MeshFilter mf;
    MeshRenderer mr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mf = gameObject.GetComponent<MeshFilter>();

        List<Vector3> vtx = new List<Vector3>();
        List<int> tris = new List<int>();

        int sctn = 10;

        for(int i = 0; i < sctn; i++)
        {
            float z = 0.1f * i;
            vtx.Add(new Vector3(0, z * z, z));
            vtx.Add(new Vector3(0.3f, z * z, z));
            vtx.Add(new Vector3(0.3f, 0.3f+ z * z, z));
            vtx.Add(new Vector3(0, 0.3f + z * z, z));
        }

        tris.Add(0);
        tris.Add(2);
        tris.Add(1);

        tris.Add(0);
        tris.Add(3);
        tris.Add(2);

        tris.Add((sctn - 1) * 4 + 1);
        tris.Add((sctn - 1) * 4 + 2);
        tris.Add((sctn - 1) * 4 + 0);

        tris.Add((sctn - 1) * 4 + 2);
        tris.Add((sctn - 1) * 4 + 3);
        tris.Add((sctn - 1) * 4 + 0);

        for (int i = 0; i < sctn - 1; i++)
        {
            tris.Add(4 * i + 1);
            tris.Add(4 * i + 5);
            tris.Add(4 * i + 0);

            tris.Add(4 * i + 5);
            tris.Add(4 * i + 4);
            tris.Add(4 * i + 0);
            //1
            tris.Add(4 * i + 2);
            tris.Add(4 * i + 6);
            tris.Add(4 * i + 1);

            tris.Add(4 * i + 6);
            tris.Add(4 * i + 5);
            tris.Add(4 * i + 1);
            //2
            tris.Add(4 * i + 3);
            tris.Add(4 * i + 7);
            tris.Add(4 * i + 2);

            tris.Add(4 * i + 7);
            tris.Add(4 * i + 6);
            tris.Add(4 * i + 2);
            //3
            tris.Add(4 * i + 3);
            tris.Add(4 * i + 0);
            tris.Add(4 * i + 4);

            tris.Add(4 * i + 3);
            tris.Add(4 * i + 4);
            tris.Add(4 * i + 7);
            //4
        }



        



        Mesh m = new Mesh();
        m.vertices = vtx.ToArray();
        m.triangles = tris.ToArray();
       

        mf.mesh = m;
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
