using Entitas;
using Invector.CharacterController;
using UnityEngine;
using System.Collections;
public class GameController : SingletonMonobehavior<GameController>
{

    Systems _systems;

    private void OnEnable()
    {
        DebugHelper.Init();
        StartCoroutine(InitGameSystem());
    }
    IEnumerator InitGameSystem()
    {
        var e = ResourceController.Instance;
        yield return new WaitWhile(() =>
        {
           
            return !e.LOADITEM || !e.LOADSKILL || !e.LOADSKILLPOOL;
        });
        var contexts = Contexts.sharedInstance;
        _systems = new GameSystem(contexts);
        _systems.Initialize();
    }
    //private void Start()
    //{
    //    _systems.Initialize();
    //}
    private void Update()
    {
        if (null == _systems ) return;
        _systems.Execute();        
    }
    private void FixedUpdate()
    {
        DebugHelper.FixedUpdate();
}
    private void LateUpdate()
    {
        if (null == _systems ) return;
        _systems.Cleanup();
    }
    protected override void OnDestroy()
    {
        _systems.TearDown();
        Resources.UnloadUnusedAssets();
    }

    
}
