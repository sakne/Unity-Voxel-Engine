using System.Collections.Generic;
using UnityEngine;

public class VoxelMesh : MonoBehaviour
{
    public TextAsset voxelDataFile;
    public GameObject voxelPrefab;
    public float ObjectSize = 1f;
    public float VoxelSize = 1f;
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

            CreateVoxelMesh(vertices, triangles, x, y, z, VoxelSize);
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

    void CreateVoxelMesh(List<Vector3> vertices, List<int> triangles, int x, int y, int z, float VoxelSize)
    {
        Mesh voxelMesh = voxelPrefab.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] meshVertices = voxelMesh.vertices;
        int[] meshTriangles = voxelMesh.triangles;

        Vector3 offset = new Vector3(x, y, z) * VoxelSize;

        int vertexOffset = vertices.Count;
        for (int i = 0; i < meshVertices.Length; i++)
        {
            Vector3 vertex = meshVertices[i] * VoxelSize + offset;

            // Generate smaller cubes within the voxel
            for (float offsetX = 0f; offsetX < VoxelSize; offsetX += VoxelSize / 2f)
            {
                for (float offsetY = 0f; offsetY < VoxelSize; offsetY += VoxelSize / 2f)
                {
                    for (float offsetZ = 0f; offsetZ < VoxelSize; offsetZ += VoxelSize / 2f)
                    {
                        Vector3 smallerCubeVertex = vertex + new Vector3(offsetX, offsetY, offsetZ);
                        vertices.Add(smallerCubeVertex);
                    }
                }
            }
        }

        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            triangles.Add(meshTriangles[i] + vertexOffset);
            triangles.Add(meshTriangles[i + 1] + vertexOffset);
            triangles.Add(meshTriangles[i + 2] + vertexOffset);
        }

    }

}
