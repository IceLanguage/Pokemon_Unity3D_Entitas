using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using System.IO;
using PokemonBattele;
using Newtonsoft.Json;
using System;

public class InitTrainerSystem : IInitializeSystem
{
    readonly GameContext context;
    public InitTrainerSystem(Contexts contexts)
    {

        context = contexts.game;
    }
    public void Initialize()
    {
        string json = null;
        if (File.Exists(ResourceController.Instance.TrainerDataPath.ToString()))
        {
            json = File.ReadAllText(ResourceController.Instance.TrainerDataPath.ToString());
        }

        Trainer trainer = ScriptableObject.CreateInstance<Trainer>();
        if (json != null)
        {
            trainer = JsonConvert.DeserializeObject<Trainer>(json);
        }
        if (null == trainer || null == trainer.pokemons || 0 == trainer.pokemons.Count)
        {
            //如果没有精灵，就给一只皮卡丘和小火龙
            Pokemon Pikaqiu = PokemonFactory.BuildPokemon(25);

            Pokemon Charmander = PokemonFactory.BuildPokemon(4);
            trainer.pokemons = new List<Pokemon>(6) { Pikaqiu, Charmander };

        }
        else
        {
            foreach (Pokemon pokemon in trainer.pokemons)
                new BattlePokemonData(pokemon);
        }
        if (null == trainer || null == trainer.bagItems|| 0 == trainer.bagItems.Count)
        {
            //如果道具背包没有物品，给5个精灵球
            trainer.bagItems = new List<BagItems>(24)
            {
                BagItems.Build("精灵球",5)
            };
        }
        context.ReplacePlayerData(trainer);

    }



}

