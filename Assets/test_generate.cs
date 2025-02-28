using System.Collections.Generic;
using UnityEngine;

public class test_generate : MonoBehaviour
{
    MeshFilter mf;
    MeshRenderer mr;
    public Mesh m;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mf = gameObject.GetComponent<MeshFilter>();

        List<Vector3> vtx = new List<Vector3>();
        List<int> tris = new List<int>();

        float r = 0.2f;
        int sctn = 10;
        int n = 8;

        for (int i = 0; i < sctn; i++)
        {
            float z = 0.1f * i;

            for (int j = 0; j < n; j++)
            {
                float angle = 2 * Mathf.PI / n * j;
                vtx.Add(new Vector3(r*Mathf.Cos(angle), r * Mathf.Sin(angle), z));
                //vtx.Add(new Vector3(0.3f, z * z, z));
                //vtx.Add(new Vector3(0.3f, 0.3f + z * z, z));
                //vtx.Add(new Vector3(0, 0.3f + z * z, z));
            }

            
            
        }
        for (int i = 0; i < sctn - 1; i++)
        {

            for (int j = 0; j < n; j++)
            {
                // <cond>?<exp1>:<exp2>
                int jnext = j == n - 1 ? 0 : j + 1;
                tris.Add(jnext + (i + 1) * n);
                tris.Add(j + (i+1) * n);
                tris.Add(j + i * n);

                tris.Add(jnext + i * n);
                tris.Add(jnext + (i+1) * n);
                tris.Add(j + i * n);
            }
            

        }



        //for (int i = 0; i < sctn; i++)
        //{
        //    float z = 0.1f * i;
        //    vtx.Add(new Vector3(0, z * z, z));
        //    vtx.Add(new Vector3(0.3f, z * z, z));
        //    vtx.Add(new Vector3(0.3f, 0.3f + z * z, z));
        //    vtx.Add(new Vector3(0, 0.3f + z * z, z));
        //}

        //tris.Add(0);
        //tris.Add(2);
        //tris.Add(1);

        //tris.Add(0);
        //tris.Add(3);
        //tris.Add(2);

        //tris.Add((sctn - 1) * 4 + 1);
        //tris.Add((sctn - 1) * 4 + 2);
        //tris.Add((sctn - 1) * 4 + 0);

        //tris.Add((sctn - 1) * 4 + 2);
        //tris.Add((sctn - 1) * 4 + 3);
        //tris.Add((sctn - 1) * 4 + 0);

        //for (int i = 0; i < sctn - 1; i++)
        //{
        //    tris.Add(4 * i + 1);
        //    tris.Add(4 * i + 5);
        //    tris.Add(4 * i + 0);

        //    tris.Add(4 * i + 5);
        //    tris.Add(4 * i + 4);
        //    tris.Add(4 * i + 0);
        //    //1
        //    tris.Add(4 * i + 2);
        //    tris.Add(4 * i + 6);
        //    tris.Add(4 * i + 1);

        //    tris.Add(4 * i + 6);
        //    tris.Add(4 * i + 5);
        //    tris.Add(4 * i + 1);
        //    //2
        //    tris.Add(4 * i + 3);
        //    tris.Add(4 * i + 7);
        //    tris.Add(4 * i + 2);

        //    tris.Add(4 * i + 7);
        //    tris.Add(4 * i + 6);
        //    tris.Add(4 * i + 2);
        //    //3
        //    tris.Add(4 * i + 3);
        //    tris.Add(4 * i + 0);
        //    tris.Add(4 * i + 4);

        //    tris.Add(4 * i + 3);
        //    tris.Add(4 * i + 4);
        //    tris.Add(4 * i + 7);
        //    //4
        //}







        m = new Mesh();
        m.vertices = vtx.ToArray();
        m.triangles = tris.ToArray();
        m.name = "AAAMESH";

        mf.mesh = m;
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
