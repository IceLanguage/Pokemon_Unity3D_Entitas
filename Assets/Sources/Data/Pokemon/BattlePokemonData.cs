using PokemonBattelePokemon;
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
       
        MainPokemonType = race.pokemonMainType;
        SecondPokemonType = race.pokemonSecondType;
        curHealth = Health = PokemonCalculation.CalFullHealth(
            race.health, basestats.Health, IV.Health);
        if(Health<=0)
        {
            curHealth = Health = 1;
        }
        PhysicPower = PokemonCalculation.CalCombatBasePower(
            race.phyPower,
            basestats.PhysicPower,
            IV.PhysicPower,
            nature.PhysicPowerAffect
        );
        PhysicDefence = PokemonCalculation.CalCombatBasePower(
            race.phyDefence,
            basestats.PhysicDefence,
            IV.PhysicDefence,
            nature.PhysicDefenceAffect
        );
        EnergyPower = PokemonCalculation.CalCombatBasePower(
            race.energyPower,
            basestats.EnergyPower,
            IV.EnergyPower,
            nature.EnergyPowerEffect
        );
        EnergyDefence = PokemonCalculation.CalCombatBasePower(
            race.energyDefence,
            basestats.EnergyDefence,
            IV.EnergyDefence,
            nature.EnergyDefenceEffect
        );
        Speed = PokemonCalculation.CalCombatBasePower(
            race.speed,
            basestats.Speed,
            IV.Speed,
            nature.SpeedAffect
        );
        skills = new List<int>(pokemon.skillList);
        skillPPs = new List<int>();
        
        skillPPs = skills
            .Select(
                x => ResourceController.Instance.allSkillDic[x].FullPP)
            .ToList();

    }
    
    public void Recover()
    {
        curHealth = Health;
        GameEntity entity = Contexts.sharedInstance.game.GetEntityWithBattlePokemonData(this);
        entity.ReplaceBattlePokemonData(this);
    }
    
}
