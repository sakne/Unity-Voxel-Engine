using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voxelMesh : MonoBehaviour
{
    [Header("Mesh Information")]
    [SerializeField] public Mesh mesh;
    [SerializeField] public Material material;
    [Header("Mesh Options")]
    [SerializeField] public float scale = 1f;
    [SerializeField] public float offset = 0f;
    [SerializeField] public bool flipX = false;
    [SerializeField] public bool flipY = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
