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
        painter.fillColor = Color.green;
        painter.BeginPath();
        painter.MoveTo(new Vector2(0, 0));
        painter.LineTo(new Vector2(0, 512));
        painter.LineTo(new Vector2(512, 512));
        painter.LineTo(new Vector2(512, 0));
        painter.ClosePath();
        painter.Fill();
        painter.Stroke();

        // Îñü Z ýïþð
        painter.strokeColor = Color.white;
        painter.lineWidth = 5;

        painter.BeginPath();
        painter.MoveTo(new Vector2(0, 512 / 3));
        painter.LineTo(new Vector2(512, 512 / 3));
        painter.MoveTo(new Vector2(0, 512 / 3 * 2));
        painter.LineTo(new Vector2(512, 512 / 3 * 2));
        painter.ClosePath();
        painter.Stroke();


        painter.strokeColor = Color.blue;
        painter.lineWidth = 5;
        painter.BeginPath();
        float l = 2;
        float w = 512 * 0.9f;
        float b = (512 - w) / 2;
        float h = 512 * 0.3f;
        float hq = 512 / 3 * 2;
        float hm = 512 / 3;
        if (slv.QArray.Count != 0)
        {
            painter.MoveTo(new Vector2(b, hq));
            painter.LineTo(new Vector2(slv.QArray[0][2] / l * w, hq));
            painter.LineTo(new Vector2(slv.QArray[0][2] / l * w, slv.QArray[0][0] / slv.QMax * h / 2 + hq));

            for (int i = 0; i < slv.QArray.Count; i++)
            {
                var x1 = slv.QArray[i][2] / l * w;
                var x2 = slv.QArray[i][3] / l * w;
                var y1 = slv.QArray[i][0] / slv.QMax * h / 2 + hq;
                var y2 = slv.QArray[i][1] / slv.QMax * h / 2 + hq;

                painter.LineTo(new Vector2(x1, y1));
                painter.LineTo(new Vector2(x2, y2));
            }
            painter.LineTo(new Vector2(slv.QArray[slv.QArray.Count - 1][3] / l * w, hq));
            painter.LineTo(new Vector2(512 - b, hq));
            painter.Stroke();
        }

        if (slv.MArray.Count != 0)
        {
            painter.MoveTo(new Vector2(b, hm));
            painter.LineTo(new Vector2(slv.MArray[0][2] / l * w, hm));
            painter.LineTo(new Vector2(slv.MArray[0][2] / l * w, slv.MArray[0][0] / slv.MMax * h / 2 + hm));

            for (int i = 0; i < slv.MArray.Count; i++)
            {
                var x1 = slv.MArray[i][2] / l * w;
                var x2 = slv.MArray[i][3] / l * w;
                var y1 = slv.MArray[i][0] / slv.MMax * h / 2 + hm;
                var y2 = slv.MArray[i][1] / slv.MMax * h / 2 + hm;

                painter.LineTo(new Vector2(x1, y1));
                painter.LineTo(new Vector2(x2, y2));
            }
            painter.LineTo(new Vector2(slv.MArray[slv.MArray.Count - 1][3] / l * w, hm));
            painter.LineTo(new Vector2(512 - b, hm));
            painter.Stroke();
        }
    }
}