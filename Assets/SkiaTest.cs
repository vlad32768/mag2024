using UnityEngine;
using SkiaSharp;

public class SkiaTest : MonoBehaviour
{
    void Start()
    {
        var info = new SKImageInfo(256, 256);
        using (var surface = SKSurface.Create(info))
        {
            Debug.Log("SkiaSharp works! Surface created.");
        }
    }
}