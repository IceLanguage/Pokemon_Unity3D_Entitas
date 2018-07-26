using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;
using System.Linq;

/// <summary>
/// 草地生成系统
/// </summary>
public class AddGrassSystem : ReactiveSystem<GameEntity>
{
    readonly Transform GrassLand = new GameObject("GrassLand").transform;
    readonly GameContext _context;
    readonly Material grassMat;
    readonly RandomService randomService = new RandomService();

    private MeshFilter mf;
    private MeshRenderer _renderer;
    private Mesh m;    
    private List<Vector3> verts = new List<Vector3>();

    //private const int terrainSize = 1;
    //private const int grassCountPerPatch = 10;
    //private readonly Vector3[] points = new Vector3[100];
    private readonly TerrainData terrainData;
    public AddGrassSystem(Contexts contexts) : base(contexts.game)
    {
        _context = contexts.game;
       // this.grassMat = grassMat;//new Material(grassMat);
       // terrainData = GameObject.FindWithTag("Terrain").GetComponent<Terrain>().terrainData;
        //for (int i = 0; i < 10; ++i)
        //{
        //    for (int j = 0; j < 10; ++j)
        //    {
        //        points[i * 10 + j] = new Vector3(-0.5f + 0.1f * i, 0, -0.5f + 0.1f * j);
        //    }
        //}
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (GameEntity e in entities)
        {
            Vector3 grassPos = e.grassPos.pos;

            //if (!e.hasGrassForces)
            //    e.AddGrassForces(new List<Force>());

            //InstateGrassLand(grassPos, e);
            GameObject glass = UnityEngine.Object.Instantiate(ResourceController.Instance.glassPrefab, GrassLand);
            glass.transform.position = grassPos;
        }



    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGrassPos;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.GrassPos);
    }

    ///// <summary>
    ///// 草地生成
    ///// </summary>
    ///// <param name="x"></param>
    ///// <param name="z"></param>
    //private void GenerateGrassField(Vector3 pos, GameEntity entity)
    //{     
    //    InstateGrassLand(pos, entity);
    //}

    ///// <summary>
    ///// 生成草地GameObject
    ///// </summary>
    ///// <param name="pos"></param>
    ///// <param name="entity"></param>
    //private void InstateGrassLand(Vector3 pos, GameEntity entity)
    //{
    //    GameObject grassGo = new GameObject("Grass");
    //    grassGo.transform.position = pos;
    //    grassGo.transform.parent = GrassLand;
    //    BuildMesh();
    //    mf = grassGo.AddComponent<MeshFilter>();
    //    _renderer = grassGo.AddComponent<MeshRenderer>();
    //    _renderer.sharedMaterial = grassMat;
    //    mf.mesh = m;

    //    entity.AddGrassMeshRender(_renderer);
    //}

    //private void BuildMesh()
    //{
    //    Vector3[] vertices = new Vector3[13 * 100];
    //    Vector2[] uvs = new Vector2[13 * 100];
    //    Vector3[] normals = new Vector3[13 * 100];
    //    int[] triangles = new int[66 * 100];

    //    float offsetV = 1f / 6f;

    //    Vector3 root = Vector3.zero;
    //    int[] show = new int[100];
    //    for (int i = 0; i < 100; i++)
    //    {
    //        show[i] = RandomService.game.Int(0, 3);
    //    }
    //    for (int k = 0; k < 100; k++)
    //    {
    //        // if (show[k] == 1) continue;
    //        float width = 0.03f * RandomService.game.Float(0.5f, 1.5f);
    //        float height = 1f * RandomService.game.Float(0.5f, 1.5f);
    //        float currentV = 0f;
    //        float currentVertexHeight = 0;
    //        root = points[k];
    //        for (int i = 0; i < 12; i++)
    //        {
    //            normals[13 * k + i] = Vector3.forward;
    //            if (i % 2 == 0)
    //            {
    //                vertices[13 * k + i] = new Vector3(root.x - width, root.y + currentVertexHeight, root.z);
    //                uvs[13 * k + i] = new Vector2(0, currentV);
    //            }
    //            else
    //            {
    //                vertices[13 * k + i] = new Vector3(root.x + width, root.y + currentVertexHeight, root.z);
    //                uvs[13 * k + i] = new Vector2(1, currentV);

    //                currentV += offsetV;
    //                currentVertexHeight = currentV * height;
    //            }
    //        }
    //        float randomB = RandomService.game.Float(-1f, 1f);
    //        vertices[13 * k + 12] = new Vector3
    //            (root.x + randomB * width,
    //            root.y + currentVertexHeight + RandomService.game.Float(0.01f, offsetV / 3f),
    //            root.z);
    //        uvs[13 * k + 12] = new Vector2(0, 1);
    //        normals[13 * k + 12] = Vector3.forward;


    //        int j = 0;
    //        for (int p = 0; p < 11; p++)
    //        {
    //            triangles[66 * k + j++] = 13 * k + p;
    //            triangles[66 * k + j++] = 13 * k + p + 2;
    //            triangles[66 * k + j++] = 13 * k + p + 1;

    //        }
    //        for (int p = 0; p < 11; p++)
    //        {
    //            triangles[66 * k + j++] = 13 * k + p;
    //            triangles[66 * k + j++] = 13 * k + p + 1;
    //            triangles[66 * k + j++] = 13 * k + p + 2;

    //        }
    //        float angle = RandomService.game.Float(0, 90);
    //        Quaternion q = Quaternion.AngleAxis(angle, Vector3.up);
    //        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
    //        for (int i = 0; i < 13; ++i)
    //        {
    //            vertices[13 * k + i] = matrix.MultiplyPoint3x4(vertices[13 * k + i]);
    //        }

    //    }

    //    m = new Mesh()
    //    {
    //        vertices = vertices ,
    //        uv = uvs,
    //        triangles = triangles,
    //        normals = normals
    //    };
    //}

}
