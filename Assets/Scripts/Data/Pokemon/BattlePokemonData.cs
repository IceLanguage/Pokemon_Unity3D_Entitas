using PokemonBattele;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class BattlePokemonData : PokemonBaseData
{
    /// <summary>
    /// 增幅
    /// </summary>
    public class Increase
    {
        public float physicPower = 1;

        public float physicDefence = 1;

        public float energyPower = 1;

        public float energyDefence = 1;

        public float speed = 1;
    }
    public Increase increase = new Increase();
    public static Dictionary<int, BattlePokemonData> Context = new Dictionary<int, BattlePokemonData>();
    public readonly int ID;
    protected readonly new int health;
    public readonly Pokemon pokemon;
    public int curHealth;
    public PokemonType MainPokemonType, SecondPokemonType;
    public AbilityType ShowAbility;
    public Nature nature;
    public Basestats basestats;
    public IndividualValues IV;
    public readonly Race race;
    public string Ename;
    public List<int> skills;
    public List<int> skillPPs;
    public Transform transform;
    public AbnormalStateEnum Abnormal;
    private StatModifiers statModifiers = new StatModifiers();
    public readonly GameEntity entity;
    private List<ChangeStateEnumForPokemon> changeStates = new List<ChangeStateEnumForPokemon>();
    public SkillType ChooseSkillType;
    public int LastUseSkillID = -1;
    public new int PhysicPower
    {
        get
        {
            
            return (int)(base.PhysicPower* increase.physicPower);
        }

        private set
        {
            base.PhysicPower = value;
        }
    }
    public new int PhysicDefence
    {
        get
        {
            return (int)(base.PhysicDefence * increase.physicDefence);
        }

        private set
        {
            base.PhysicDefence = value;
        }
    }
    public new int EnergyPower
    {
        get
        {
            
            return (int)(base.EnergyPower * increase.energyPower);
        }

        private set
        {
            base.EnergyPower = value;
        }
    }
    public new int EnergyDefence
    {
        get
        {
            return (int)(base.EnergyDefence * increase.energyDefence);
        }

        private set
        {
            base.EnergyDefence = value;
        }
    }
    public new int Speed
    {
        get
        {
            return (int)(base.Speed * increase.speed);
        }

        private set
        {
            base.Speed = value;
        }
    }
    public new int Health
    {
        get
        {
            return base.Health;
        }

        private set
        {
            base.Health = value;
        }
    }
    public void AddChangeState(ChangeStateEnumForPokemon state)
    {
        bool canAddState = true;
        //考虑特性
        if (AbilityManager.AbilityImpacts.ContainsKey(ShowAbility))
            AbilityManager.AbilityImpacts[ShowAbility]
                .OnGetChangeState
                (
                    this, state, ref canAddState
                );

        if (!canAddState) return;
        if(!ChangeStateForPokemonEnums.Contains(state))

        {
            ChangeStateForPokemonEnums.Add(state);
            ChangeStateForPokemon.ChangeStateForPokemons[state].Init(this);
        }
       
    }
    public void RemoveChangeState(ChangeStateEnumForPokemon state)
    {
        if (ChangeStateForPokemonEnums.Contains(state))

        {
            ChangeStateForPokemonEnums.Remove(state);
        }

    }
    public void ClearChangeStates()
    {
        int count = changeStates.Count;
        for(int i =count-1;i>=0;i--)
        {
            RemoveChangeState(changeStates[i]);
        }
    }
    public List<ChangeStateEnumForPokemon> ChangeStateForPokemonEnums
    {
        get
        {
            return changeStates;
        }
    }
    //异常状态
    public PokemonState StateForAbnormal
    {
        get
        {
            return AbnormalState.Abnormalstates[Abnormal];
        }
    }

    public StatModifiers StatModifiers
    {
        get
        {
            return statModifiers;
        }

        set
        {
            //考虑特性
            if (AbilityManager.AbilityImpacts.ContainsKey(ShowAbility))
                AbilityManager.AbilityImpacts[ShowAbility]
                    .OnStatModifiersChange
                    (
                        this,ref value
                    );
            statModifiers = value;
            ReCalPokemonData();

        }
    }

    //设置新的异常状态
    public void SetAbnormalStateEnum(AbnormalStateEnum newState)
    {
        if (newState == Abnormal) return;
        //考虑特性
        bool can = true;
        if (AbilityManager.AbilityImpacts.ContainsKey(ShowAbility))
            AbilityManager.AbilityImpacts[ShowAbility]
                .OnGetAbormal
                (
                    this, newState,ref can
                );
        if (!can) return;
        AbnormalState.Abnormalstates[newState].Init(this);
    }

    public BattlePokemonData(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        race = pokemon.PokeRace;
        basestats = pokemon.Basestats;
        IV = pokemon.IV;
        nature = pokemon.PokeNature;
        ShowAbility = pokemon.ShowAbility;
        Ename = pokemon.Ename;

        LHCoroutine.CoroutineManager.DoCoroutine(InitPokemonData());

        entity = Contexts.sharedInstance.game.CreateEntity();
        entity.AddBattlePokemonData(this);
        Action action = DefaultAction;
        entity.AddPokemonDataChangeEvent(action);

        ID = pokemon.GetInstanceID();
        Context[ID] = this;
    }
    private void DefaultAction() { }
    private IEnumerator InitPokemonData()
    {
        yield return new WaitWhile
        (() =>
            {
                return ResourceController.Instance.allSkillDic.Count<612;
            }
        );   
        curHealth = Health = PokemonCalculation.CalFullHealth(
            race.health, basestats.Health, IV.Health);
        if(Health<=0)
        {
            curHealth = Health = 1;
        }
        
        skills = new List<int>(pokemon.skillList);
        skillPPs = new List<int>();       
        skillPPs = skills
            .Select(
                x => ResourceController.Instance.allSkillDic[x].FullPP)
            .ToList();

        ReCalPokemonData();
    }

    private void ReCalPokemonData()
    {
        MainPokemonType = race.pokemonMainType;
        SecondPokemonType = race.pokemonSecondType;
        PhysicPower = PokemonCalculation.CalCombatBasePower(
            race.phyPower,
            basestats.PhysicPower,
            IV.PhysicPower,
            nature.PhysicPowerAffect,
            StatModifiers.ActualCorrection[StatModifiers.PhysicPower]
        );
        PhysicDefence = PokemonCalculation.CalCombatBasePower(
            race.phyDefence,
            basestats.PhysicDefence,
            IV.PhysicDefence,
            nature.PhysicDefenceAffect,
            StatModifiers.ActualCorrection[StatModifiers.PhysicDefence]
        );
        EnergyPower = PokemonCalculation.CalCombatBasePower(
            race.energyPower,
            basestats.EnergyPower,
            IV.EnergyPower,
            nature.EnergyPowerEffect,
            StatModifiers.ActualCorrection[StatModifiers.EnergyPower]
        );
        EnergyDefence = PokemonCalculation.CalCombatBasePower(
            race.energyDefence,
            basestats.EnergyDefence,
            IV.EnergyDefence,
            nature.EnergyDefenceEffect,
            StatModifiers.ActualCorrection[StatModifiers.EnergyDefence]
        );
        Speed = PokemonCalculation.CalCombatBasePower(
            race.speed,
            basestats.Speed,
            IV.Speed,
            nature.SpeedAffect,
            StatModifiers.ActualCorrection[StatModifiers.Speed]
        );
        if(entity != null)
            entity.ReplaceBattlePokemonData(this);
        var playData = Contexts.sharedInstance.game.playerData.scriptableObject;
        Contexts.sharedInstance.game.ReplacePlayerData(playData);
    }
    //恢复
    public void Recover()
    {
        LastUseSkillID = -1;
        increase = new Increase();
        StatModifiers = new StatModifiers();
        SetAbnormalStateEnum(AbnormalStateEnum.Normal);
        ClearChangeStates();
        curHealth = Health;
        InitPokemonData();
        
    }
    
}
