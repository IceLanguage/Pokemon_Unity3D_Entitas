using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterPokemon", menuName = "ScriptableObjec/EncounterPokemon")]
public class EncounterPokemon:ScriptableObject
{
    //遭遇的精灵
    public List<int> Pokemoms;
    //遭遇的概率
    public List<int> EncounterProbabilities;

}
