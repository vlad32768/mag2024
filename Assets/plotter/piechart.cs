using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class PieChart : VisualElement
{
    balka_solver slv;
    
    float m_Radius = 100.0f;
    float m_Value = 40.0f;

    public float radius
    {
        get => m_Radius;
        set
        {
            m_Radius = value;
        }
    }

    public float diameter => m_Radius * 2.0f;

    public float value
    {
        get { return m_Value; }
        set { m_Value = value; MarkDirtyRepaint(); }
    }

    public PieChart()
    {
        generateVisualContent += DrawCanvas;
    }
    public PieChart(balka_solver s)
    {
        slv = s;
        generateVisualContent += DrawCanvas;
    }

    void DrawCanvas(MeshGenerationContext ctx)
    {
       // if (slv.QArray.Count!=0)Debug.Log(slv.QArray[0][0]);
        //Console.Write(slv.QArray.ToString());
        var painter = ctx.painter2D;
        painter.strokeColor = Color.white;
        painter.fillColor = Color.white;
        painter.BeginPath();
        painter.MoveTo(new Vector2(0, 0));
        painter.LineTo(new Vector2(0, 512));
        painter.LineTo(new Vector2(512, 512));
        painter.LineTo(new Vector2(512, 0));
        painter.ClosePath();
        painter.Fill();
        painter.Stroke();


        painter.strokeColor = Color.blue;
        painter.fillColor = Color.red;
        painter.lineWidth = 1;

        //painter.Clear();
        painter.BeginPath();
        painter.MoveTo(new Vector2(0, 256));
        painter.LineTo(new Vector2(512, 256));
        painter.ClosePath();
        painter.Stroke();

        painter.strokeColor = Color.red;
        painter.fillColor = Color.green;
        painter.lineWidth = 1;
        
        painter.BeginPath();
        painter.MoveTo(new Vector2(0, 256));
        for (float i = 0; i < 4 * Mathf.PI; i+=0.1f)
        {
            painter.LineTo(new Vector2(i * 512.0f / (4.0f * (Mathf.PI)), 256.0f + (Mathf.Sin(i)+UnityEngine.Random.value / 4.0f-0.2f) * 128));
        };
        painter.Stroke();

        //painter.Fill();
        /*
        var percentage = m_Value;

        var percentages = new float[] {
            percentage, 100 - percentage
        };
        var colors = new Color32[] {
            new Color32(182,235,122,255),
            new Color32(251,120,19,255)
        };
        float angle = 0.0f;
        float anglePct = 0.0f;
        int k = 0;
        foreach (var pct in percentages)
        {
            anglePct += 360.0f * (pct / 100);

            painter.fillColor = colors[k++];
            painter.BeginPath();
            painter.MoveTo(new Vector2(m_Radius, m_Radius));
            painter.Arc(new Vector2(m_Radius, m_Radius), m_Radius, angle, anglePct);
            painter.Fill();

            angle = anglePct;
        }
        */
    }
}