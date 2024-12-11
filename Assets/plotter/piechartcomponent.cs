using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PieChartComponent : MonoBehaviour
{
    PieChart m_PieChart;
    public GameObject balkaobj;

    void Start()
    {
        m_PieChart = new PieChart(balkaobj.GetComponent<balka_solver>());
        GetComponent<UIDocument>().rootVisualElement.Add(m_PieChart);
    }

    private void Update()
    {
        m_PieChart.MarkDirtyRepaint();
    }
}