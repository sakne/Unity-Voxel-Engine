using System.Collections.Generic;
using UnityEngine;

public class VoxelMesh : MonoBehaviour
{
    public TextAsset voxelDataFile;
    public GameObject voxelPrefab;
    public float voxelSize = 1f;
    private Vector3 location;

    private Dictionary<Vector3Int, int> voxelHealth; // Map voxel position to its health

    void Start()
    {
        location = transform.position;
        string fileText = voxelDataFile.text;

        string[] lines = fileText.Split('\n');

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        voxelHealth = new Dictionary<Vector3Int, int>(); // Initialize the voxel health dictionary

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

            int health;
            if (!int.TryParse(values[3], out health))
            {
                Debug.LogError("Invalid health value at line " + (i + 1) + ": " + values[3]);
                continue;
            }

            CreateVoxelMesh(vertices, triangles, x, y, z);
            voxelHealth[new Vector3Int(x, y, z)] = health; // Store the voxel's health
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
        combinedObject.tag = "voxelmesh";

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
            vertices.Add(meshVertices[i] * voxelSize + offset);
        }

        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            triangles.Add(meshTriangles[i] + vertexOffset);
            triangles.Add(meshTriangles[i + 1] + vertexOffset);
            triangles.Add(meshTriangles[i + 2] + vertexOffset);
        }

        int initialHealth = 3;

        // Store the voxel cube position and health value in a dictionary
        Vector3Int voxelPosition = new Vector3Int(x, y, z);
        voxelHealth.Add(voxelPosition, initialHealth);
    }

    public void DamageVoxel(int x, int y, int z, int damage)
    {
        Vector3Int voxelPosition = new Vector3Int(x, y, z);

        if (voxelHealth.ContainsKey(voxelPosition))
        {
            voxelHealth[voxelPosition] -= damage;

            if (voxelHealth[voxelPosition] <= 0)
            {
                DestroyVoxel(x, y, z);
            }
        }
        else
        {
            Debug.LogWarning("Invalid voxel index: (" + x + ", " + y + ", " + z + ")");
        }
    }

    void DestroyVoxel(int x, int y, int z)
    {
        Vector3Int voxelPosition = new Vector3Int(x, y, z);

        if (voxelHealth.ContainsKey(voxelPosition))
        {
            voxelHealth.Remove(voxelPosition);

            // Find the voxel cube game object at the given position
            Transform voxel = transform.Find("(" + x + ", " + y + ", " + z + ")");

            if (voxel != null)
            {
                // Destroy the voxel cube game object
                Destroy(voxel.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Invalid voxel index: (" + x + ", " + y + ", " + z + ")");
        }
    }
}
