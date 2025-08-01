using UnityEngine;
using System.Collections.Generic;
public enum GemType
{
    Crystal,    // 水晶 - 长条状
    Diamond,    // 钻石 - 多面体
    Rough,      // 原石 - 不规则块状
    Cluster     // 晶簇 - 多个尖刺
}
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GemGenerator : MonoBehaviour
{
    [Header("宝石参数")]
    public GemType gemType = GemType.Crystal;
    [Range(3, 20)]
    public int complexity = 8;
    [Range(0f, 1f)]
    public float irregularity = 0.3f;
    [Range(0f, 1f)]
    public float sharpness = 0.7f;
    [Range(0f, 2f)]
    public float noiseScale = 0.5f;
    public Vector3 scale = Vector3.one;
    public int randomSeed = 0;

    private MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.mesh == null)
        {
            GenerateGem();
        }
    }

    public void GenerateGem()
    {
        Random.InitState(randomSeed);

        Mesh mesh = CreateGemMesh();
        meshFilter.mesh = mesh;
    }

    public void RandomizeParameters()
    {
        randomSeed = Random.Range(0, 10000);
        irregularity = Random.Range(0.2f, 0.8f);
        sharpness = Random.Range(0.4f, 1f);
        complexity = Random.Range(6, 16);
        noiseScale = Random.Range(0.3f, 1.5f);
        GenerateGem();
    }

    private Mesh CreateGemMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); // 添加UV列表

        switch (gemType)
        {
            case GemType.Crystal:
                CreateCrystalMesh(vertices, triangles, uvs);
                break;
            case GemType.Diamond:
                // CreateDiamondMesh(vertices, triangles, uvs);
                break;
            case GemType.Rough:
                // CreateRoughMesh(vertices, triangles, uvs);
                break;
            case GemType.Cluster:
                // CreateClusterMesh(vertices, triangles, uvs);
                break;
        }

        // 应用噪声变形
        ApplyNoise(vertices);

        // 应用缩放
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = Vector3.Scale(vertices[i], scale);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray(); // 设置UV坐标
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private void CreateCrystalMesh(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        // 创建长条状水晶
        float height = 2f;
        float topRadius = 0.3f;
        float bottomRadius = 0.8f;

        // 底部顶点
        vertices.Add(Vector3.down * height * 0.5f);
        
        // 底部圆环
        for (int i = 0; i < complexity; i++)
        {
            float angle = (float)i / complexity * Mathf.PI * 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * bottomRadius, -height * 0.3f, Mathf.Sin(angle) * bottomRadius);
            vertices.Add(pos);
        }

        // 顶部圆环
        for (int i = 0; i < complexity; i++)
        {
            float angle = (float)i / complexity * Mathf.PI * 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * topRadius, height * 0.3f, Mathf.Sin(angle) * topRadius);
            vertices.Add(pos);
        }

        // 顶部顶点
        vertices.Add(Vector3.up * height * 0.5f);
        
        // 为每个顶点生成对应的UV坐标
        uvs.Add(new Vector2(0.5f, 0)); // 底部中心点
    
        // 底部圆环UV
        for (int i = 0; i < complexity; i++)
        {
            float angle = (float)i / complexity * Mathf.PI * 2;
            float u = Mathf.Cos(angle) * 0.5f + 0.5f;
            float v = Mathf.Sin(angle) * 0.5f + 0.5f;
            uvs.Add(new Vector2(u, v));
        }
    
        // 顶部圆环UV
        for (int i = 0; i < complexity; i++)
        {
            float angle = (float)i / complexity * Mathf.PI * 2;
            float u = Mathf.Cos(angle) * 0.3f + 0.5f;
            float v = Mathf.Sin(angle) * 0.3f + 0.5f;
            uvs.Add(new Vector2(u, v));
        }
    
        uvs.Add(new Vector2(0.5f, 1)); // 顶部中心点

        // 生成三角形
        GenerateCrystalTriangles(triangles, complexity);
    }

    private void CreateDiamondMesh(List<Vector3> vertices, List<int> triangles)
    {
        // 创建钻石形状
        vertices.Add(Vector3.up * 1.2f); // 顶点

        // 上环
        for (int i = 0; i < complexity; i++)
        {
            float angle = (float)i / complexity * Mathf.PI * 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * 0.8f, 0.3f, Mathf.Sin(angle) * 0.8f);
            vertices.Add(pos);
        }

        // 下环
        for (int i = 0; i < complexity; i++)
        {
            float angle = (float)i / complexity * Mathf.PI * 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * 0.6f, -0.3f, Mathf.Sin(angle) * 0.6f);
            vertices.Add(pos);
        }

        vertices.Add(Vector3.down * 1.5f); // 底点

        GenerateDiamondTriangles(triangles, complexity);
    }

    private void CreateRoughMesh(List<Vector3> vertices, List<int> triangles)
    {
        // 创建不规则原石
        CreateIcosphere(vertices, triangles, 1f, 2);
        
        // 添加额外的不规则性
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            float noise = Mathf.PerlinNoise(v.x * 3 + randomSeed, v.z * 3 + randomSeed) - 0.5f;
            vertices[i] = v * (1f + noise * irregularity);
        }
    }

    private void CreateClusterMesh(List<Vector3> vertices, List<int> triangles)
    {
        // 创建晶簇 - 多个小水晶组合
        int clusterCount = Random.Range(3, 6);
        
        for (int cluster = 0; cluster < clusterCount; cluster++)
        {
            int baseIndex = vertices.Count;
            
            // 随机位置和大小
            Vector3 center = Random.insideUnitSphere * 0.5f;
            float size = Random.Range(0.5f, 1.2f);
            
            // 创建小水晶
            List<Vector3> clusterVerts = new List<Vector3>();
            List<int> clusterTris = new List<int>();
            
            // CreateCrystalMesh(clusterVerts, clusterTris);
            
            // 调整位置和大小
            for (int i = 0; i < clusterVerts.Count; i++)
            {
                vertices.Add(clusterVerts[i] * size + center);
            }
            
            // 调整三角形索引
            for (int i = 0; i < clusterTris.Count; i++)
            {
                triangles.Add(clusterTris[i] + baseIndex);
            }
        }
    }

    private void CreateIcosphere(List<Vector3> vertices, List<int> triangles, float radius, int subdivisions)
    {
        // 创建基础二十面体
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertices.AddRange(new Vector3[]
        {
            new Vector3(-1, t, 0).normalized * radius,
            new Vector3(1, t, 0).normalized * radius,
            new Vector3(-1, -t, 0).normalized * radius,
            new Vector3(1, -t, 0).normalized * radius,
            new Vector3(0, -1, t).normalized * radius,
            new Vector3(0, 1, t).normalized * radius,
            new Vector3(0, -1, -t).normalized * radius,
            new Vector3(0, 1, -t).normalized * radius,
            new Vector3(t, 0, -1).normalized * radius,
            new Vector3(t, 0, 1).normalized * radius,
            new Vector3(-t, 0, -1).normalized * radius,
            new Vector3(-t, 0, 1).normalized * radius
        });

        // 基础三角形
        int[,] faces = {
            {0,11,5}, {0,5,1}, {0,1,7}, {0,7,10}, {0,10,11},
            {1,5,9}, {5,11,4}, {11,10,2}, {10,7,6}, {7,1,8},
            {3,9,4}, {3,4,2}, {3,2,6}, {3,6,8}, {3,8,9},
            {4,9,5}, {2,4,11}, {6,2,10}, {8,6,7}, {9,8,1}
        };

        for (int i = 0; i < faces.GetLength(0); i++)
        {
            triangles.Add(faces[i, 0]);
            triangles.Add(faces[i, 1]);
            triangles.Add(faces[i, 2]);
        }
    }

    private void GenerateCrystalTriangles(List<int> triangles, int sides)
    {
        // 底面三角形
        for (int i = 0; i < sides; i++)
        {
            triangles.Add(0);
            triangles.Add(((i + 1) % sides) + 1);
            triangles.Add(i + 1);
        }

        // 侧面三角形
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            
            // 下三角形
            triangles.Add(i + 1);
            triangles.Add(next + 1);
            triangles.Add(i + 1 + sides);
            
            // 上三角形
            triangles.Add(next + 1);
            triangles.Add(next + 1 + sides);
            triangles.Add(i + 1 + sides);
        }

        // 顶面三角形
        for (int i = 0; i < sides; i++)
        {
            triangles.Add(sides * 2 + 1);
            triangles.Add(i + 1 + sides);
            triangles.Add(((i + 1) % sides) + 1 + sides);
        }
    }

    private void GenerateDiamondTriangles(List<int> triangles, int sides)
    {
        // 顶部三角形
        for (int i = 0; i < sides; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(((i + 1) % sides) + 1);
        }

        // 中间带
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            
            triangles.Add(i + 1);
            triangles.Add(next + 1);
            triangles.Add(i + 1 + sides);
            
            triangles.Add(next + 1);
            triangles.Add(next + 1 + sides);
            triangles.Add(i + 1 + sides);
        }

        // 底部三角形
        for (int i = 0; i < sides; i++)
        {
            triangles.Add(sides * 2 + 1);
            triangles.Add(((i + 1) % sides) + 1 + sides);
            triangles.Add(i + 1 + sides);
        }
    }

    private void ApplyNoise(List<Vector3> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            float noise1 = Mathf.PerlinNoise(v.x * 2 + randomSeed, v.z * 2 + randomSeed);
            float noise2 = Mathf.PerlinNoise(v.x * 5 + randomSeed, v.z * 5 + randomSeed) * 0.5f;
            
            float totalNoise = (noise1 + noise2 - 0.75f) * noiseScale * irregularity;
            
            Vector3 direction = v.normalized;
            vertices[i] = v + direction * totalNoise;
        }
    }

    void OnValidate()
    {
        if (Application.isPlaying && meshFilter != null)
        {
            GenerateGem();
        }
    }
}