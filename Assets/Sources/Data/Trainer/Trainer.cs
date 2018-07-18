using PokemonBattele;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Trainer : ScriptableObject {

    public List<Pokemon> pokemons;
    [SerializeField]
    public List<BagItems> bagItems;
}
