using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 负责将根据MeshData渲染Mesh
/// </summary>
[RequireComponent(typeof(MeshFilter))] //A Mesh(相当于这个物体的样子)
[RequireComponent(typeof(MeshRenderer))] //负责渲染
[RequireComponent(typeof(MeshCollider))] //负责生成碰撞
public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;

    public ChunkData ChunkData { get; private set; } //该脚本挂载的物体用来接受数据

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = _meshFilter.mesh;
    }

    //初始化ChunkData
    public void InitializeChunk(ChunkData data)
    {
        this.ChunkData = data;
    }

    //根据数据渲染
    private void RenderMesh(MeshData meshData)
    {
        _mesh.Clear(); //先清除原来的Mesh
        //subMesh也叫子网格区分水和地面等
        _mesh.subMeshCount = 2;
        //Concat来自于Linq命名空间，用来合并不同的集合（List或Array）
        //也就是将该Block的MeshData的顶点和MeshData里的waterMesh的顶点都加入到该Mesh的顶点数组
        _mesh.vertices = meshData.Vertices.Concat(meshData.WaterMesh.Vertices).ToArray();

        _mesh.SetTriangles(meshData.Triangles.ToArray(), 0);
        //由于该Mesh的顶点数组有两部分构成，所以我们应该加上数组的长度来获得waterMesh的顶点，最后的那个1代表是哪个subMesh
        _mesh.SetTriangles(meshData.WaterMesh.Triangles.Select(val => val + meshData.Vertices.Count).ToArray(), 1);

        _mesh.uv = meshData.UV.Concat(meshData.WaterMesh.UV).ToArray();
        _mesh.RecalculateNormals();

        _meshCollider.sharedMesh = null;
        var collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.ColliderVertices.ToArray();
        collisionMesh.triangles = meshData.ColliderTriangles.ToArray();
        collisionMesh.RecalculateNormals();

        _meshCollider.sharedMesh = collisionMesh;
    }


    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }
}