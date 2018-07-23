using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Unique]
[Game]
public sealed class PlayerData:IComponent
{
    public Trainer scriptableObject;
}
