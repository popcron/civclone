using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRenderer : MonoBehaviour {

    public class WorldMesh
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Color> colors = new List<Color>();
        public Mesh mesh;
        
        public WorldMesh()
        {
            mesh = new Mesh();
        }

        public void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        public void AddRenderTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }
    }

    public Material material;
    private List<WorldMesh> meshes = new List<WorldMesh>();

    private int VerticesPerTile
    {
        get
        {
            return 3 * (3 + 12);
        }
    }

    public static Vector3 GetCorner(int index)
    {
        return new Vector3[] {
        new Vector3(0f, 0f, WorldManager.OuterRadius),
        new Vector3(WorldManager.InnerRadius, 0f, 0.5f * WorldManager.OuterRadius),
        new Vector3(WorldManager.InnerRadius, 0f, -0.5f * WorldManager.OuterRadius),
        new Vector3(0f, 0f, -WorldManager.OuterRadius),
        new Vector3(-WorldManager.InnerRadius, 0f, -0.5f * WorldManager.OuterRadius),
        new Vector3(-WorldManager.InnerRadius, 0f, 0.5f * WorldManager.OuterRadius),
        new Vector3(0f, 0f, WorldManager.OuterRadius)
        }[index];
    }

    public void Triangulate(TileObject[] tiles)
    {
        WorldMesh mesh = new WorldMesh();
        for (int i = 0; i < tiles.Length; i++)
        {
            Triangulate(mesh, tiles[i]);
            if (mesh.vertices.Count + VerticesPerTile >= 65534 || i == tiles.Length - 1)
            {
                meshes.Add(mesh);
                mesh = new WorldMesh();
            }
        }

        for (int i = 0; i < meshes.Count; i++)
        {
            CreateMesh(meshes[i]);
        }
    }

    private void CreateMesh(WorldMesh mesh)
    {
        mesh.mesh.vertices = mesh.vertices.ToArray();
        mesh.mesh.triangles = mesh.triangles.ToArray();
        mesh.mesh.colors = mesh.colors.ToArray();
        mesh.mesh.RecalculateNormals();

        MeshRenderer renderer = new GameObject("Mesh").AddComponent<MeshRenderer>();
        renderer.transform.SetParent(transform);
        renderer.sharedMaterial = material;

        MeshFilter filter = renderer.gameObject.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh.mesh;

        MeshCollider collider = renderer.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh.mesh;
    }

    private void Triangulate(WorldMesh mesh, TileObject tile)
    {
        Vector3 center = tile.transform.localPosition;
        for (int i = 0; i < 6; i++)
        {
            mesh.AddRenderTriangle(center, center + GetCorner(i), center + GetCorner(i + 1));
            mesh.AddTriangleColor(tile.color);

            mesh.AddRenderTriangle(center + GetCorner(i + 1), center + GetCorner(i), center + GetCorner(i) + Vector3.down);
            mesh.AddRenderTriangle(center + GetCorner(i) + Vector3.down, center + GetCorner(i + 1) + Vector3.down, center + GetCorner(i + 1));

            mesh.AddTriangleColor(tile.color * 0.5f);
            mesh.AddTriangleColor(tile.color * 0.5f);
        }
    }
}
