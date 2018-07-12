using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

/// <summary>
/// 草地生成系统
/// </summary>
public class AddGlassSystem : ReactiveSystem<GameEntity>
{
    readonly Transform GlassLand = new GameObject("GlassLand").transform;
    readonly GameContext _context;
    readonly Material grassMat;
    readonly RandomService randomService = new RandomService();

    private MeshFilter mf;
    private MeshRenderer _renderer;
    private Mesh m;    
    private List<Vector3> verts = new List<Vector3>();
    private GameObject grass;
    private MeshRenderer mr;

    private const int terrainSize = 1;
    private const int grassCountPerPatch = 1;

    public AddGlassSystem(Contexts contexts, Material grassMat) : base(contexts.game)
    {
        _context = contexts.game;
        this.grassMat = new Material(grassMat);

    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(GameEntity e in entities)
        {
            Vector3 grassPos = e.glassPos.pos;
            if (!e.hasGlassForces)
                e.AddGlassForces(new List<Force>());
            GenerateGlassField(grassPos, e);
        }

    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGlassPos;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.GlassPos);
    }

    /// <summary>
    /// 草地生成
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    private void GenerateGlassField(Vector3 pos, GameEntity entity)
    {     
        randomService.Initialize(System.DateTime.Now.Millisecond);
        List<int> indices = new List<int>();
        for (int i = 0; i < 65000; i++)
        {
            indices.Add(i);
        }

        Vector3 startPosition = new Vector3(0, 0, 0);
        Vector3 patchSize = new Vector3(1, 0, 1);

        for (int x = 0; x < terrainSize; x++)
        {
            for (int y = 0; y < terrainSize; y++)
            {
                GenerateGrass(startPosition, patchSize);
                startPosition.x += patchSize.x;
            }

            startPosition.x = 0;
            startPosition.z += patchSize.z;
        }


        while (verts.Count > 65000)
        {
            m = new Mesh();
            m.vertices = verts.GetRange(0, 65000).ToArray();
            m.SetIndices(indices.ToArray(), MeshTopology.Points, 0);

            InstateGrassLand(pos, entity);
            verts.RemoveRange(0, 65000);
            return;
        }

        m = new Mesh();
        m.vertices = verts.ToArray();
        m.SetIndices(indices.GetRange(0, verts.Count).ToArray(), MeshTopology.Points, 0);
        InstateGrassLand(pos, entity);
    }

    /// <summary>
    /// 生成草地GameObject
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="entity"></param>
    private void InstateGrassLand(Vector3 pos, GameEntity entity)
    {
        GameObject grassGo = new GameObject("Glass");
        grassGo.transform.position = pos;
        grassGo.transform.parent = GlassLand;
        //entity.AddGlass(grassGo);

        grassGo.Link(entity, _context);

        mf = grassGo.AddComponent<MeshFilter>();
        _renderer = grassGo.AddComponent<MeshRenderer>();
        _renderer.sharedMaterial = new Material(grassMat);
        mf.mesh = m;
        if (_renderer.sharedMaterial.HasProperty("_Height"))
        {
            _renderer.sharedMaterial.SetFloat("_Height", randomService.Float(1.5f, 3f));
        }
        entity.AddGlassMeshRender(_renderer);
    }


    /// <summary>
    /// 生成草地节点
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="patchSize"></param>
    private void GenerateGrass(Vector3 startPosition, Vector3 patchSize)
    {
        for (var i = 0; i < grassCountPerPatch; i++)
        {
            var randomizedZDistance = randomService.Float(0, 1) * patchSize.z;
            var randomizedXDistance = randomService.Float(0, 1) * patchSize.x;

            int indexX = (int)((startPosition.x + randomizedXDistance));
            int indexZ = (int)((startPosition.z + randomizedZDistance));

            if (indexX >= terrainSize)
            {
                indexX = (int)terrainSize - 1;
            }

            if (indexZ >= terrainSize)
            {
                indexZ = (int)terrainSize - 1;
            }

            Vector3 currentPosition = new Vector3(startPosition.x + randomizedXDistance, 0, startPosition.z + randomizedZDistance);

            this.verts.Add(currentPosition);
        }
    }


}
