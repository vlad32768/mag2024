using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class DataPlotter : MonoBehaviour
{
    [Header("Data Settings")]
    public List<float> dataPointsX = new List<float>();
    public List<float> dataPointsY = new List<float>();
    public string xAxisLabel = "X Axis";
    public string yAxisLabel = "Y Axis";

    [Header("Appearance Settings")]
    public Color lineColor = Color.green;
    public float lineThickness = 2.0f;
    public Vector2 plotSize = new Vector2(400, 200);
    public Color axisColor = Color.white;
    public float axisThickness = 2.0f;

    private RectTransform plotArea;
    private LineRenderer lineRenderer;
    private GameObject xAxisLine;
    private GameObject yAxisLine;
    private TMP_Text xAxisText;
    private TMP_Text yAxisText;

    void Start()
    {
        InitializePlotArea();
        CreateAxisLines();
        CreateAxisLabels();
        DrawPlot();
    }

    void InitializePlotArea()
    {
        plotArea = GetComponent<RectTransform>();
        plotArea.sizeDelta = plotSize;

        // Create LineRenderer
        GameObject lineGO = new GameObject("PlotLine");
        lineGO.transform.SetParent(transform, false);
        lineRenderer = lineGO.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineThickness / 100f;
        lineRenderer.endWidth = lineThickness / 100f;
        lineRenderer.useWorldSpace = false;
    }

    void CreateAxisLines()
    {
        // X-axis
        xAxisLine = new GameObject("XAxis");
        xAxisLine.transform.SetParent(transform, false);
        Image xImage = xAxisLine.AddComponent<Image>();
        xImage.color = axisColor;

        RectTransform xRect = xAxisLine.GetComponent<RectTransform>();
        xRect.anchorMin = new Vector2(0, 0);
        xRect.anchorMax = new Vector2(1, 0);
        xRect.sizeDelta = new Vector2(0, axisThickness);
        xRect.anchoredPosition = Vector2.zero;

        // Y-axis
        yAxisLine = new GameObject("YAxis");
        yAxisLine.transform.SetParent(transform, false);
        Image yImage = yAxisLine.AddComponent<Image>();
        yImage.color = axisColor;

        RectTransform yRect = yAxisLine.GetComponent<RectTransform>();
        yRect.anchorMin = new Vector2(0, 0);
        yRect.anchorMax = new Vector2(0, 1);
        yRect.sizeDelta = new Vector2(axisThickness, 0);
        yRect.anchoredPosition = Vector2.zero;
    }

    void CreateAxisLabels()
    {
        // X-axis label
        GameObject xTextGO = new GameObject("XAxisLabel");
        xTextGO.transform.SetParent(transform, false);
        xAxisText = xTextGO.AddComponent<TextMeshProUGUI>();
        xAxisText.text = xAxisLabel;
        xAxisText.alignment = TextAlignmentOptions.Center;
        xAxisText.fontSize = 14;

        RectTransform xTextRect = xTextGO.GetComponent<RectTransform>();
        xTextRect.anchorMin = new Vector2(0.5f, 0);
        xTextRect.anchorMax = new Vector2(0.5f, 0);
        xTextRect.pivot = new Vector2(0.5f, 1);
        xTextRect.anchoredPosition = new Vector2(0, -20);

        // Y-axis label
        GameObject yTextGO = new GameObject("YAxisLabel");
        yTextGO.transform.SetParent(transform, false);
        yAxisText = yTextGO.AddComponent<TextMeshProUGUI>();
        yAxisText.text = yAxisLabel;
        yAxisText.alignment = TextAlignmentOptions.Center;
        yAxisText.fontSize = 14;
        yAxisText.transform.Rotate(0, 0, 90);

        RectTransform yTextRect = yTextGO.GetComponent<RectTransform>();
        yTextRect.anchorMin = new Vector2(0, 0.5f);
        yTextRect.anchorMax = new Vector2(0, 0.5f);
        yTextRect.pivot = new Vector2(1, 0.5f);
        yTextRect.anchoredPosition = new Vector2(-20, 0);
    }

    public void DrawPlot()
    {
        if (dataPointsY == null || dataPointsY.Count < 2 || dataPointsX == null || dataPointsX.Count < 2)
        {
            Debug.LogError("Need at least two data points to plot.");
            return;
        }

        // Calculate min/max values
        float minY = Mathf.Infinity;
        float maxY = -Mathf.Infinity;
        foreach (float y in dataPointsY)
        {
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        float minX = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        foreach (float x in dataPointsX)
        {
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
        }

        if (minY == maxY) maxY += 0.1f;

        // Calculate positions
        Vector3[] positions = new Vector3[dataPointsY.Count];
        float plotWidth = plotSize.x;
        float plotHeight = plotSize.y;

        for (int i = 0; i < dataPointsY.Count; i++)
        {
            float x = ((dataPointsX[i]) / (maxX - minX)) * plotWidth;
            //float x = (float)i / (dataPoints.Count - 1) * plotWidth;
            float y = ((dataPointsY[i]) / (maxY - minY)) * plotHeight;
            positions[i] = new Vector3(x, y, 0);
        }

        lineRenderer.positionCount = dataPointsX.Count;
        lineRenderer.SetPositions(positions);
    }

    public void UpdateData(List<float> newDataX, List<float> newDataY)
    {
        dataPointsX = newDataX;
        dataPointsY = newDataY;
        DrawPlot();
    }
}
