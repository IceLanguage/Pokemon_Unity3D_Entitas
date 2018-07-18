using PokemonBattele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class BattlePokemonData : PokemonBaseData
{
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
    public AbnormalState Abnormal;
    private StatModifiers statModifiers = new StatModifiers();
    //异常状态
    public PokemonState StateForAbnormal
    {
        get
        {
            return PokemonState.Abnormalstates[Abnormal];
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
            statModifiers = value;
            ReCalPokemonData();

        }
    }

    //设置新的异常状态
    public void SetAbnormalState(AbnormalState newState)
    {
        PokemonState.Abnormalstates[newState].Init(this);
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

        InitPokemonData();

        var entity = Contexts.sharedInstance.game.CreateEntity();
        entity.AddBattlePokemonData(this);
        Action action = DefaultAction;
        entity.AddPokemonDataChangeEvent(action);

        ID = pokemon.GetInstanceID();
        Context[ID] = this;
    }
    private void DefaultAction() { }
    private void InitPokemonData()
    {
              
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
        GameEntity entity = Contexts.sharedInstance.game.GetEntityWithBattlePokemonData(this);
        entity.ReplaceBattlePokemonData(this);
    }
    //恢复
    public void Recover()
    {
        StatModifiers = new StatModifiers();
        SetAbnormalState(AbnormalState.Normal);
        InitPokemonData();
        
    }
    
}
