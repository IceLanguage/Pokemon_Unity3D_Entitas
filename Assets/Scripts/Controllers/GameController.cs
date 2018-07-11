using Entitas;
using Invector.CharacterController;
using UnityEngine;

public class GameController : SingletonMonobehavior<GameController>
{

    Systems _systems;

    private void Start()
    {
        InitSystem();
        
    }

    private void InitSystem()
    {
        var contexts = Contexts.sharedInstance;
        _systems = new GameSystem(contexts);
        _systems.Initialize();
    }

    private void Update()
    {
        if (null == _systems ) return;
        _systems.Execute();
        _systems.Cleanup();
    }

    protected override void OnDestroy()
    {
        _systems.TearDown();
    }

    
}
