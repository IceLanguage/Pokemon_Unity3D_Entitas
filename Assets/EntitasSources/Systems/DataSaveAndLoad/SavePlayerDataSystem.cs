using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Entitas;
using Newtonsoft.Json;

class SavePlayerDataSystem : ReactiveSystem<GameEntity>
{
    readonly GameContext context;
    private string path = ResourceController.Instance.TrainerDataPath.ToString();
    public SavePlayerDataSystem(Contexts contexts) :base(contexts.game)
    {
        context = contexts.game;
    }
    protected override void Execute(List<GameEntity> entities)
    {
        DebugHelper.Log("正在保存玩家数据");
        foreach(var entity in entities)
        {
            string json = JsonConvert.SerializeObject(entity.playerData.scriptableObject);
            
           
            int index = path.LastIndexOf('/');
            path = path.Substring(0, index);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllText(ResourceController.Instance.TrainerDataPath.ToString(),
                json, Encoding.UTF8);
            break;
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.playerData != null &&!context.isBattleFlag;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.PlayerData);
    }
}
