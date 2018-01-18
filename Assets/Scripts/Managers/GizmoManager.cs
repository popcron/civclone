//by popcron.itch.io

using UnityEngine;
using System.Collections.Generic;
using System;

public class Gizmos
{
    public static void DrawLine(Vector3 a, Vector3 b, Color color)
    {
        if (!GizmoManager.Show) return;

        GizmoManager.lines.Add(new GizmoManager.GizmoLine(a, b, color));

        try
        {
            UnityEngine.Gizmos.color = color;
            UnityEngine.Gizmos.DrawLine(a, b);
        }
        catch
        {
            //ignores the annoying
            //UnityException: Gizmo drawing functions can only be used in OnDrawGizmos and OnDrawGizmosSelected.
            //error
        }
    }

    public static void DrawTerritory(List<WorldTile> territory, Color primaryColor, Color secondaryColor)
    {
        for (int i = 0; i < territory.Count; i++)
        {
            if (territory[i])
            {
                Vector3 offset = Vector3.up * 0.02f;
                Vector3 position = territory[i].transform.position;

                bool left, right, topLeft, topRight, bottomLeft, bottomRight;
                left = !territory[i].Left || territory[i].Left.Nation != territory[i].Nation;
                right = !territory[i].Right || territory[i].Right.Nation != territory[i].Nation;
                topLeft = !territory[i].TopLeft || territory[i].TopLeft.Nation != territory[i].Nation;
                topRight = !territory[i].TopRight || territory[i].TopRight.Nation != territory[i].Nation;
                bottomLeft = !territory[i].BottomLeft || territory[i].BottomLeft.Nation != territory[i].Nation;
                bottomRight = !territory[i].BottomRight || territory[i].BottomRight.Nation != territory[i].Nation;

                if(left && !topLeft)
                {
                    Vector3 a = WorldRenderer.GetCorner(5) * WorldManager.OuterRadius;
                    Vector3 elevated = new Vector3(position.x, territory[i].TopLeft.transform.position.y, position.z);

                    DrawLine(a + position + offset, a + elevated + offset, primaryColor);
                }

                if (topRight && !right)
                {
                    Vector3 a = WorldRenderer.GetCorner(1) * WorldManager.OuterRadius;
                    Vector3 elevated = new Vector3(position.x, territory[i].Right.transform.position.y, position.z);

                    DrawLine(a + position + offset, a + elevated + offset, primaryColor);
                }

                if (bottomRight && !right)
                {
                    Vector3 a = WorldRenderer.GetCorner(2) * WorldManager.OuterRadius;
                    Vector3 elevated = new Vector3(position.x, territory[i].Right.transform.position.y, position.z);

                    DrawLine(a + position + offset, a + elevated + offset, primaryColor);
                }

                if (right && !topRight)
                {
                    Vector3 a = WorldRenderer.GetCorner(1) * WorldManager.OuterRadius;
                    Vector3 elevated = new Vector3(position.x, territory[i].TopRight.transform.position.y, position.z);

                    DrawLine(a + position + offset, a + elevated + offset, primaryColor);
                }

                if (topLeft && !topRight)
                {
                    Vector3 a = WorldRenderer.GetCorner(0) * WorldManager.OuterRadius;
                    Vector3 elevated = new Vector3(position.x, territory[i].TopRight.transform.position.y, position.z);

                    DrawLine(a + position + offset, a + elevated + offset, primaryColor);
                }

                if (right && !bottomRight)
                {
                    Vector3 a = WorldRenderer.GetCorner(2) * WorldManager.OuterRadius;
                    Vector3 elevated = new Vector3(position.x, territory[i].BottomRight.transform.position.y, position.z);

                    DrawLine(a + position + offset, a + elevated + offset, primaryColor);
                }

                if (right)
                {
                    Vector3 a = WorldRenderer.GetCorner(1) * WorldManager.OuterRadius;
                    Vector3 b = WorldRenderer.GetCorner(2) * WorldManager.OuterRadius;

                    DrawLine(a + position + offset, b + position + offset, primaryColor);
                }
                if (left)
                {
                    Vector3 a = WorldRenderer.GetCorner(4) * WorldManager.OuterRadius;
                    Vector3 b = WorldRenderer.GetCorner(5) * WorldManager.OuterRadius;

                    DrawLine(a + position + offset, b + position + offset, primaryColor);
                }
                if (topRight)
                {
                    Vector3 a = WorldRenderer.GetCorner(0) * WorldManager.OuterRadius;
                    Vector3 b = WorldRenderer.GetCorner(1) * WorldManager.OuterRadius;

                    DrawLine(a + position + offset, b + position + offset, primaryColor);
                }
                if (topLeft)
                {
                    Vector3 a = WorldRenderer.GetCorner(5) * WorldManager.OuterRadius;
                    Vector3 b = WorldRenderer.GetCorner(6) * WorldManager.OuterRadius;

                    DrawLine(a + position + offset, b + position + offset, primaryColor);
                }
                if (bottomRight)
                {
                    Vector3 a = WorldRenderer.GetCorner(2) * WorldManager.OuterRadius;
                    Vector3 b = WorldRenderer.GetCorner(3) * WorldManager.OuterRadius;

                    DrawLine(a + position + offset, b + position + offset, primaryColor);
                }
                if (bottomLeft)
                {
                    Vector3 a = WorldRenderer.GetCorner(3) * WorldManager.OuterRadius;
                    Vector3 b = WorldRenderer.GetCorner(4) * WorldManager.OuterRadius;

                    DrawLine(a + position + offset, b + position + offset, primaryColor);
                }
            }
        }
    }

