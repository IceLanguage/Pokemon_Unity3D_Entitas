﻿using System;
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

        public override void Effect(BattlePokemonData pokemon)
        {
            if (isAdd)
            {
                pokemon.StatModifiers.PhysicPower += ChangeNum;
                DebugHelper.LogFormat("{0}物攻上升{1}", pokemon.Ename, ChangeNum);
            }

            else
            {
                pokemon.StatModifiers.PhysicPower -= ChangeNum;
                DebugHelper.LogFormat("{0}物攻下降{1}", pokemon.Ename, ChangeNum);
            }
        }


    }

    class ChangePhysicDefenceStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangePhysicDefenceStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            if (isAdd)
            {
                pokemon.StatModifiers.PhysicDefence += ChangeNum;

                DebugHelper.LogFormat("{0}物防上升{1}", pokemon.Ename, ChangeNum);
            }

            else
            {
                pokemon.StatModifiers.PhysicDefence -= ChangeNum;

                DebugHelper.LogFormat("{0}物防下降{1}", pokemon.Ename, ChangeNum);

            }
        }


    }

    class ChangeEnergyPowerStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeEnergyPowerStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            if (isAdd)
            {
                pokemon.StatModifiers.EnergyPower += ChangeNum;

                DebugHelper.LogFormat("{0}特攻上升{1}", pokemon.Ename, ChangeNum);

            }

            else
            {
                pokemon.StatModifiers.EnergyPower -= ChangeNum;

                DebugHelper.LogFormat("{0}特攻下降{1}", pokemon.Ename, ChangeNum);
;
            }
        }


    }

    class ChangeEnergyDefenceStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeEnergyDefenceStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            if (isAdd)
            {
                pokemon.StatModifiers.EnergyDefence += ChangeNum;

                DebugHelper.LogFormat("{0}特防上升{1}", pokemon.Ename, ChangeNum);

            }

            else
            {
                pokemon.StatModifiers.EnergyDefence -= ChangeNum;

                DebugHelper.LogFormat("{0}特防下降{1}", pokemon.Ename, ChangeNum);

            }
        }


    }

    class ChangeCriticalHitModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeCriticalHitModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            if (isAdd)
            {
                pokemon.StatModifiers.CriticalHit += 1;

                DebugHelper.LogFormat("{0}击中要害概率上升{1}", pokemon.Ename, ChangeNum);

            }

            else
            {
                pokemon.StatModifiers.CriticalHit -= 1;

                DebugHelper.LogFormat("{0}击中要害概率下降{1}", pokemon.Ename, ChangeNum);

            }
        }


    }

    class ChangeSpeedStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeSpeedStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            if (isAdd)
            {
                pokemon.StatModifiers.Speed += ChangeNum;

                DebugHelper.LogFormat("{0}速度上升{1}", pokemon.Ename, ChangeNum);

            }

            else
            {
                pokemon.StatModifiers.Speed -= ChangeNum;

                DebugHelper.LogFormat("{0}速度下降{1}", pokemon.Ename, ChangeNum);

            }
        }

    }

    class ChangeHitRateStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeHitRateStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            if (isAdd)
            {
                pokemon.StatModifiers.HitRate += ChangeNum;

                DebugHelper.LogFormat("{0}命中率上升{1}", pokemon.Ename, ChangeNum);

            }

            else
            {
                pokemon.StatModifiers.HitRate -= ChangeNum;

                DebugHelper.LogFormat("{0}命中率下降{1}", pokemon.Ename, ChangeNum);

            }
        }

    }

    class ChangeAvoidanceRateStatModifiersEffect : StatModifiersChangeEffect
    {
        public ChangeAvoidanceRateStatModifiersEffect(int probability, int num, bool isAdd)
            : base(probability, num, isAdd)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            if (isAdd)
            {
                pokemon.StatModifiers.AvoidanceRate += ChangeNum;

                DebugHelper.LogFormat("{0}回避率上升{1}", pokemon.Ename, ChangeNum);

            }

            else
            {
                pokemon.StatModifiers.AvoidanceRate -= ChangeNum;

                DebugHelper.LogFormat("{0}回避率下降{1}", pokemon.Ename, ChangeNum);

            }
        }


    }

    class SetPhysicPowerStatModifiersEffect : StatModifiersSetEffect
    {
        public SetPhysicPowerStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.StatModifiers.PhysicPower = value;

            DebugHelper.LogFormat("{0}物攻等级变更为{1}", pokemon.Ename, value);

        }

    }
    class SetPhysicDefenceStatModifiersEffect : StatModifiersSetEffect
    {
        public SetPhysicDefenceStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.StatModifiers.PhysicDefence = value;

            DebugHelper.LogFormat("{0}物防等级变更为{1}", pokemon.Ename, value);

        }

    }

    class SetEnergyPowerStatModifiersEffect : StatModifiersSetEffect
    {
        public SetEnergyPowerStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.StatModifiers.EnergyPower = value;

            DebugHelper.LogFormat("{0}特攻等级变更为{1}", pokemon.Ename, value);

        }


    }
    class SetEnergyDefenceStatModifiersEffect : StatModifiersSetEffect
    {
        public SetEnergyDefenceStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.StatModifiers.EnergyDefence = value;

            DebugHelper.LogFormat("{0}特防等级变更为{1}", pokemon.Ename, value);
 
        }

    }

    class SetSpeedStatModifiersEffect : StatModifiersSetEffect
    {
        public SetSpeedStatModifiersEffect(int probability, int num)
            : base(probability, num)
        {
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.StatModifiers.Speed = value;

            DebugHelper.LogFormat("{0}速度等级变更为{1}", pokemon.Ename, value);
          
        }


    }
}