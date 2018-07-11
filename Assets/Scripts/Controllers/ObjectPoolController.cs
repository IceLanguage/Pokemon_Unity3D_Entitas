using UnityEngine;
using System.Collections;
using PokemonBattelePokemon;

public static class ObjectPoolController 
{
    public static ObjectPool_Dict<int, GameObject> SkillEffectObjectPool =
        new ObjectPool_Dict<int, GameObject>();

    public static ObjectPool_Dict<AbnormalState, GameObject> AbnormalStateObjectPool =
        new ObjectPool_Dict<AbnormalState, GameObject>();

    public static ObjectPool_Queue<GameObject> PokemonBallObjectPool = new ObjectPool_Queue<GameObject>();
}
