using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "EncounterPokemon", menuName = "ScriptableObjec/EncounterPokemon")]
#endif
public class EncounterPokemon:ScriptableObject
{
    //遭遇的精灵
    public List<int> Pokemoms;
    //遭遇的概率
    public List<int> EncounterProbabilities;

}
