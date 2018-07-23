using System.Collections.Generic;
using Entitas;

public sealed class DestroyEntitySystem : ICleanupSystem
{

    readonly IGroup<GameEntity> _group;
    readonly List<GameEntity> _buffer = new List<GameEntity>();

    public DestroyEntitySystem(Contexts contexts)
    {
        _group = contexts.game.GetGroup(GameMatcher.Destroy);
    }

    public void Cleanup()
    {
        var enumerator = _group.GetEntities(_buffer).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                enumerator.Current.Destroy();

            }
        }
        finally
        {
            enumerator.Dispose();
        }
       
    }
}