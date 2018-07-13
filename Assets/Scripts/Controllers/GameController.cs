using Entitas;
using Invector.CharacterController;
using UnityEngine;

public class GameController : SingletonMonobehavior<GameController>
{

    Systems _systems;

    private void OnEnable()
    {
        var contexts = Contexts.sharedInstance;
        _systems = new GameSystem(contexts);
    }
    private void Start()
    {
        _systems.Initialize();
    }
    private void Update()
    {
        if (null == _systems ) return;
        _systems.Execute();        
    }
    private void LateUpdate()
    {
        _systems.Cleanup();
    }
    protected override void OnDestroy()
    {
        _systems.TearDown();
        Resources.UnloadUnusedAssets();
    }

    
}
