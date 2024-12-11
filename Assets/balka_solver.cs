using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
public class balka_solver : MonoBehaviour
{

    List<List<float>> forceArray = new List<List<float>>();
    List<List<float>> loadArray = new List<List<float>>();
    List<List<float>> momentArray = new List<List<float>>();
    List<List<float>> BoundaryArray = new List<List<float>>();
    List<List<float>> ReactionArray = new List<List<float>>();

    List<List<float>> combinedArray = new List<List<float>>();
    List<float> intervalsArray = new List<float>();
    public List<List<float>> QArray = new List<List<float>>();
    public List<List<float>> MArray = new List<List<float>>();

    public float R = 0;
    public float H = 0;
    public float M = 0;
    public int intervals = 0;
    public int intervalsFL = 0;
    public int intervalsM = 0;
    public int StatN = 0;
    public int Row = 0;



    List <load_data> loads;
    
    struct load_data
    {
        public load_data(float a, float s, load_script b)
        {
            z = a;
            sign = s;
            ls = b;
        }

        public float z;
        public float sign;
        public load_script ls;
    }

    void Start()
    {
        loads = new List <load_data> ();
    }

    /// <summary>
    ///  Fill arrays for solving
    ///  
    ///  Need loads array
    /// </summary>
    void FillArrays()
    {
        forceArray.Clear();
        momentArray.Clear();
        BoundaryArray.Clear();
        loadArray.Clear();

        foreach (var item in loads)
        {
            switch (item.ls.loadType)
            {
                case load_script.LoadType.Force:
                    forceArray.Add(new List<float> { item.z, item.sign * item.ls.param,0 });
                break;
                case load_script.LoadType.Torque:
                    momentArray.Add(new List<float> { item.z, item.sign * item.ls.param, 0 });
                break;
            }    
        }

        BoundaryArray.Add(new List<float> {0,3,0});
    }

    void Solve()
    {
        StatN = 0;


        R = 0;
        H = 0;
        M = 0;
        intervals = 0;
        intervalsFL = 0;
        intervalsM = 0;
        StatN = 0;
        Row = 0;


        for (int i = 0; i < BoundaryArray.Count; i++) { StatN = StatN + (int)BoundaryArray[i][1]; }
        if (StatN != 3) { print("Балка статическа неопределима, давай по новой"); }

        if (BoundaryArray.Count == 1 && BoundaryArray[0][0] == 0 && BoundaryArray[0][1] == 3)
        {
            for (int i = 0; i < forceArray.Count; i++)
            {
                R = R - forceArray[i][1];
                M = M - forceArray[i][0] * forceArray[i][1];
            }
            for (int i = 0; i < loadArray.Count; i++)
            {
                R = R - (loadArray[i][1] - loadArray[i][0]) * loadArray[i][2];
                M = M - (loadArray[i][0] + (loadArray[i][1] - loadArray[i][0]) / 2) * (loadArray[i][1] - loadArray[i][0]) * loadArray[i][2];
            }
            for (int i = 0; i < momentArray.Count; i++)
            {
                M = M - momentArray[i][1];
            }
            forceArray.Add(new List<float> { BoundaryArray[0][0], R, 0 });      // Координата по Z и значение силы 3
            momentArray.Add(new List<float> { BoundaryArray[0][0], M, 0 });      // Координата по Z и значение силы 3
            intervals = BoundaryArray.Count + momentArray.Count + forceArray.Count + loadArray.Count * 2 - 1;
            intervalsFL = intervals - momentArray.Count;
            intervalsM = intervals;
        }

        
    }

  
    void Draw()
    {
        combinedArray.Clear();
        intervalsArray.Clear();
        QArray.Clear();
        MArray.Clear();

        combinedArray.AddRange(forceArray);
        combinedArray.AddRange(loadArray);
        combinedArray.AddRange(momentArray);
        //combinedArray.AddRange(BoundaryArray);

        for (int i = 0; i < combinedArray.Count; i++) { intervalsArray.Add(combinedArray[i][0]); }

        intervalsArray.Sort();
        intervalsArray = intervalsArray.Distinct().ToList();
        print("\nIntervalsArray: " + string.Join(", ", intervalsArray));

        // Расчет значений для эпюры поперечных сил
        for (int i = 1; i < intervalsArray.Count; i++)
        {
            float Q1 = 0; float Q2 = 0; float z1 = 0; float z2 = 0;
            for (int j = 0; j < forceArray.Count; j++)
            {
                if (forceArray[j][0] < intervalsArray[i])
                {
                    Q1 = Q1 + forceArray[j][1];
                    Q2 = Q1;
                    z1 = intervalsArray[i - 1];
                    z2 = intervalsArray[i];
                }
            }

            for (int k = 0; k < loadArray.Count; k++)
            {
                if (loadArray[k][0] < intervalsArray[i])
                {
                    Q1 = Q1 + 0;
                    Q2 = Q2 + 0;
                    z1 = intervalsArray[i - 1];
                    z2 = intervalsArray[i];
                }
            }

            QArray.Add(new List<float> { Q1, Q2, z1, z2 });
            print("\nQArray: " + Q1 + " " + Q2 + " " + z1 + " " + z2);
        }

        // Расчет значений для эпюры изгибающих моментов
        for (int i = 1; i < intervalsArray.Count; i++)
        {
            float M1 = 0; float M2 = 0; float z1 = 0; float z2 = 0;
            for (int j = 0; j < forceArray.Count; j++)
            {
                if (forceArray[j][0] < intervalsArray[i])
                {
                    z1 = intervalsArray[i - 1];
                    z2 = intervalsArray[i];
                    M1 = M1 + forceArray[j][1] * (z1 - forceArray[j][0]);
                    M2 = M2 + forceArray[j][1] * (z2 - forceArray[j][0]);
                }
            }

            for (int j = 0; j < loadArray.Count; j++)
            {
                if (loadArray[j][0] < intervalsArray[i])
                {
                    z1 = intervalsArray[i - 1];
                    z2 = intervalsArray[i];
                    M1 = M1 + 0;
                    M2 = M2 + 0;
                }
            }

            for (int j = 0; j < momentArray.Count; j++)
            {
                if (momentArray[j][0] < intervalsArray[i])
                {
                    z1 = intervalsArray[i - 1];
                    z2 = intervalsArray[i];
                    M1 = M1 - momentArray[j][1];
                    M2 = M2 - momentArray[j][1];
                }
            }
            MArray.Add(new List<float> { M1, M2, z1, z2 });
            print("\nMArray: " + M1 + " " + M2 + " " + z1 + " " + z2);
        }



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
                var c = child.localPosition.z;
                
                var ls = child.gameObject.GetComponent<load_script>();
                // Load gameobject orientation * global orientation!
                var dot_up = Vector3.Dot(Vector3.up, child.up);
                var dotp_fwd = Vector3.Dot(new Vector3(1, 0, 0), child.forward);

                var sgn = dot_up;
                if (ls.loadType == load_script.LoadType.Torque) sgn *= dotp_fwd;

                loads.Add(new load_data(c,sgn,ls));
            }
        }

        loads=loads.OrderBy(a=>a.z).ToList();
        
        foreach (var item in loads)
        {
            print(string.Format("z={0},type= {2}, param={1}, sgn={3}", item.z, item.ls.param, item.ls.loadType.ToString(),item.sign));
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            ProcessChildren();
            FillArrays();
            Solve();
            Draw();
        }
        
    }
}
