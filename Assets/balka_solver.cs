using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.IO.Compression;
using System;

public class balka_solver : MonoBehaviour
{

    List<List<float>> forceArray = new List<List<float>>();
    List<List<float>> loadArray = new List<List<float>>();
    List<List<float>> momentArray = new List<List<float>>();
    List<List<float>> BoundaryArray = new List<List<float>>();
    List<List<float>> ReactionArray = new List<List<float>>();

    List<List<float>> combinedArrayQ = new List<List<float>>();
    List<List<float>> combinedArrayM = new List<List<float>>();
    List<float> intervalsArrayQ = new List<float>();
    List<float> intervalsArrayM = new List<float>();
    public List<List<float>> QArray = new List<List<float>>();
    public List<List<float>> MArray = new List<List<float>>();

    public float Ra = 0;
    public float Ma = 0;
    public float Rb = 0;
    public float QMax = 0;
    public float MMax = 0;

    public int intervals = 0;
    public int StatN = 0;


    List<load_data> loads;

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
        loads = new List<load_data>();
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
                    forceArray.Add(new List<float> { item.z, item.sign * item.ls.param, 0 });
                    break;
                case load_script.LoadType.Torque:
                    momentArray.Add(new List<float> { item.z, item.sign * item.ls.param, 0 });
                    break;
                case load_script.LoadType.Joint1:
                    BoundaryArray.Add(new List<float> { item.z, 1, 0 });
                    break;
                case load_script.LoadType.Joint2:
                    BoundaryArray.Add(new List<float> { item.z, 2, 0 });
                    break;
                case load_script.LoadType.Console:
                    BoundaryArray.Add(new List<float> { item.z, 3, 0 });
                    break;
            }
        }


    }

    void Solve()
    {
        StatN = 0; Ra = 0; Ma = 0; Rb = 0; intervals = 0;

        for (int i = 0; i < BoundaryArray.Count; i++) { StatN = StatN + (int)BoundaryArray[i][1]; }
        print("StatN: " + StatN + "BoundaryArray.Count" + BoundaryArray.Count);
        if (StatN == 3)
        {
            if (BoundaryArray.Count == 1)
            {
                for (int i = 0; i < forceArray.Count; i++)
                {
                    Ra = Ra - forceArray[i][1];
                    Ma = Ma - forceArray[i][0] * Math.Abs(forceArray[i][1] - BoundaryArray[0][0]);
                }
                for (int i = 0; i < loadArray.Count; i++)
                {
                    // Распределенная нагрузка пока не реализована :-( 
                    //Ra = Ra - (loadArray[i][1] - loadArray[i][0]) * loadArray[i][2];
                    //Ma = Ma - (loadArray[i][0] + (loadArray[i][1] - loadArray[i][0]) / 2) * (loadArray[i][1] - loadArray[i][0]) * loadArray[i][2];
                }
                for (int i = 0; i < momentArray.Count; i++)
                {
                    Ma = Ma - momentArray[i][1];
                }
                forceArray.Add(new List<float> { BoundaryArray[0][0], Ra, 0 });      // Координата по Z и значение силы 3
                momentArray.Add(new List<float> { BoundaryArray[0][0], Ma, 0 });      // Координата по Z и значение силы 3

            }
            if (BoundaryArray.Count == 2)
            {
                if (BoundaryArray[1][0] > BoundaryArray[0][0])
                {
                    List<float> tempRow = BoundaryArray[0];
                    BoundaryArray[0] = BoundaryArray[1];
                    BoundaryArray[1] = tempRow; ;
                }

                var z = BoundaryArray[0][0] - BoundaryArray[1][0];
                for (int i = 0; i < forceArray.Count; i++)
                {
                    var z1 = -1 * (BoundaryArray[1][0] - forceArray[i][0]);
                    var z2 = 1 * (BoundaryArray[0][0] - forceArray[i][0]);
                    Ra = Ra - forceArray[i][1] * z1 / z;
                    Rb = Rb - forceArray[i][1] * z2 / z;
                }
                for (int i = 0; i < loadArray.Count; i++)
                {
                    // Тут пусто, так как распределенные нагрузки не реализованны
                }
                for (int i = 0; i < momentArray.Count; i++)
                {
                    Ra = Ra - momentArray[i][1] / z;
                    Rb = Rb - momentArray[i][1] / z;
                }
                forceArray.Add(new List<float> { BoundaryArray[0][0], Ra, 0 });      // Координата по Z и значение силы
                forceArray.Add(new List<float> { BoundaryArray[1][0], Rb, 0 });      // Координата по Z и значение силы
            }
            intervals = BoundaryArray.Count + momentArray.Count + forceArray.Count + loadArray.Count * 2 - 1;
        }
        else { print("Балка статическа неопределима, давай по новой"); }
    }



    void Draw()
    {
        combinedArrayQ.Clear();
        combinedArrayM.Clear();
        intervalsArrayQ.Clear();
        intervalsArrayM.Clear();
        QArray.Clear();
        MArray.Clear();

        combinedArrayQ.AddRange(forceArray);
        combinedArrayQ.AddRange(loadArray);

        combinedArrayM.AddRange(forceArray);
        combinedArrayM.AddRange(loadArray);
        combinedArrayM.AddRange(momentArray);

        for (int i = 0; i < combinedArrayQ.Count; i++) { intervalsArrayQ.Add(combinedArrayQ[i][0]); }
        for (int i = 0; i < combinedArrayM.Count; i++) { intervalsArrayM.Add(combinedArrayM[i][0]); }

        intervalsArrayQ.Sort();
        intervalsArrayQ = intervalsArrayQ.Distinct().ToList();
        intervalsArrayM.Sort();
        intervalsArrayM = intervalsArrayM.Distinct().ToList();
        print("\nIntervalsArrayQ: " + string.Join(", ", intervalsArrayQ));
        print("\nIntervalsArrayM: " + string.Join(", ", intervalsArrayM));

        // Расчет значений для эпюры поперечных сил
        for (int i = 1; i < intervalsArrayQ.Count; i++)
        {
            float Q1 = 0; float Q2 = 0; float z1 = 0; float z2 = 0;
            for (int j = 0; j < forceArray.Count; j++)
            {
                if (forceArray[j][0] < intervalsArrayQ[i])
                {
                    Q1 = Q1 - forceArray[j][1];
                    Q2 = Q1;
                    z1 = intervalsArrayQ[i - 1];
                    z2 = intervalsArrayQ[i];
                }
            }

            for (int k = 0; k < loadArray.Count; k++)
            {
                if (loadArray[k][0] < intervalsArrayQ[i])
                {
                    Q1 = Q1 + 0;
                    Q2 = Q2 + 0;
                    z1 = intervalsArrayQ[i - 1];
                    z2 = intervalsArrayQ[i];
                }
            }

            QArray.Add(new List<float> { Q1, Q2, z1, z2 });
            QMax = QArray.SelectMany(row => new[] { Math.Abs(row[0]), Math.Abs(row[1]) }).Max();
            print("\nQArray: " + Q1 + " " + Q2 + " " + z1 + " " + z2);
        }

        // Расчет значений для эпюры изгибающих моментов
        for (int i = 1; i < intervalsArrayM.Count; i++)
        {
            float M1 = 0; float M2 = 0; float z1 = 0; float z2 = 0;
            for (int j = 0; j < forceArray.Count; j++)
            {
                if (forceArray[j][0] < intervalsArrayM[i])
                {
                    z1 = intervalsArrayM[i - 1];
                    z2 = intervalsArrayM[i];
                    M1 = M1 + forceArray[j][1] * (z1 - forceArray[j][0]);
                    M2 = M2 + forceArray[j][1] * (z2 - forceArray[j][0]);
                }
            }

            for (int j = 0; j < loadArray.Count; j++)
            {
                if (loadArray[j][0] < intervalsArrayM[i])
                {
                    z1 = intervalsArrayM[i - 1];
                    z2 = intervalsArrayM[i];
                    M1 = M1 + 0;
                    M2 = M2 + 0;
                }
            }

            for (int j = 0; j < momentArray.Count; j++)
            {
                if (momentArray[j][0] < intervalsArrayM[i])
                {
                    z1 = intervalsArrayM[i - 1];
                    z2 = intervalsArrayM[i];
                    M1 = M1 - momentArray[j][1];
                    M2 = M2 - momentArray[j][1];
                }
            }
            MArray.Add(new List<float> { M1, M2, z1, z2 });
            MMax = MArray.SelectMany(row => new[] { Math.Abs(row[0]), Math.Abs(row[1]) }).Max();
            print("\nMArray: " + M1 + " " + M2 + " " + z1 + " " + z2);
        }
    }

    void ProcessChildren()
    {
        loads.Clear();
        var num = transform.childCount;
        for (int i = 0; i < num; i++)
        {
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

                loads.Add(new load_data(c, sgn, ls));
            }
        }

        loads = loads.OrderBy(a => a.z).ToList();

        foreach (var item in loads)
        {
            print(string.Format("z={0},type= {2}, param={1}, sgn={3}", item.z, item.ls.param, item.ls.loadType.ToString(), item.sign));
        }

    }

    public void Do_Solve()
    {

        ProcessChildren();
        FillArrays();
        Solve();
        Draw();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            Do_Solve();
        }

    }
}
