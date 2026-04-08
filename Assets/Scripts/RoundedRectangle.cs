using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RoundedRectangle : Graphic
{
    [Range(0, 100)] public float radius = 20f;
    [Range(4, 64)] public int steps = 16;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = rectTransform.rect;

        // CRITICAL FIX: This line prevents the "weird shape" by capping the radius
        float maxAllowedRadius = Mathf.Min(r.width, r.height) * 0.5f;
        float safeRadius = Mathf.Clamp(radius, 0, maxAllowedRadius);

        // Define the 4 corner centers correctly
        Vector2[] centers = new Vector2[] {
            new Vector2(r.xMax - safeRadius, r.yMax - safeRadius), // Top Right
            new Vector2(r.xMin + safeRadius, r.yMax - safeRadius), // Top Left
            new Vector2(r.xMin + safeRadius, r.yMin + safeRadius), // Bottom Left
            new Vector2(r.xMax - safeRadius, r.yMin + safeRadius)  // Bottom Right
        };

        // Add a center point to anchor the triangles
        vh.AddVert(new Vector3(r.center.x, r.center.y, 0), color, Vector2.zero);

        int vCount = 1;
        for (int i = 0; i < 4; i++)
        {
            float startAngle = i * 90f;
            for (int j = 0; j <= steps; j++)
            {
                float ang = (startAngle + (j / (float)steps) * 90f) * Mathf.Deg2Rad;
                float x = centers[i].x + Mathf.Cos(ang) * safeRadius;
                float y = centers[i].y + Mathf.Sin(ang) * safeRadius;
                
                vh.AddVert(new Vector3(x, y, 0), color, Vector2.zero);
                if (vCount > 1) vh.AddTriangle(0, vCount - 1, vCount);
                vCount++;
            }
        }
        vh.AddTriangle(0, vCount - 1, 1); // Close the loop
    }

    void AddCorner(VertexHelper vh, Vector2 center, float startAngle, float r)
    {
        for (int i = 0; i <= steps; i++)
        {
            float ang = (startAngle + (i / (float)steps) * 90f) * Mathf.Deg2Rad;
            vh.AddVert(new Vector3(center.x + Mathf.Cos(ang) * r, center.y + Mathf.Sin(ang) * r, 0), color, Vector2.zero);
        }
    }

    private void Update() { if (!Application.isPlaying) SetAllDirty(); }
}