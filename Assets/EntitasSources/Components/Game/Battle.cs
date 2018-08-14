using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Unique]
[Game]
public struct BattleFlagComponent : IComponent
{
}

[Game]
public sealed class BattlePokemonDataComponent:IComponent
{
    [PrimaryEntityIndex]
    public BattlePokemonData data;
}

[Game]
public sealed class PokemonDataChangeEventComponent:IComponent
{
    public Action Event;
    
}