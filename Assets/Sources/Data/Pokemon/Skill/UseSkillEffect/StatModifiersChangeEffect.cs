using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    abstract class StatModifiersChangeEffect:UseSkillEffectWithProbability
    {
        protected readonly int ChangeNum;
        protected readonly bool isAdd;
        public StatModifiersChangeEffect(int probability,int num,bool isAdd) : base(probability)
        {
            ChangeNum = num;
            this.isAdd = isAdd;
        }

    }
    abstract class StatModifiersSetEffect : UseSkillEffectWithProbability
    {
        protected readonly int value;
        public StatModifiersSetEffect(int probability, int num) : base(probability)
        {
            value = num;
        }

    }
    class ChangePhysicPowerStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangePhysicPowerStatModifiersEffect(int probability, int num, bool isAdd) 
            : base(probability,num,isAdd)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if(isAdd)
                {
                    pokemon.StatModifiers.PhysicPower += ChangeNum;
                    Debug.Log(pokemon.Ename + "物攻上升" + ChangeNum + System.DateTime.Now);
                }
                    
                else
                {
                    pokemon.StatModifiers.PhysicPower -= ChangeNum;
                    Debug.Log(pokemon.Ename + "物攻下降" + ChangeNum + System.DateTime.Now);
                }
                    
            }
        }
    }

    class ChangePhysicDefenceStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangePhysicDefenceStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if (isAdd)
                {
                    pokemon.StatModifiers.PhysicDefence += ChangeNum;
                    Debug.Log(pokemon.Ename + "物防上升" + ChangeNum + System.DateTime.Now);
                }
                    
                else
                {
                    pokemon.StatModifiers.PhysicDefence -= ChangeNum;
                    Debug.Log(pokemon.Ename + "物防下降" + ChangeNum + System.DateTime.Now);
                }
                    
            }
        }
    }

    class ChangeEnergyPowerStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeEnergyPowerStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if (isAdd)
                {
                    pokemon.StatModifiers.EnergyPower += ChangeNum;
                    Debug.Log(pokemon.Ename + "特攻上升" + ChangeNum + System.DateTime.Now);
                }
                    
                else
                {
                    pokemon.StatModifiers.EnergyPower -= ChangeNum;
                    Debug.Log(pokemon.Ename + "特攻下降" + ChangeNum + System.DateTime.Now);
                }
                    
            }
        }
    }

    class ChangeEnergyDefenceStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeEnergyDefenceStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if (isAdd)
                {
                    pokemon.StatModifiers.EnergyDefence += ChangeNum;
                    Debug.Log(pokemon.Ename + "特防上升" + ChangeNum + System.DateTime.Now);
                }
                   
                else
                {
                    pokemon.StatModifiers.EnergyDefence -= ChangeNum;
                    Debug.Log(pokemon.Ename + "特防下降" + ChangeNum + System.DateTime.Now);
                }
                    
            }
        }
    }

    class ChangeCriticalHitModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeCriticalHitModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }

        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if (isAdd)
                {
                    pokemon.StatModifiers.CriticalHit += 1;
                    Debug.Log(pokemon.Ename + "击中要害概率上升" + ChangeNum + System.DateTime.Now);
                }

                else
                {
                    pokemon.StatModifiers.CriticalHit -= 1;
                    Debug.Log(pokemon.Ename + "击中要害概率下降" + ChangeNum + System.DateTime.Now);
                }

            }
        }
    }

    class ChangeSpeedStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeSpeedStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if (isAdd)
                {
                    pokemon.StatModifiers.Speed += ChangeNum;
                    Debug.Log(pokemon.Ename + "速度上升" + ChangeNum + System.DateTime.Now);
                }
                    
                else
                {
                    pokemon.StatModifiers.Speed -= ChangeNum;
                    Debug.Log(pokemon.Ename + "速度下降" + ChangeNum + System.DateTime.Now);
                }
                    
            }
        }
    }

    class ChangeHitRateStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeHitRateStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if (isAdd)
                {
                    pokemon.StatModifiers.HitRate += ChangeNum;
                    Debug.Log(pokemon.Ename + "命中率上升" + ChangeNum + System.DateTime.Now);
                }

                else
                {
                    pokemon.StatModifiers.HitRate -= ChangeNum;
                    Debug.Log(pokemon.Ename + "命中率下降" + ChangeNum + System.DateTime.Now);
                }

            }
        }
    }

    class ChangeAvoidanceRateStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeAvoidanceRateStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if (isAdd)
                {
                    pokemon.StatModifiers.AvoidanceRate += ChangeNum;
                    Debug.Log(pokemon.Ename + "回避率上升" + ChangeNum + System.DateTime.Now);
                }

                else
                {
                    pokemon.StatModifiers.AvoidanceRate -= ChangeNum;
                    Debug.Log(pokemon.Ename + "回避率下降" + ChangeNum + System.DateTime.Now);
                }

            }
        }
    }

    class SetPhysicPowerStatModifiersEffect : StatModifiersSetEffect
    {
        public SetPhysicPowerStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.StatModifiers.PhysicPower = value;
                Debug.Log(pokemon.Ename + "物攻等级变更为" + value + System.DateTime.Now);
            }
        }
    }
    class SetPhysicDefenceStatModifiersEffect : StatModifiersSetEffect
    {
        public SetPhysicDefenceStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.StatModifiers.PhysicDefence = value;
                Debug.Log(pokemon.Ename + "物防等级变更为" + value + System.DateTime.Now);
            }
        }
    }

    class SetEnergyPowerStatModifiersEffect : StatModifiersSetEffect
    {
        public SetEnergyPowerStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.StatModifiers.EnergyPower = value;
                Debug.Log(pokemon.Ename + "特攻等级变更为" + value + System.DateTime.Now);
            }
        }
    }
    class SetEnergyDefenceStatModifiersEffect : StatModifiersSetEffect
    {
        public SetEnergyDefenceStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.StatModifiers.EnergyDefence = value;
                Debug.Log(pokemon.Ename + "特防等级变更为" + value + System.DateTime.Now);
            }
        }
    }

    class SetSpeedStatModifiersEffect : StatModifiersSetEffect
    {
        public SetSpeedStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.StatModifiers.Speed = value;
                Debug.Log(pokemon.Ename + "速度等级变更为" + value + System.DateTime.Now);
            }
        }
    }
}
