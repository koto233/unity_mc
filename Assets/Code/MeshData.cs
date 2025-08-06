using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于保存Face的Mesh数据信息
/// </summary>
public class MeshData
{
    //用List而不用数组是为了更容易添加新的顶点进去
    public List<Vector3> Vertices = new List<Vector3>();
    public List<int> Triangles = new List<int>();

    public List<Vector2> UV = new List<Vector2>();

    //单独出来是因为有些Face是需要渲染出来而不需要有碰撞，比如Water
    public List<Vector3> ColliderVertices = new List<Vector3>();
    public List<int> ColliderTriangles = new List<int>();

    public MeshData WaterMesh;
    private bool _isMainMesh = true; //不是水方块
    

    public MeshData(bool isMainMesh)
    {
        if (isMainMesh)
        {
            WaterMesh = new MeshData(false); //构造函数，但传入的是false,也就是waterMesh将会是空
        }
    }

    /// <summary>
    /// 添加顶点
    /// </summary>
    /// <param name="vertex">顶点数据</param>
    /// <param name="vertexGeneratesCollider">是否碰撞</param>
    public void AddVertex(Vector3 vertex, bool vertexGeneratesCollider)
    {
        Vertices.Add(vertex);
        if (vertexGeneratesCollider)
        {
            ColliderVertices.Add(vertex);
        }
    }

    /// <summary>
    ///  添加三角面，一个Mesh实际就是很多个三角面
    /// </summary>
    /// <returns></returns>
    public void AddQuadTriangles(bool quadGeneratesCollider)
    {
        Triangles.Add(Vertices.Count - 4);
        Triangles.Add(Vertices.Count - 3);
        Triangles.Add(Vertices.Count - 2);

        Triangles.Add(Vertices.Count - 4);
        Triangles.Add(Vertices.Count - 2);
        Triangles.Add(Vertices.Count - 1);

        // 如果要生成碰撞，需要将顶点再加入到colliderVertices
        if (quadGeneratesCollider)
        {
            ColliderTriangles.Add(ColliderVertices.Count - 4);
            ColliderTriangles.Add(ColliderVertices.Count - 3);
            ColliderTriangles.Add(ColliderVertices.Count - 2);
            ColliderTriangles.Add(ColliderVertices.Count - 4);
            ColliderTriangles.Add(ColliderVertices.Count - 2);
            ColliderTriangles.Add(ColliderVertices.Count - 1);
        }
    }
}