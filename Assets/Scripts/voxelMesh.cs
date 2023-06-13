using UnityEngine;
using System.Collections.Generic;

public class VoxelMesh : MonoBehaviour
{
    public TextAsset voxelDataFile;
    public GameObject voxelPrefab;
    public float voxelSize = 1f;
    private Vector3 location;

    void Start()
    {
        location = transform.position;
        string fileText = voxelDataFile.text;

        string[] lines = fileText.Split('\n');


        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Iterate over each line
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = line.Split(' ');

             if (values.Length != 4)
            {
                Debug.LogError("Invalid line format at line " + (i + 1) + ": " + line);
                continue;
            }

            if (!int.TryParse(values[0], out int x))
            {
                Debug.LogError("Invalid x coordinate at line " + (i + 1) + ": " + values[0]);
                continue;
            }

            if (!int.TryParse(values[1], out int y))
            {
                Debug.LogError("Invalid y coordinate at line " + (i + 1) + ": " + values[1]);
                continue;
            }

            if (!int.TryParse(values[2], out int z))
            {
                Debug.LogError("Invalid z coordinate at line " + (i + 1) + ": " + values[2]);
                continue;
            }

            CreateVoxelMesh(vertices, triangles, x, y, z);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.SetVertices(vertices);
        combinedMesh.SetTriangles(triangles, 0);
        combinedMesh.RecalculateNormals();

        GameObject combinedObject = new GameObject("CombinedVoxelMesh");
        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = combinedObject.AddComponent<MeshCollider>();

        combinedObject.transform.Rotate(Vector3.right, -90f);
        combinedObject.transform.position = location;

        meshFilter.mesh = combinedMesh;
        meshRenderer.material = voxelPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        meshCollider.sharedMesh = combinedMesh;
    }

    void CreateVoxelMesh(List<Vector3> vertices, List<int> triangles, int x, int y, int z)
    {
        Mesh voxelMesh = voxelPrefab.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] meshVertices = voxelMesh.vertices;
        int[] meshTriangles = voxelMesh.triangles;

        Vector3 offset = new Vector3(x, y, z) * voxelSize;

        int vertexOffset = vertices.Count;
        for (int i = 0; i < meshVertices.Length; i++)
        {
            vertices.Add(meshVertices[i] * voxelSize  + offset);
        }

        for (int i = 0; i < meshTriangles.Length; i++)
        {
            triangles.Add(meshTriangles[i] + vertexOffset);
        }
    }
}
