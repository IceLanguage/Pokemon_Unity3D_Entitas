using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using PokemonBattele;

class InitAbilityImpactSystem : IInitializeSystem
{
    public InitAbilityImpactSystem()
    {

    }
    public void Initialize()
    {
        AbilityManager.AbilityImpacts[AbilityType.茂盛] = new Overgrow();
        // 叶绿素
        AbilityManager.AbilityImpacts[AbilityType.猛火] = new Blaze();
        //太阳之力
        AbilityManager.AbilityImpacts[AbilityType.激流] = new Torrent();
        //雨盘
        AbilityManager.AbilityImpacts[AbilityType.虫之预感] = new Swarm();
        AbilityManager.AbilityImpacts[AbilityType.狙击手] = new Sniper();
        AbilityManager.AbilityImpacts[AbilityType.锐利目光] = new KeenEye();
        AbilityManager.AbilityImpacts[AbilityType.蹒跚] = new TangledFeet();
        AbilityManager.AbilityImpacts[AbilityType.健壮胸肌] = new BigPecks();
        AbilityManager.AbilityImpacts[AbilityType.静电] = new Static();
        AbilityManager.AbilityImpacts[AbilityType.避雷针] = new LightningRod();
        AbilityManager.AbilityImpacts[AbilityType.同步] = new Synchronize();
        AbilityManager.AbilityImpacts[AbilityType.精神力] = new InnerFocus();
        AbilityManager.AbilityImpacts[AbilityType.魔法防守] = new MagicGuard();
        AbilityManager.AbilityImpacts[AbilityType.毅力] = new Guts();
        AbilityManager.AbilityImpacts[AbilityType.无防守] = new NoGuard();
        AbilityManager.AbilityImpacts[AbilityType.不屈之心] = new Steadfast();
        //沙隐
        AbilityManager.AbilityImpacts[AbilityType.结实] = new Sturdy();
        //坚硬脑袋
        AbilityManager.AbilityImpacts[AbilityType.我行我素] = new OwnTempo();
        //迟钝
        AbilityManager.AbilityImpacts[AbilityType.再生力] = new Regenerator();
        AbilityManager.AbilityImpacts[AbilityType.磁力] = new MagnetPull();
        AbilityManager.AbilityImpacts[AbilityType.分析] = new Analytic();
        AbilityManager.AbilityImpacts[AbilityType.硬壳盔甲] = new ShellArmor();
        //连续攻击
        //防尘
        AbilityManager.AbilityImpacts[AbilityType.碎裂铠甲] = new WeakArmor();
        AbilityManager.AbilityImpacts[AbilityType.诅咒之躯] = new CursedBody();
        AbilityManager.AbilityImpacts[AbilityType.威吓] = new Intimidate();
        AbilityManager.AbilityImpacts[AbilityType.自信过度] = new Moxie();
        AbilityManager.AbilityImpacts[AbilityType.储水] = new WaterAbsorb();
        //湿润之躯
        AbilityManager.AbilityImpacts[AbilityType.飞毛腿] = new QuickFeet();
        AbilityManager.AbilityImpacts[AbilityType.引火] = new FlashFire();
        //紧张感
        AbilityManager.AbilityImpacts[AbilityType.压迫感] = new Pressure();
        //贪吃鬼
        AbilityManager.AbilityImpacts[AbilityType.免疫] = new Immunity();
        AbilityManager.AbilityImpacts[AbilityType.厚脂肪] = new ThickFat();
    }
}
