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
        dict["超级球"] = new UsePokemonBall(1.5f, "超级球");
        dict["高级球"] = new UsePokemonBall(1.5f, "高级球");
        dict["大师球"] = new UsePokemonBall(255f, "大师球");
        dict["速度球"] = new UseFastBall();
        dict["捕网球"] = new UseNetBall();
        dict["先机球"] = new UseQuickBall();
        dict["计时球"] = new UseTimerBall();

        //dict["力量强化"] = new UseBattleProp("力量强化",
        //    new ChangePhysicPowerStatModifiersEffect(-1, 2, true, true));
        //dict["防御强化"] = new UseBattleProp("防御强化",
        //    new ChangePhysicDefenceStatModifiersEffect(-1, 2, true, true));
    }
}
