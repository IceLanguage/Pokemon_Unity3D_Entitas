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
                    pokemon.PhysicPower += ChangeNum;
                    Debug.Log(pokemon.Ename + "物攻上升" + ChangeNum);
                }
                    
                else
                {
                    pokemon.PhysicPower -= ChangeNum;
                    Debug.Log(pokemon.Ename + "物攻下降" + ChangeNum);
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
                    pokemon.PhysicDefence += ChangeNum;
                    Debug.Log(pokemon.Ename + "物防上升" + ChangeNum);
                }
                    
                else
                {
                    pokemon.PhysicDefence -= ChangeNum;
                    Debug.Log(pokemon.Ename + "物防下降" + ChangeNum);
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
                    pokemon.EnergyPower += ChangeNum;
                    Debug.Log(pokemon.Ename + "特攻上升" + ChangeNum);
                }
                    
                else
                {
                    pokemon.EnergyPower -= ChangeNum;
                    Debug.Log(pokemon.Ename + "特攻下降" + ChangeNum);
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
                    pokemon.EnergyDefence += ChangeNum;
                    Debug.Log(pokemon.Ename + "特防上升" + ChangeNum);
                }
                   
                else
                {
                    pokemon.EnergyDefence -= ChangeNum;
                    Debug.Log(pokemon.Ename + "特防下降" + ChangeNum);
                }
                    
            }
        }
    }

    class ChangeAllStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeAllStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                if (isAdd)
                {
                    pokemon.PhysicPower += ChangeNum;
                    pokemon.PhysicDefence += ChangeNum;
                    pokemon.EnergyPower += ChangeNum;
                    pokemon.EnergyDefence += ChangeNum;
                    pokemon.Speed += ChangeNum;
                    Debug.Log(pokemon.Ename + "所有能力上升" + ChangeNum);
                }

                else
                {
                    pokemon.PhysicPower -= ChangeNum;
                    pokemon.PhysicDefence -= ChangeNum;
                    pokemon.EnergyPower -= ChangeNum;
                    pokemon.EnergyDefence -= ChangeNum;
                    pokemon.Speed -= ChangeNum;
                    Debug.Log(pokemon.Ename + "所有能力下降" + ChangeNum);
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
                    pokemon.Speed += ChangeNum;
                    Debug.Log(pokemon.Ename + "速度上升" + ChangeNum);
                }
                    
                else
                {
                    pokemon.Speed -= ChangeNum;
                    Debug.Log(pokemon.Ename + "速度下降" + ChangeNum);
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
                pokemon.PhysicPower = value;
                Debug.Log(pokemon.Ename + "物攻等级变更为" + value);
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
                pokemon.PhysicDefence = value;
                Debug.Log(pokemon.Ename + "物防等级变更为" + value);
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
                pokemon.EnergyPower = value;
                Debug.Log(pokemon.Ename + "特攻等级变更为" + value);
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
                pokemon.EnergyDefence = value;
                Debug.Log(pokemon.Ename + "特防等级变更为" + value);
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
                pokemon.Speed = value;
                Debug.Log(pokemon.Ename + "速度等级变更为" + value);
            }
        }
    }
}
