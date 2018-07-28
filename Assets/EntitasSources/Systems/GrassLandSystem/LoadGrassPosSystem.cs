using Entitas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 加载草地配置系统
/// </summary>
internal class LoadGrassPosSystem : IInitializeSystem
{
    readonly GameContext _context;
    public LoadGrassPosSystem(Contexts contexts)
    {
        _context = contexts.game;
    }

    public void Initialize()
    {
        TextAsset t = Resources.Load<TextAsset>("Config/GrassesPos");
        string json = "";
        if (t != null)
            json = t.text;
        else
            Debug.LogError("无法加载草地配置信息");

        GrassPosConfig grassPosConfig = JsonConvert.DeserializeObject<GrassPosConfig>(json);
        if (null == grassPosConfig) return;
        List<Vector3> GrassPosList = grassPosConfig.GetGrassPos();
        GrassPosList = GrassPosList.Distinct().ToList();

        foreach(Vector3 pos in GrassPosList)
        {
            GameEntity entity = _context.CreateEntity();

            ////草地位置稍有偏移
            //Vector3 grassPos = new Vector3(
            //    RandomService.game.Float(-0.1f, 0.1f) + pos.x,
            //    0f,
            //    RandomService.game.Float(-0.1f, 0.1f) + pos.z);

            entity.AddGrassPos(pos);
        }

    }
}
