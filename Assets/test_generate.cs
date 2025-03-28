using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class test_generate : MonoBehaviour
{
    MeshFilter mf;
    MeshRenderer mr;
    public Mesh m;
    public balka_solver bs;
    public float yc = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    (List<Vector3>, List<Vector3>) GenerateIBeam()
    {
        List<Vector3> norm = new List<Vector3>();
        List<Vector3> IBeam = new List<Vector3>();
        IBeam.Add(new Vector3(0, 0, 0)); norm.Add(Vector3.left);
        IBeam.Add(new Vector3(0, 0, 0)); norm.Add(Vector3.down);
        IBeam.Add(new Vector3(0.3f, 0, 0)); norm.Add(Vector3.down);
        IBeam.Add(new Vector3(0.3f, 0, 0)); norm.Add(Vector3.right);
        IBeam.Add(new Vector3(0.3f, 0.1f, 0)); norm.Add(Vector3.right);
        IBeam.Add(new Vector3(0.3f, 0.1f, 0)); norm.Add(Vector3.up);

        IBeam.Add(new Vector3(0.2f, 0.1f, 0)); norm.Add(Vector3.up);
        IBeam.Add(new Vector3(0.2f, 0.1f, 0)); norm.Add(Vector3.right);
        IBeam.Add(new Vector3(0.2f, 0.5f, 0)); norm.Add(Vector3.right);
        IBeam.Add(new Vector3(0.2f, 0.5f, 0)); norm.Add(Vector3.down);
        IBeam.Add(new Vector3(0.3f, 0.5f, 0)); norm.Add(Vector3.down);
        IBeam.Add(new Vector3(0.3f, 0.5f, 0)); norm.Add(Vector3.right);

        IBeam.Add(new Vector3(0.3f, 0.6f, 0)); norm.Add(Vector3.right);
        IBeam.Add(new Vector3(0.3f, 0.6f, 0)); norm.Add(Vector3.up);
        IBeam.Add(new Vector3(0, 0.6f, 0)); norm.Add(Vector3.up);
        IBeam.Add(new Vector3(0, 0.6f, 0)); norm.Add(Vector3.left);
        IBeam.Add(new Vector3(0, 0.5f, 0)); norm.Add(Vector3.left);
        IBeam.Add(new Vector3(0, 0.5f, 0)); norm.Add(Vector3.down);

        IBeam.Add(new Vector3(0.1f, 0.5f, 0)); norm.Add(Vector3.down);
        IBeam.Add(new Vector3(0.1f, 0.5f, 0)); norm.Add(Vector3.left);
        IBeam.Add(new Vector3(0.1f, 0.1f, 0)); norm.Add(Vector3.left);
        IBeam.Add(new Vector3(0.1f, 0.1f, 0)); norm.Add(Vector3.up);
        IBeam.Add(new Vector3(0f, 0.1f, 0)); norm.Add(Vector3.up);
        IBeam.Add(new Vector3(0f, 0.1f, 0)); norm.Add(Vector3.left);
        float sc = 1f / 3f;
        
        Vector3 xs = Vector3.right * -0.15f;
        Vector3 ys = Vector3.up * -0.3f;
        
        for (int i = 0; i < IBeam.Count; ++i)
        {
            IBeam[i] += (xs + ys);
            IBeam[i] *= sc;
            
        }
        return (IBeam,norm);
        

        

    }
    
    

    private void GenerateBeamMesh()
    {
      
        List<Vector3> vtx = new List<Vector3>();
        List<Vector3> norm = new List<Vector3>();
        List<int> tris = new List<int>();


        float r = 0.2f;
        int sctn = 1000;


        var (IBeam,Norm) = GenerateIBeam();

        for (int i = 0; i < sctn; i++)
        {
            Vector3 z = Vector3.forward * 2f / sctn * i;
            Vector3 y = Vector3.up * - bs.CalculateDeflection(z.z);
            

            for (int j = 0; j < IBeam.Count; j++)
            {
                vtx.Add(IBeam[j] + z + y);
                norm.Add(Norm[j]);
            }
        }
        
        var n = IBeam.Count;
        for (int i = 0; i < sctn - 1; i++)
        {

            for (int j = 1; j < n; j+=2)
            {
                // <cond>?<exp1>:<exp2>
                int jnext = j == n - 1 ? 0 : j + 1;
                tris.Add(jnext + (i + 1) * n);
                tris.Add(j + (i + 1) * n);
                tris.Add(j + i * n);

                tris.Add(jnext + i * n);
                tris.Add(jnext + (i + 1) * n);
                tris.Add(j + i * n);
            }
        }

        m = new Mesh();
        m.vertices = vtx.ToArray();
        m.triangles = tris.ToArray();
        m.normals = norm.ToArray();
        m.name = "Beam_Mesh";
    }
    void Start()
    {
        mf = gameObject.GetComponent<MeshFilter>();
        GenerateBeamMesh();
        mf.mesh = m;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            yc = bs.CalculateDeflection(1f);
            mf = gameObject.GetComponent<MeshFilter>();
            GenerateBeamMesh();
            mf.mesh = m;
        }

    }
}
