using UnityEngine;
using System.Collections;
using PokemonBattele;

public static class ObjectPoolController 
{
    public static ObjectPool_Queue<GameObject> PokemonBallObjectsPool = 
        new ObjectPool_Queue<GameObject>();

    public static ObjectPool_Dict<int, GameObject> SkillEffectsObjectPool = 
        new ObjectPool_Dict<int, GameObject>();

    public static ObjectPool_Dict<int, GameObject> PokemonObjectsPool =
        new ObjectPool_Dict<int, GameObject>();

}