    public static void DrawTile(Vector3 position, float size, Color color)
    {
        for(int i = 0; i < 6;i++)
        {
            Vector3 a = WorldRenderer.GetCorner(i) * size;
            Vector3 b = WorldRenderer.GetCorner(i + 1) * size;

            DrawLine(a + position, b + position, color);
        }
    }

    public static void DrawBox(Vector3 position, Vector3 size, Color color)
    {
        if (!GizmoManager.Show) return;

        Vector3 point1 = new Vector3(position.x - size.x / 2f, position.y - size.y / 2f, position.z - size.z / 2f);
        Vector3 point2 = new Vector3(position.x + size.x / 2f, position.y - size.y / 2f, position.z - size.z / 2f);
        Vector3 point3 = new Vector3(position.x + size.x / 2f, position.y + size.y / 2f, position.z - size.z / 2f);
        Vector3 point4 = new Vector3(position.x - size.x / 2f, position.y + size.y / 2f, position.z - size.z / 2f);

        Vector3 point5 = new Vector3(position.x - size.x / 2f, position.y - size.y / 2f, position.z + size.z / 2f);
        Vector3 point6 = new Vector3(position.x + size.x / 2f, position.y - size.y / 2f, position.z + size.z / 2f);
        Vector3 point7 = new Vector3(position.x + size.x / 2f, position.y + size.y / 2f, position.z + size.z / 2f);
        Vector3 point8 = new Vector3(position.x - size.x / 2f, position.y + size.y / 2f, position.z + size.z / 2f);

        DrawLine(point1, point2, color);
        DrawLine(point2, point3, color);
        DrawLine(point3, point4, color);
        DrawLine(point4, point1, color);

        DrawLine(point5, point6, color);
        DrawLine(point6, point7, color);
        DrawLine(point7, point8, color);
        DrawLine(point8, point5, color);

        DrawLine(point1, point5, color);
        DrawLine(point2, point6, color);
        DrawLine(point3, point7, color);
        DrawLine(point4, point8, color);
    }

    public static void DrawSquare(Vector3 position, Vector3 size, Color color)
    {
        if (!GizmoManager.Show) return;

        Vector3 point1 = new Vector3(position.x - size.x / 2f, position.y - size.y / 2f, position.z);
        Vector3 point2 = new Vector3(position.x + size.x / 2f, position.y - size.y / 2f, position.z);
        Vector3 point3 = new Vector3(position.x + size.x / 2f, position.y + size.y / 2f, position.z);
        Vector3 point4 = new Vector3(position.x - size.x / 2f, position.y + size.y / 2f, position.z);

        DrawLine(point1, point2, color);
        DrawLine(point2, point3, color);
        DrawLine(point3, point4, color);
        DrawLine(point4, point1, color);
    }

    public static void DrawCircle(Vector3 position, float radius, Color color)
    {
        if (!GizmoManager.Show) return;

        DrawPolygon(position, radius, 18, color);
    }

    public static void DrawPolygon(Vector3 position, float radius, int points, Color color, float offset = 0f)
    {
        if (!GizmoManager.Show) return;

        float angle = 360f / points;
        offset *= Mathf.Deg2Rad;

        for (int i = 0; i < points; ++i)
        {
            float sx = Mathf.Cos(Mathf.Deg2Rad * angle * i + offset) * radius / 2;
            float sy = Mathf.Sin(Mathf.Deg2Rad * angle * i + offset) * radius / 2;

            float nx = Mathf.Cos(Mathf.Deg2Rad * angle * (i + 1) + offset) * radius / 2;
            float ny = Mathf.Sin(Mathf.Deg2Rad * angle * (i + 1) + offset) * radius / 2;

            Vector3 a = new Vector3(sx, position.y, sy);
            Vector3 b = new Vector3(nx, position.y, ny);

            DrawLine(position + a, position + b, color);
        }
    }
}

//Must be attached to a camera.
public class GizmoManager : MonoBehaviour
{
    public struct GizmoLine
    {
        public Vector3 a;
        public Vector3 b;
        public Color color;

        public GizmoLine(Vector3 a, Vector3 b, Color color)
        {
            this.a = a;
            this.b = b;
            this.color = color;
        }
    }

    public bool showGizmos = true;
    public Material material;
    internal static List<GizmoLine> lines = new List<GizmoLine>();

    public static bool Show { get; private set; }

    private void Update()
    {
        Show = showGizmos;
    }

    void OnPostRender()
    {
        GL.PushMatrix();
        material.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        
        for (int i = 0; i < lines.Count; i++)
        {
            GL.Color(lines[i].color);

            Vector3 a = Camera.main.WorldToScreenPoint(lines[i].a);
            Vector3 b = Camera.main.WorldToScreenPoint(lines[i].b);

            GL.Vertex(new Vector2(a.x / Screen.width, a.y / Screen.height));
            GL.Vertex(new Vector2(b.x / Screen.width, b.y / Screen.height));
        }

        GL.End();
        GL.PopMatrix();

        lines.Clear();
    }
}