using UnityEngine;
using UnityEngine.UI;
using SkiaSharp;
using System.IO;
using System.Collections.Generic;

[RequireComponent(typeof(RawImage))]
public class SkiaPlot : MonoBehaviour
{
    [Header("Plot Settings")]
    public int textureWidth = 800;
    public int textureHeight = 600;
    public Vector2 plotBoundsMin = new Vector2(-10, -10);
    public Vector2 plotBoundsMax = new Vector2(10, 10);
    public float margin = 60f;

    [Header("Axis Settings")]
    public string xAxisLabel = "X Axis";
    public string yAxisLabel = "Y Axis";
    public float axisLabelFontSize = 16f;
    public float tickLabelFontSize = 12f;
    public float tickLength = 10f;
    public int xTicks = 10;
    public int yTicks = 10;

    [Header("Style Settings")]
    public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f);
    public Color axisColor = Color.white;
    public float axisWidth = 2.0f;
    public Color gridColor = new Color(0.4f, 0.4f, 0.4f);
    public float gridWidth = 0.5f;

    [Header("Data Series")]
    public List<DataSeries> dataSeries = new List<DataSeries>()
    {
        new DataSeries()
        {
            points = new List<Vector2>()
            {
                new Vector2(2f, 3f),
                new Vector2(-1f, 4f),
                new Vector2(3.5f, -2.5f),
                new Vector2(-4f, -3f)
            },
            color = Color.red,
            pointSize = 5f,
            name = "Series 1"
        }
    };

    [Header("Legend Settings")]
    public bool showLegend = true;
    public Vector2 legendPosition = new Vector2(0.8f, 0.8f); // normalized position
    public float legendFontSize = 14f;
    public float legendPadding = 10f;
    public float legendEntrySpacing = 5f;

    private Texture2D texture;
    private RawImage rawImage;
    private SKTypeface font;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        CreateTexture();
        LoadFont();
        DrawPlot();
    }

    void LoadFont()
    {
        // Load a default font (Arial is usually available)
        font = SKTypeface.FromFamilyName("Arial");
    }

    void CreateTexture()
    {
        // Vlad: it seems that Wintel default texture format is BGRA32
        // Skia and Unity texture formats must be the same
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.BGRA32, false);
        //texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        rawImage.texture = texture;
    }

    SKColor ToSKColor(Color c)
    {
        return new SKColor(
            (byte)(c.r * 255.0f),
            (byte)(c.g * 255.0f),
            (byte)(c.b * 255.0f),
            (byte)(c.a * 255.0f));
    }

    void DrawPlot()
    {
        // Vlad: Skia's default texture format is BGRA on Windows. 
        // Skia and Unity texture formats must be the same
        var info = new SKImageInfo(textureWidth, textureHeight/*,SKColorType.Bgra8888*/);
        //var info = new SKImageInfo(textureWidth, textureHeight,SKColorType.Rgba8888);
        using (var surface = SKSurface.Create(info))
        {
            var canvas = surface.Canvas;
            canvas.Clear(ToSKColor(backgroundColor));

            DrawGrid(canvas);
            DrawAxes(canvas);
            DrawTicks(canvas);
            DrawAxisLabels(canvas);
            DrawDataPoints(canvas);

            if (showLegend)
            {
                DrawLegend(canvas);
            }

            UpdateUnityTexture(surface);
        }
    }

    void DrawGrid(SKCanvas canvas)
    {
        var paint2 = new SKPaint
        {
            Color = new SKColor(255,0,0,255),
            StrokeWidth = 4,
            IsAntialias = true
        };
        canvas.DrawLine(0, 0, 200, 100, paint2);


        var paint = new SKPaint
        {
            Color = ToSKColor(gridColor),
            StrokeWidth = gridWidth,
            IsAntialias = true
        };

        // Vertical grid lines
        float xStep = (plotBoundsMax.x - plotBoundsMin.x) / xTicks;
        for (float x = plotBoundsMin.x; x <= plotBoundsMax.x; x += xStep)
        {
            var pos = DataToPixel(new Vector2(x, 0));
            canvas.DrawLine(pos.X, margin, pos.X, textureHeight - margin, paint);
        }

        // Horizontal grid lines
        float yStep = (plotBoundsMax.y - plotBoundsMin.y) / yTicks;
        for (float y = plotBoundsMin.y; y <= plotBoundsMax.y; y += yStep)
        {
            var pos = DataToPixel(new Vector2(0, y));
            canvas.DrawLine(margin, pos.Y, textureWidth - margin, pos.Y, paint);
        }
    }

    void DrawAxes(SKCanvas canvas)
    {
        var axisPaint = new SKPaint
        {
            Color = ToSKColor(axisColor),
            StrokeWidth = axisWidth,
            IsAntialias = true
        };

        // X axis
        var xStart = DataToPixel(new Vector2(plotBoundsMin.x, 0));
        var xEnd = DataToPixel(new Vector2(plotBoundsMax.x, 0));
        canvas.DrawLine(margin, xStart.Y, textureWidth - margin, xEnd.Y, axisPaint);

        // Y axis
        var yStart = DataToPixel(new Vector2(0, plotBoundsMin.y));
        var yEnd = DataToPixel(new Vector2(0, plotBoundsMax.y));
        canvas.DrawLine(yStart.X, margin, yEnd.X, textureHeight - margin, axisPaint);
    }

    void DrawTicks(SKCanvas canvas)
    {
        var tickPaint = new SKPaint
        {
            Color = ToSKColor(axisColor),
            StrokeWidth = 1.5f,
            IsAntialias = true
        };

        var labelFont = new SKFont(font, tickLabelFontSize);

        var labelPaint = new SKPaint
        {
            Color = ToSKColor(axisColor),
            //TextSize = tickLabelFontSize,
            IsAntialias = true,
            //Typeface = font
        };

        // X axis ticks
        float xStep = (plotBoundsMax.x - plotBoundsMin.x) / xTicks;
        for (float x = plotBoundsMin.x; x <= plotBoundsMax.x; x += xStep)
        {
            var pos = DataToPixel(new Vector2(x, 0));

            // Tick mark
            canvas.DrawLine(pos.X, pos.Y - tickLength / 2, pos.X, pos.Y + tickLength / 2, tickPaint);

            // Tick label
            string label = x.ToString("0.##");
            var textBounds = new SKRect();

            //labelPaint.MeasureText(label, ref textBounds);
            //canvas.DrawText(label, pos.X - textBounds.Width / 2, pos.Y + tickLength + textBounds.Height, labelPaint);
            
            // Vlad: Modern text stuff uses distinct SkFont structure; font stuff in SkPaint structure is obsolete 

            labelFont.MeasureText(label, out textBounds, labelPaint);
            canvas.DrawText(label, pos.X - textBounds.Width / 2, pos.Y + tickLength + textBounds.Height,labelFont, labelPaint);
        }

        // Y axis ticks
        float yStep = (plotBoundsMax.y - plotBoundsMin.y) / yTicks;
        for (float y = plotBoundsMin.y; y <= plotBoundsMax.y; y += yStep)
        {
            var pos = DataToPixel(new Vector2(0, y));

            // Tick mark
            canvas.DrawLine(pos.X - tickLength / 2, pos.Y, pos.X + tickLength / 2, pos.Y, tickPaint);

            // Tick label
            string label = y.ToString("0.##");
            var textBounds = new SKRect();
            labelPaint.MeasureText(label, ref textBounds);
            canvas.DrawText(label, pos.X - tickLength - textBounds.Width - 5, pos.Y + textBounds.Height / 2, labelPaint);
        }
    }

    void DrawAxisLabels(SKCanvas canvas)
    {
        var labelPaint = new SKPaint
        {
            Color = ToSKColor(axisColor),
            TextSize = axisLabelFontSize,
            IsAntialias = true,
            Typeface = font
        };

        // X axis label
        var xLabelPos = DataToPixel(new Vector2(0, plotBoundsMin.y));
        var xLabelBounds = new SKRect();
        labelPaint.MeasureText(xAxisLabel, ref xLabelBounds);
        canvas.DrawText(xAxisLabel,
            textureWidth / 2 - xLabelBounds.Width / 2,
            textureHeight - margin / 2 + xLabelBounds.Height / 2,
            labelPaint);

        // Y axis label
        var yLabelPos = DataToPixel(new Vector2(plotBoundsMin.x, 0));
        var yLabelBounds = new SKRect();
        labelPaint.MeasureText(yAxisLabel, ref yLabelBounds);

        // Rotate canvas for vertical text
        canvas.Save();
        canvas.RotateDegrees(-90, yLabelPos.X - margin / 2, textureHeight / 2);
        canvas.DrawText(yAxisLabel,
            yLabelPos.X - margin / 2,
            textureHeight / 2 + yLabelBounds.Height / 2,
            labelPaint);
        canvas.Restore();
    }

    void DrawDataPoints(SKCanvas canvas)
    {
        foreach (var series in dataSeries)
        {
            var paint = new SKPaint
            {
                Color = ToSKColor(series.color),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            foreach (var point in series.points)
            {
                var pixel = DataToPixel(point);
                canvas.DrawCircle(pixel.X, pixel.Y, series.pointSize, paint);
            }
        }
    }

    void DrawLegend(SKCanvas canvas)
    {
        if (dataSeries.Count == 0) return;

        var legendPaint = new SKPaint
        {
            Color = ToSKColor(axisColor),
            TextSize = legendFontSize,
            IsAntialias = true,
            Typeface = font
        };

        // Calculate legend size
        float maxTextWidth = 0;
        float totalHeight = 0;
        var tempRect = new SKRect();

        foreach (var series in dataSeries)
        {
            legendPaint.MeasureText(series.name, ref tempRect);
            maxTextWidth = Mathf.Max(maxTextWidth, tempRect.Width);
            totalHeight += legendFontSize + legendEntrySpacing;
        }

        float legendWidth = maxTextWidth + 30 + legendPadding * 2;
        float legendHeight = totalHeight + legendPadding * 2 - legendEntrySpacing;

        // Legend position
        float legendX = legendPosition.x * textureWidth - legendWidth;
        float legendY = legendPosition.y * textureHeight - legendHeight;

        // Draw legend background
        var backgroundPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 200),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRoundRect(
            new SKRect(legendX, legendY, legendX + legendWidth, legendY + legendHeight),
            5, 5, backgroundPaint);

        // Draw legend entries
        float currentY = legendY + legendPadding;
        foreach (var series in dataSeries)
        {
            // Draw color indicator
            var seriesPaint = new SKPaint
            {
                Color = ToSKColor(series.color),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawRect(
                new SKRect(
                    legendX + legendPadding,
                    currentY,
                    legendX + legendPadding + 20,
                    currentY + legendFontSize),
                seriesPaint);

            // Draw series name
            canvas.DrawText(series.name,
                legendX + legendPadding + 25,
                currentY + legendFontSize * 0.8f,
                legendPaint);

            currentY += legendFontSize + legendEntrySpacing;
        }
    }

    SKPoint DataToPixel(Vector2 dataPoint)
    {
        float x = margin + ((dataPoint.x - plotBoundsMin.x) / (plotBoundsMax.x - plotBoundsMin.x)) * (textureWidth - 2 * margin);
        float y = margin + (1 - (dataPoint.y - plotBoundsMin.y) / (plotBoundsMax.y - plotBoundsMin.y)) * (textureHeight - 2 * margin);
        return new SKPoint(x, y);
    }

    void UpdateUnityTexture(SKSurface surface)
    {
        using (var image = surface.Snapshot())
        using (var pixmap = image.PeekPixels())
        {
            var pixelData = pixmap.GetPixels();
            texture.LoadRawTextureData(pixelData, pixmap.RowBytes * textureHeight);
            texture.Apply();
        }
    }
}

[System.Serializable]
public class DataSeries
{
    public string name;
    public List<Vector2> points;
    public Color color;
    public float pointSize;
}