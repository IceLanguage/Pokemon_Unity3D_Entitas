using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using PokemonBattele;

class InitBagItemEffectSystem : IInitializeSystem
{
    public void Initialize()
    {
        var dict = ResourceController.Instance.UseBagUItemDict;
        dict["精灵球"] = new UsePokemonBall();
    }
}
